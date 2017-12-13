using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFind
{
    /// <summary>
    /// pathfindmgr统计cpu时间，用的太多的话就等到下一帧再计算
    /// </summary>
    public class PathFindMgr
    {
        static PathFindMgr _instance = null;
        public static PathFindMgr Instance
        {
            get  
            {
                if(_instance == null)
                {
                    _instance = new PathFindMgr();
                }
                return _instance;
            }
        }

        float m_oneStep = 0.1f;
        float m_accTimer = 0;

        public static float s_maxUnitSize = 5;
        public static float s_maxNeighborDistanceTakeInCount = 5 * 5;
        public static int s_maxNeighborTakeInCount = 10;
        public static float s_pushParamForUnit = 1;
        public static float s_pushParamForObstacle = 1;

        int m_agendIdGenerator = 0;
        Dictionary<int, PathFindAgent> m_agentDic = new Dictionary<int, PathFindAgent>();

        AstarMapMgr m_mapMgr = null;
        AstarPathFinder m_pathFinder = null;

        public void Init(float oneStep, float maxUnitSize)
        {
            m_oneStep = oneStep;

            s_maxUnitSize = maxUnitSize;
            s_maxNeighborDistanceTakeInCount = s_maxUnitSize * 10;

            /* Specify the global time step of the simulation. */
            RVO.Simulator.Instance.setTimeStep(m_oneStep);

            m_mapMgr = new AstarMapMgr();

            m_pathFinder = new AstarPathFinder();
        }

        public void LoadMap()
        {
            m_mapMgr.Init(100, 100);

            m_pathFinder.Init(1000, m_mapMgr);

            // todo 初始化astar地图碰撞
            m_agendIdGenerator += 1;
            UnityEngine.GameObject obstacleGo = UnityEngine.GameObject.Find("Obstacle");
            UnityEngine.GameObject obstaclePrefab = UnityEngine.Resources.Load("Barrier") as UnityEngine.GameObject;
            Vector2i centerI = new Vector2i((int)obstacleGo.transform.position.x, (int)obstacleGo.transform.position.z);
            int lengh = (int)obstacleGo.transform.localScale.x;
            int width = (int)obstacleGo.transform.localScale.z;
            List<Vector2i> obstacleGridList = new List<Vector2i>();
            for (int i = -lengh / 2; i < lengh / 2; ++i )
            {
                for (int j = -width / 2; j < width / 2; ++j )
                {
                    Vector2i iter = centerI + new Vector2i(i, j);
                    UnityEngine.GameObject.Instantiate(obstaclePrefab, new UnityEngine.Vector3(iter.m_x, -1, iter.m_y), UnityEngine.Quaternion.identity);
                    obstacleGridList.Add(iter);
                }
            }
            m_mapMgr.RegisetrPosList(obstacleGridList, m_agendIdGenerator);
            // todo 初始化rvo地图碰撞
            // 格子寻路都为整数格，所以沟边尽量以整数中心计算占用格子数
            RVO.Vector2 center = new RVO.Vector2(obstacleGo.transform.position.x, obstacleGo.transform.position.z);
            RVO.Vector2 scale = new RVO.Vector2(obstacleGo.transform.localScale.x, obstacleGo.transform.localScale.z);
            RVO.Vector2 point1 = center + new RVO.Vector2(-scale.x_ / 2, +scale.y_ / 2);
            RVO.Vector2 point2 = center + new RVO.Vector2(+scale.x_ / 2, +scale.y_ / 2);
            RVO.Vector2 point3 = center + new RVO.Vector2(+scale.x_ / 2, -scale.y_ / 2);
            RVO.Vector2 point4 = center + new RVO.Vector2(-scale.x_ / 2, -scale.y_ / 2);
            List<RVO.Vector2> obstacleList = new List<RVO.Vector2>();
            obstacleList.Add(point1);
            obstacleList.Add(point2);
            obstacleList.Add(point3);
            obstacleList.Add(point4);
            RVO.Simulator.Instance.addObstacle(obstacleList);

            RVO.Simulator.Instance.processObstacles();
        }

        public AstarMapMgr GetMapMgr()
        {
            return m_mapMgr;
        }

        public AstarPathFinder GetPathFinder()
        {
            return m_pathFinder;
        }

        public void Update(float deltaTime)
        {
            m_accTimer += deltaTime;
            if(m_accTimer >= m_oneStep)
            {
                m_accTimer -= m_oneStep;

                RVO.Simulator.Instance.doStep();

                foreach(var iter in m_agentDic)
                {
                    iter.Value.Update(deltaTime);
                }
            }
        }

        public int AddAgent(Common.Vector2 pos, float radius, float moveSpeed)
        {
            m_agendIdGenerator += 1;
            PathFindAgent agent = new PathFindAgent();
            agent.Init(m_agendIdGenerator, this, pos, radius, moveSpeed);
            m_agentDic.Add(m_agendIdGenerator, agent);
            return m_agendIdGenerator;
        }

        public void MoveTo(int agentId, Common.Vector2 pos, System.Action arriveCallback = null, System.Action blockCallback = null)
        {
            PathFindAgent agent = null;
            if (m_agentDic.TryGetValue(agentId, out agent))
            {
                agent.MoveTo(pos, arriveCallback, blockCallback);
            }
        }

        public Common.Vector2 GetCurPos(int agentId)
        {
            PathFindAgent agent = null;
            if (m_agentDic.TryGetValue(agentId, out agent))
            {
                return agent.GetCurPos();
            }
            else
            {
                return new Common.Vector2(0, 0);
            }
            
        }
    }
}


