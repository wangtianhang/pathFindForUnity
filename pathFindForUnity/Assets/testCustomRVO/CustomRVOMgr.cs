using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CustomRVOMgr : MonoBehaviour
{
    float m_accTime = 0;
    float m_tickSpan = 0.1f;

    int m_idGenerator = 1;

    Dictionary<int, CustomRVOAgent> m_agentDic = new Dictionary<int, CustomRVOAgent>();

    public void Init()
    {

    }

    public int AddAgent(Vector3 pos)
    {
        GameObject prefab = Resources.Load("testCustomRVOAgent") as GameObject;
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        CustomRVOAgent agent = go.AddComponent<CustomRVOAgent>();
        agent.Init(m_idGenerator, pos);
        m_idGenerator += 1;
        m_agentDic.Add(agent.m_id, agent);
        return agent.m_id;
    }

    public void MoveAgent(int agentId, Vector3 pos)
    {

    }

    void Update()
    {
        m_accTime += Time.deltaTime;
        if (m_accTime >= m_tickSpan)
        {
            m_accTime -= m_tickSpan;
            Tick();
        }
    }

    void Tick()
    {

    }
}

