using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

class Agent1 : MonoBehaviour
{
    int m_agentId = 0;

    public void Init(int agentId)
    {
        m_agentId = agentId;
    }

    void Update()
    {
        RVO.Vector2 pos = RVO.Simulator.Instance.getAgentPosition(m_agentId);
        transform.position = new Vector3(pos.x_, 0, pos.y_);
    }
}

