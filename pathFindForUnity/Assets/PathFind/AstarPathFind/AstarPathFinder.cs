using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFind
{
    public enum FindPathResult
    {
        None,
        OnTheWay,
        Arrived,
        Blocked,
    }

    public class AstarResult
    {
        public FindPathResult m_result = FindPathResult.None;
        public List<Vector2i> m_pathList = new List<Vector2i>();
    }

    public class AstarPathFinder
    {
        AstarMapMgr m_mapMgr = null;

        NodePool m_nodePool = null;

        List<Node> m_open_list = new List<Node>();
        BitArray m_open_node_hash_list = null;
        List<Node> m_close_list = new List<Node>();
        BitArray m_close_node_hash_list = null;

        public int NEAREST_POS_SEARCH_RADIUS = 5;
        public int PATH_MAX_NODE = 3;

        public void Init(int pathSearchNodeMax, AstarMapMgr mapMgr)
        {
            m_mapMgr = mapMgr;

            m_nodePool = new NodePool(pathSearchNodeMax);

            m_open_node_hash_list = new BitArray(m_mapMgr.MAP_LENGTH_X * m_mapMgr.MAP_WIDTH_Z, false);
            m_close_node_hash_list = new BitArray(m_mapMgr.MAP_LENGTH_X * m_mapMgr.MAP_WIDTH_Z, false);

            m_open_list.Capacity = m_mapMgr.MAP_WIDTH_Z * m_mapMgr.MAP_LENGTH_X;
            m_close_list.Capacity = m_mapMgr.MAP_WIDTH_Z * m_mapMgr.MAP_LENGTH_X;
        }

        public AstarResult AstarFind(PathFindAgent agent, Vector2i srcPos, Vector2i desPos)
        {
            AstarResult ret = new AstarResult();

            ClearNode();

            Vector2i nearest_final_pos = new Vector2i();
            if (!computeNearestFreePos(srcPos, desPos, agent, out nearest_final_pos))
            {
                ret.m_result = FindPathResult.Blocked;
                return ret;
            }
            if (srcPos == nearest_final_pos)
            {
                ret.m_result = FindPathResult.Arrived;
                return ret;
            }

            //a) push starting pos into openNodes
            Node first_node = m_nodePool.getNewNode();
            first_node.m_next = null;
            first_node.m_prev = null;
            first_node.m_pos = srcPos;
            first_node.m_heuristic = (nearest_final_pos - first_node.m_pos).magnitude();
            AddOpenNode(first_node);

            //b) loop
            bool path_found = true;
            bool note_limited_reached = false;
            Node node = null;

            while (!note_limited_reached)
            {
                //b1) is open nodes is empty => failed to find the path
                if (m_open_list.Count == 0)
                {
                    path_found = false;
                    break;
                }

                //b2) get the minimum heuristic node
                node = minHeuristic();

                //b3) if minHeuristic is the finalNode => path was found
                if (node.m_pos == nearest_final_pos)
                {
                    path_found = true;
                    break;
                }

                //b4) move this node from closedNodes to openNodes
                // add all succesors that are not in closedNodes or openNodes to openNodes
                AddCloseNode(node);
                RemoveOpenNode(node);

                for (int i = -1; i <= 1 && !note_limited_reached; ++i)
                {
                    for (int j = -1; j <= 1 && !note_limited_reached; ++j)
                    {
                        Vector2i suc_pos = node.m_pos + new Vector2i(i, j);
                        if (!isOpenOrClosePos(suc_pos))
                        {
                            if (m_mapMgr.TestAgentCells(suc_pos, agent))
                            {
                                Node suc_node = m_nodePool.getNewNode();
                                if (suc_node != null)
                                {
                                    suc_node.m_pos = suc_pos;
                                    suc_node.m_heuristic = heuristic(suc_node.m_pos, nearest_final_pos);
                                    suc_node.m_prev = node;
                                    suc_node.m_next = null;
                                    AddOpenNode(suc_node);
                                }
                                else
                                {
                                    note_limited_reached = true;
                                }
                            }
                        }
                    }
                }
            }

            //if consumed all nodes find best node (to avoid strage behaviour)
            Node last_node = node;
            if (note_limited_reached)
            {
                for (int i = 0; i < m_close_list.Count; ++i)
                {
                    if (m_close_list[i].m_heuristic < last_node.m_heuristic)
                    {
                        last_node = m_close_list[i];
                    }
                }
            }

            if (path_found == false || last_node == first_node)
            {
                ret.m_result = FindPathResult.Blocked;
                return ret;
            }
            else
            {
                Node cur_node = last_node;
                while (cur_node.m_prev != null)
                {
                    cur_node.m_prev.m_next = cur_node;
                    cur_node = cur_node.m_prev;
                }

                cur_node = first_node;

                for (int i = 0; cur_node.m_next != null && i < PATH_MAX_NODE; ++i)
                {
                    ret.m_pathList.Add(cur_node.m_next.m_pos);
                    cur_node = cur_node.m_next;
                }

                ret.m_result = FindPathResult.OnTheWay;

                if (ret.m_result == FindPathResult.OnTheWay && ret.m_pathList.Count == 0)
                {
                    UnityEngine.Debug.LogWarning("没算出路径点");
                }

                return ret;
            }


        }

        void ClearNode()
        {
            m_nodePool.clear();

            m_open_node_hash_list.SetAll(false);
            m_close_node_hash_list.SetAll(false);

            m_open_list.Clear();
            m_close_list.Clear();
        }

        bool computeNearestFreePos(Vector2i scrPos, Vector2i final_pos, PathFindAgent agent, out Vector2i nearest_free_pos)
        {
            nearest_free_pos = new Vector2i(0, 0);
            //ModuleAstarBattle astarBattleModule = SingletonMgr.GetModuleMgr().GetModuleBattle();
            bool has_found = false;
            float nearest_distance = 1000000;
            for (int i = -NEAREST_POS_SEARCH_RADIUS; i <= NEAREST_POS_SEARCH_RADIUS; ++i)
            {
                for (int j = -NEAREST_POS_SEARCH_RADIUS; j <= NEAREST_POS_SEARCH_RADIUS; ++j)
                {
                    Vector2i new_final_pos = final_pos + new Vector2i(i, j);
                    if (m_mapMgr.TestAgentCells(new_final_pos, agent))
                    {
                        float distance_sqr = (final_pos - new_final_pos).sqrMagnitude();
                        if (distance_sqr < nearest_distance)
                        {
                            has_found = true;
                            nearest_free_pos = new_final_pos;
                            nearest_distance = distance_sqr;
                        }
                        else if (distance_sqr == nearest_distance)
                        {
                            if ((new_final_pos - scrPos).sqrMagnitude() < (nearest_free_pos - scrPos).sqrMagnitude())
                            {
                                nearest_free_pos = new_final_pos;
                            }
                        }
                    }
                }
            }

            if (!has_found)
            {
                nearest_free_pos = m_mapMgr.GetNearestCanMovePosFromDes(scrPos, final_pos, agent);
                has_found = true;
            }

            return has_found;
        }

        void AddOpenNode(Node add_node)
        {
            m_open_list.Add(add_node);
            int index = m_mapMgr.GetIndex(add_node.m_pos);
            if (index >= 0 && index < m_open_node_hash_list.Count)
            {
                m_open_node_hash_list[index] = true;
            }
            else
            {
                UnityEngine.Debug.LogError("AddOpenNode " + index);
            }
        }

        void AddCloseNode(Node add_node)
        {
            m_close_list.Add(add_node);
            int index = m_mapMgr.GetIndex(add_node.m_pos);
            if (index < m_close_node_hash_list.Count)
            {
                m_close_node_hash_list[index] = true;
            }
            else
            {
                UnityEngine.Debug.LogError("AddCloseNode " + index);
            }

        }

        void RemoveOpenNode(Node remove_node)
        {
            m_open_list.Remove(remove_node);
            int index = m_mapMgr.GetIndex(remove_node.m_pos);
            if (index < m_open_node_hash_list.Count)
            {
                m_open_node_hash_list[index] = false;
            }
            else
            {
                UnityEngine.Debug.LogError("RemoveOpenNode " + index);
            }

        }

        Node minHeuristic()
        {
            Node minNode = m_open_list[0];
            for (int i = 0; i < m_open_list.Count; ++i)
            {
                if (m_open_list[i].m_heuristic < minNode.m_heuristic)
                {
                    minNode = m_open_list[i];
                }
            }
            return minNode;
        }

        float heuristic(Vector2i pos, Vector2i final_pos)
        {
            Vector2i distance = final_pos - pos;
            return distance.magnitude();
        }

        bool isOpenOrClosePos(Vector2i suc_pos)
        {
            // todo:close数量上升后复杂度急剧增加,可优化，将坐标hash成一个数值，二分查找
            int index = m_mapMgr.GetIndex(suc_pos);

            if (index < 0
                || index >= m_close_node_hash_list.Count)
            {
                return false;
            }

            if (m_close_node_hash_list[index])
            {
                return true;
            }

            if (m_open_node_hash_list[index])
            {
                return true;
            }

            return false;
        }
    }
}
