using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//  对于来回徘徊并且无法判定block的agent，由上层设置计时器来解决吧。。
namespace PathFind
{
    public class PathFindAgent
    {
        public enum State
        {
            None,
            Moving,
            Stopping,
        }

        State m_state = State.None;
        System.Action m_arriveCallback = null;
        System.Action m_blockCallback = null;

        public int m_agentId = 0;
        //Common.Vector2 m_pos = new Common.Vector2(0, 0);
        float m_radius = 0;
        float m_speed = 0;

        //int m_RVOid = 0;
        
        public List<Vector2i> m_outlineOffsetList = new List<Vector2i>();
        public List<Vector2i> m_outlineList = new List<Vector2i>();

        PathFindMgr m_mgr = null;

        Common.Vector2 m_curPos = new Common.Vector2();
        Common.Vector2 m_desPos = new Common.Vector2(0, 0);
        List<Vector2i> m_pathList = new List<Vector2i>();
        Vector2i m_gridPos = new Vector2i();

        public void Init(int id, PathFindMgr mgr, Common.Vector2 pos, float radius, float speed)
        {
            m_agentId = id;
            //m_pos = pos;
            m_curPos = pos;

            m_gridPos = new Vector2i((int)m_curPos.x, (int)m_curPos.y);
            m_mgr = mgr;
            m_radius = radius;
            m_speed = speed;

//             m_RVOid = RVO.Simulator.Instance.addAgent(new RVO.Vector2(pos.x, pos.y), 
//                 PathFindMgr.s_maxNeighborDistanceTakeInCount, 
//                 PathFindMgr.s_maxNeighborTakeInCount, 
//                 PathFindMgr.s_pushParamForUnit, 
//                 PathFindMgr.s_pushParamForObstacle, 
//                 m_radius, m_speed, new RVO.Vector2(0, 0));

            int radiusI = (int)radius;
            for (int i = -radiusI; i <= radiusI; ++i )
            {
                for (int j = -radiusI; j <= radiusI; ++j )
                {
                    Vector2i iter = new Vector2i(i, j);
                    if(iter.magnitude() < radius)
                    {
                        m_outlineOffsetList.Add(iter);

                        m_outlineList.Add(iter);
                    }
                }
            }
            RegisterPos();
        }

        public void MoveTo(Common.Vector2 pos, System.Action arriveCallback = null, System.Action blockCallback = null)
        {
            m_state = State.Moving;
            m_arriveCallback = arriveCallback;
            m_blockCallback = blockCallback;
            m_desPos = pos;

            Common.Vector2 curPos = GetCurPos();
            Vector2i srcPos = new Vector2i((int)curPos.x, (int)curPos.y);
            Vector2i desPos = new Vector2i((int)m_desPos.x, (int)m_desPos.y);
            m_pathList.Clear();
            GridPathResult result = m_mgr.GetPathFinder().GridAstarFind(this, srcPos, desPos);
            //if(result != null)
            {
                if(result.m_result == FindPathResult.OnTheWay)
                {
                    m_pathList = result.m_pathList;
                    //CheckPathPoint();
                    UnRegisterPos();
                    m_gridPos = m_pathList[0];
                    RegisterPos();
                    return;
                }
                else if(result.m_result == FindPathResult.Arrived)
                {
                    StopAgent(m_arriveCallback);
                    return;
                }
                else if(result.m_result == FindPathResult.Blocked)
                {
                    StopAgent(m_blockCallback);
                    return;
                }
            }
        }

        public void Update(float deltaTime)
        {
            if(m_state == State.Moving)
            {
                if(reachedGoal())
                {
                    StopAgent(m_arriveCallback);
                    return;
                }
                else 
                {
//                     if(m_pathList.Count == 0)
//                     {
//                         Common.Vector2 curPos = GetCurPos();
//                         Vector2i srcPos = new Vector2i((int)curPos.x, (int)curPos.y);
//                         Vector2i desPos = new Vector2i((int)m_desPos.x, (int)m_desPos.y);
//                         GridPathResult result = m_mgr.GetPathFinder().GridAstarFind(this, srcPos, desPos);
//                         //if(result != null)
//                         if(result.m_result == FindPathResult.OnTheWay)
//                         {
//                             m_pathList = result.m_pathList;
//                             MoveToPathPoint();
//                             return;
//                         }
//                         else if (result.m_result == FindPathResult.Arrived)
//                         {
//                             StopAgent(m_arriveCallback);
//                             return;
//                         }
//                         else if(result.m_result == FindPathResult.Blocked)
//                         {
//                             StopAgent(m_blockCallback);
//                             return;
//                         }
//                     }
//                     else
//                     {
                        MoveToPathPoint(deltaTime);
/*                    }*/
                }
            }

            //RegisterPos();
        }

