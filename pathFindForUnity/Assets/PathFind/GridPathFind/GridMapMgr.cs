using UnityEngine;
using System.Collections;

using System.Collections.Generic;

namespace PathFind
{
    public class GridMapCell
    {
        public List<int> m_whoseArea = new List<int>();
    }

    public class GridMapMgr 
    {
        List<GridMapCell> m_mapCellList = new List<GridMapCell>();
        public int MAP_LENGTH_X = 100;
        public int MAP_WIDTH_Z = 100;

        List<GameObject> m_debugGo = new List<GameObject>();

        public void Init(int x, int z)
        {
            MAP_LENGTH_X = x;
            MAP_WIDTH_Z = z;

            for (int i = 0; i < MAP_LENGTH_X; ++i)
            {
                for (int j = 0; j < MAP_WIDTH_Z; ++j)
                {
                    GridMapCell gridMapCell = new GridMapCell();
                    m_mapCellList.Add(gridMapCell);
                }
            }
        }

        public int GetIndex(Vector2i posi)
        {
            //int x = (int)pos.x;
            //int y = (int)pos.y;
            if (posi.m_x < 0 || posi.m_y < 0)
            {
                return -1;
            }
            else
            {
                int index = posi.m_x * MAP_WIDTH_Z + posi.m_y;
                return index;
            }
        }

        public void RegisetrPosList(List<Vector2i> posList, int agentId)
        {
            for(int i = 0; i < posList.Count; ++i)
            {
                Vector2i cellPos = posList[i];
                int index = GetIndex(cellPos);
                if(index >= 0 && index < m_mapCellList.Count)
                {
                    if(m_mapCellList[index].m_whoseArea.Contains(agentId))
                    {
                        Debug.LogError("RegisetrPosList " + agentId);
                    }
                    m_mapCellList[index].m_whoseArea.Add(agentId);
                }
            }
        }

        public void UnRegisterPosList(List<Vector2i> posList, int agentId)
        {
            for (int i = 0; i < posList.Count; ++i)
            {
                Vector2i cellPos = posList[i];
                int index = GetIndex(cellPos);
                if (index >= 0 && index < m_mapCellList.Count)
                {
                    m_mapCellList[index].m_whoseArea.Remove(agentId);
                }
            }
        }

        public bool TestAgentCells(Vector2i posi, PathFindAgent agent)
        {
            if (posi.m_x >= MAP_LENGTH_X
                || posi.m_y >= MAP_WIDTH_Z)
            {
                return false;
            }

            List<Vector2i> outLineList = agent.m_outlineList;
            for (int i = 0; i < outLineList.Count; ++i )
            {
                outLineList[i] = posi + agent.m_outlineOffsetList[i];
            }

            bool canUse = true;
            for (int i = 0; i < outLineList.Count; ++i)
            {
                int index = GetIndex(outLineList[i]);
                if (index >= 0 && index < m_mapCellList.Count)
                {
                    GridMapCell cellInfo = m_mapCellList[index];
                    if (cellInfo.m_whoseArea.Count == 0)
                    {
                        //return true;
                    }
                    else if (cellInfo.m_whoseArea.Count == 1
                        && cellInfo.m_whoseArea[0] == agent.m_agentId)
                    {
                        //return true;
                    }
                    else
                    {
                        //return false;
                        canUse = false;
                    }
                }
                else
                {
                    //return false;
                    canUse = false;
                }
            }

            return canUse;
        }

        public Vector2i GetNearestCanMovePosFromDes(Vector2i srcPosi, Vector2i desPosi, PathFindAgent agent)
        {
            //srcPos.y = 0;
            //desPos.y = 0;
            Common.Vector3 srcPos = new Common.Vector3(srcPosi.m_x, 0, srcPosi.m_y);
            Common.Vector3 desPos = new Common.Vector3(desPosi.m_x, 0, desPosi.m_y);

            Common.Vector3 retPos = desPos;
            Common.Vector3 dirOrigin = srcPos - desPos;
            if (dirOrigin.sqrMagnitude < 0.1 * 0.1)
            {
                return new Vector2i((int)retPos.x, (int)retPos.z);
            }
            dirOrigin.Normalize();

            Common.Vector3 dir = srcPos - retPos;
            //dir.Normalize();
            if (!TestAgentCells(new Vector2i((int)retPos.x, (int)retPos.z), agent))
            {
                while (Common.Vector3.Dot(dirOrigin, dir) > 0)
                {
                    if (TestAgentCells(new Vector2i((int)retPos.x, (int)retPos.z), agent))
                    {
                        break;
                    }
                    else
                    {
                        retPos = retPos + dirOrigin;
                        dir = srcPos - retPos;
                        //dir.Normalize();
                    }
                }
            }

            Vector2i retPosi = new Vector2i((int)retPos.x, (int)retPos.z);
            return retPosi;
        }

        Vector2i ConvertIndexToPos(int index)
        {
            int y = index % MAP_WIDTH_Z;
            int x = index / MAP_WIDTH_Z;
            return new Vector2i(x, y);
        }

        public void ShowColliderData()
        {
            foreach(var iter in m_debugGo)
            {
                GameObject.Destroy(iter);
            }
            UnityEngine.GameObject colliderPrefab = UnityEngine.Resources.Load("Barrier2") as UnityEngine.GameObject;
            for(int i = 0; i < m_mapCellList.Count; ++i)
            {
                GridMapCell iter = m_mapCellList[i];
                if(iter.m_whoseArea.Count != 0)
                {
                    Vector2i pos = ConvertIndexToPos(i);
                    UnityEngine.GameObject.Instantiate(colliderPrefab, new UnityEngine.Vector3(pos.m_x, 0, pos.m_y), UnityEngine.Quaternion.identity);
                }
            }
        }
    }


}


