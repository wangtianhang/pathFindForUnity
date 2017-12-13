using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class test1 : MonoBehaviour {

    List<RVO.Vector2> m_goals = new List<RVO.Vector2>();
    float m_accTimer = 0.1f;
    float m_step = 0.1f;

	// Use this for initialization
	void Start () 
    {
        Application.targetFrameRate = 30;

        setupScenario();
	}

	// Update is called once per frame
	void Update () 
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

        if(reachedGoal())
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
        GameObject prefab = Resources.Load("agent") as GameObject;

        for (int i = 0; i < 250; ++i)
        {
            RVO.Simulator.Instance.addAgent(200.0f *
                new RVO.Vector2((float)Math.Cos(i * 2.0f * Math.PI / 250.0f),
                    (float)Math.Sin(i * 2.0f * Math.PI / 250.0f)));
            m_goals.Add(-RVO.Simulator.Instance.getAgentPosition(i));

            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = "agent" + i;
            Agent1 agent = go.AddComponent<Agent1>();
            agent.Init(i);
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
        for (int i = 0; i < RVO.Simulator.Instance.getNumAgents(); ++i)
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
        for (int i = 0; i < RVO.Simulator.Instance.getNumAgents(); ++i)
        {
            if (RVO.RVOMath.absSq(RVO.Simulator.Instance.getAgentPosition(i) - m_goals[i]) > RVO.Simulator.Instance.getAgentRadius(i) * RVO.Simulator.Instance.getAgentRadius(i))
            {
                return false;
            }
        }

        return true;
    }
}
