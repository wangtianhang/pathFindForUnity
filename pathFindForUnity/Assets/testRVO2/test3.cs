using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class test3 : MonoBehaviour 
{
    List<RVO.Vector2> m_goals = new List<RVO.Vector2>();
    float m_accTimer = 0.1f;
    float m_step = 0.1f;

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 30;

        setupScenario();
    }

    // Update is called once per frame
    void Update()
    {
        m_accTimer += Time.deltaTime;
        // 考虑大delta的情况
        if (m_accTimer >= m_step)
        {
            m_accTimer -= m_step;

            updateVisualization();

            setPreferredVelocities();

            RVO.Simulator.Instance.doStep();
        }

        if (reachedGoal())
        {
            gameObject.SetActive(false);
        }
    }

    void setupScenario()
    {
        /* Specify the global time step of the simulation. */
        RVO.Simulator.Instance.setTimeStep(m_step);

        /*
         * Specify the default parameters for agents that are subsequently
         * added.
         */
        RVO.Simulator.Instance.setAgentDefaults(15.0f, 10, 10.0f, 10.0f, 1.5f, 2.0f, new RVO.Vector2(0.0f, 0.0f));

        /*
         * Add agents, specifying their start position, and store their
         * goals on the opposite side of the environment.
         */


        //         for (int i = 0; i < 250; ++i)
        //         {
        //             RVO.Simulator.Instance.addAgent(200.0f *
        //                 new RVO.Vector2((float)Math.Cos(i * 2.0f * Math.PI / 250.0f),
        //                     (float)Math.Sin(i * 2.0f * Math.PI / 250.0f)));
        //             m_goals.Add(-RVO.Simulator.Instance.getAgentPosition(i));
        // 
        //             GameObject go = GameObject.Instantiate(prefab) as GameObject;
        //             go.name = "agent" + i;
        //             Agent agent = go.AddComponent<Agent>();
        //             agent.Init(i);
        //         }

        

        SetAgent0();

        SetWall();

    }

    void SetWall()
    {
        GameObject prefab = Resources.Load("testCustomRVOAgent") as GameObject;

        for (int i = -30; i <= 30; i++)
        {
            for (int j = -1; j <= 1; ++j)
            {
                int id = RVO.Simulator.Instance.addAgent(new RVO.Vector2(i, j), 10, 10, 10, 10, 0.5f, 1, new RVO.Vector2(0, 0));

                GameObject go = GameObject.Instantiate(prefab) as GameObject;
                go.name = "agent " + id;
                Agent1 agent = go.AddComponent<Agent1>();
                agent.Init(id);
            }
        }

    }

    void SetAgent0()
    {
        GameObject prefab = Resources.Load("testCustomRVOAgent") as GameObject;
        {
            RVO.Simulator.Instance.addAgent(new RVO.Vector2(0, -10), 10, 10, 10, 10, 0.5f, 1, new RVO.Vector2(0, 0));
            m_goals.Add(new RVO.Vector2(0, 10));

            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = "agent" + 0;
            Agent1 agent = go.AddComponent<Agent1>();
            agent.Init(0);
        }
    }

    void updateVisualization()
    {

    }

    void setPreferredVelocities()
    {
        /*
         * Set the preferred velocity to be a vector of unit magnitude
         * (speed) in the direction of the goal.
         */
        for (int i = 0; i < m_goals.Count; ++i)
        {
            RVO.Vector2 goalVector = m_goals[i] - RVO.Simulator.Instance.getAgentPosition(i);

            if (RVO.RVOMath.absSq(goalVector) > 1.0f)
            {
                goalVector = RVO.RVOMath.normalize(goalVector);
            }

            RVO.Simulator.Instance.setAgentPrefVelocity(i, goalVector);
        }
    }

    bool reachedGoal()
    {
        /* Check if all agents have reached their goals. */
        for (int i = 0; i < m_goals.Count; ++i)
        {
            if (RVO.RVOMath.absSq(RVO.Simulator.Instance.getAgentPosition(i) - m_goals[i]) > RVO.Simulator.Instance.getAgentRadius(i) * RVO.Simulator.Instance.getAgentRadius(i))
            {
                return false;
            }
        }

        return true;
    }
}