        void MoveToPathPoint(float deltaTime)
        {
            Vector2i pathPoint = m_pathList[0];
            Common.Vector2 distance = new Common.Vector2(pathPoint.m_x, pathPoint.m_y) - m_curPos;
            if (distance.sqrMagnitude < 0.1f)
            {
                m_pathList.RemoveAt(0);
                if (m_pathList.Count == 0)
                {
                    Common.Vector2 curPos = GetCurPos();
                    Vector2i srcPos = new Vector2i((int)curPos.x, (int)curPos.y);
                    Vector2i desPos = new Vector2i((int)m_desPos.x, (int)m_desPos.y);
                    GridPathResult result = m_mgr.GetPathFinder().GridAstarFind(this, srcPos, desPos);
                    //if(result != null)
                    if (result.m_result == FindPathResult.OnTheWay)
                    {
                        m_pathList = result.m_pathList;
                        //Debug.Log("MoveToPathPoint " + pathPoint);
                        //MoveToPathPoint();
                        UnRegisterPos();
                        m_gridPos = m_pathList[0];
                        RegisterPos();

                        return;
                    }
                    else if (result.m_result == FindPathResult.Arrived)
                    {
                        StopAgent(m_arriveCallback);
                        return;
                    }
                    else if (result.m_result == FindPathResult.Blocked)
                    {
                        StopAgent(m_blockCallback);
                        return;
                    }
                }
                else
                {
                    Vector2i pathPoint2 = m_pathList[0];
                    Common.Vector2 distance2 = new Common.Vector2(pathPoint2.m_x, pathPoint2.m_y) - m_curPos;
                    distance2.Normalize();
                    m_curPos += (deltaTime * m_speed) * distance2;
                }
            }
            else
            {
                distance.Normalize();
                m_curPos += (deltaTime * m_speed) * distance;
            }
        }

        void StopAgent(System.Action callback)
        {
            m_state = State.Stopping;
            //RVO.Simulator.Instance.setAgentPrefVelocity(m_RVOid, new RVO.Vector2(0, 0));
            if(callback != null)
            {
                callback();
            }
        }

        void UnRegisterPos()
        {
            for (int i = 0; i < m_outlineOffsetList.Count; ++i)
            {
                m_outlineList[i] = m_gridPos + m_outlineOffsetList[i];
            }
            m_mgr.GetMapMgr().UnRegisterPosList(m_outlineList, m_agentId);
        }

        void RegisterPos()
        {
            for (int i = 0; i < m_outlineOffsetList.Count; ++i )
            {
                m_outlineList[i] = m_gridPos + m_outlineOffsetList[i];
            }
            m_mgr.GetMapMgr().RegisetrPosList(m_outlineList, m_agentId);
        }

        bool reachedGoal()
        {
            /* Check if all agents have reached their goals. */
//             RVO.Vector2 pos = RVO.Simulator.Instance.getAgentPosition(m_RVOid);
//             RVO.Vector2 des = new RVO.Vector2(m_desPos.x, m_desPos.y);
//             if (RVO.RVOMath.absSq(pos - des) < 0.1f * 0.1f)
//             {
//                 return true;
//             }
// 
//             return false;
            Common.Vector2 distance = m_desPos - m_curPos;
            if (distance.sqrMagnitude < 0.1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Common.Vector2 GetCurPos()
        {
            //RVO.Vector2 pos = RVO.Simulator.Instance.getAgentPosition(m_RVOid);
            //return new Common.Vector2(pos.x_, pos.y_);
            return m_curPos;
        }
    }
}

