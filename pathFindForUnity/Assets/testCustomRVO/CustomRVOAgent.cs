using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct AvoidRequest
{
    public AvoidRequest(Vector3 dir)
    {
        m_dir = dir;
        m_dir.Normalize();
    }
    public Vector3 m_dir;
}

public enum MoveState
{
    None,
    Moving,
    Stoping,
}

public class CustomRVOAgent : MonoBehaviour
{
    CustomRVOMgr m_mgr = null;
    public int m_id = 0;
    public MoveState m_moveState = MoveState.Stoping;
    public float m_speed = 1;
    public Queue<AvoidRequest> m_avoidQueue = new Queue<AvoidRequest>();

    public Vector3 m_desPos = Vector3.zero;

    public void Init(int id, Vector3 pos, CustomRVOMgr mgr)
    {
        m_id = id;
        transform.position = pos;
        m_mgr = mgr;
    }

    public void Move(Vector3 pos)
    {
        m_moveState = MoveState.Moving;
        m_desPos = pos;
    }

	public void Tick(float delta)
    {
        if (m_moveState == MoveState.Moving)
        {
            // 移动中不接受avoid请求
            m_avoidQueue.Clear();

            Vector3 distance = m_desPos - transform.position;
            if(distance.magnitude < 0.2f)
            {
                m_moveState = MoveState.Stoping;
                return;
            }
            else
            {
                Vector3 dir = distance.normalized;
                Vector3 wantPos = transform.position + dir * m_speed * delta;
                bool hasCollision = false;
                foreach (var iter in m_mgr.m_agentDic)
                {
                    if(iter.Value == this)
                    {
                        continue;
                    }
                    Vector3 rvoDistance = transform.position - iter.Value.transform.position;
                    if(rvoDistance.magnitude < 1)
                    {
                        hasCollision = true;
                        Vector3 avoidAgentDir = iter.Value.transform.position - transform.position;
                        Vector3 avoidDir = Vector3.zero;
                        if(Vector3.Cross(dir, avoidAgentDir).y >= 0)
                        {
                            // 在前进方向的右边
                            avoidDir = Vector3.Cross(Vector3.up, dir);
                        }
                        else
                        {
                            // 在前进方向的左边
                            avoidDir = Vector3.Cross(dir, avoidDir);
                        }
                        AvoidRequest avoidRequest = new AvoidRequest(avoidDir);
                        iter.Value.m_avoidQueue.Enqueue(avoidRequest);
                    }
                }
                if(!hasCollision)
                {
                    transform.position = wantPos;
                }
            }
        }
        else if (m_moveState == MoveState.Stoping)
        {
            while (m_avoidQueue.Count != 0)
            {
                AvoidRequest avoidCmd = m_avoidQueue.Dequeue();
                Vector3 avoidDeltaPos = avoidCmd.m_dir * m_speed * delta;
                transform.position += avoidDeltaPos;
            }
        }
    }
}
