using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class testGridPathFinder1 : MonoBehaviour
{

    List<Unit> m_unitList = new List<Unit>();
    public const float OneFrameSpan = 1 / (float)10;
    float m_remainRealTime = 0;
	// Use this for initialization
	void Start () 
    {
        Application.targetFrameRate = 30;

        PathFind.PathFindMgr.Instance.Init(OneFrameSpan, 5);

        PathFind.PathFindMgr.Instance.LoadMap();

        InitUnit();
	}

    void InitUnit()
    {
        {
            float radius = 3;
            GameObject prefab = Resources.Load("agent3") as GameObject;
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.transform.localScale = new Vector3(radius, radius, radius);
            go.name = "unit" + 1;
            Unit unit = go.AddComponent<Unit>();
            unit.Init(1, new Vector3(40, 0, 45), radius, 5);
            unit.MoveTo(new Vector3(60, 0, 55));
            m_unitList.Add(unit);
        }

        {
            float radius = 2;
            GameObject prefab = Resources.Load("agent3") as GameObject;
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.transform.localScale = new Vector3(radius, radius, radius);
            go.name = "unit" + 2;
            Unit unit = go.AddComponent<Unit>();
            unit.Init(2, new Vector3(50, 0, 45), radius, 5);
            unit.MoveTo(new Vector3(50, 0, 55));
            m_unitList.Add(unit);
        }

        {
            float radius = 1;
            GameObject prefab = Resources.Load("agent3") as GameObject;
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.transform.localScale = new Vector3(radius, radius, radius);
            go.name = "unit" + 3;
            Unit unit = go.AddComponent<Unit>();
            unit.Init(3, new Vector3(60, 0, 45), radius, 5);
            unit.MoveTo(new Vector3(40, 0, 55));
            m_unitList.Add(unit);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        m_remainRealTime += Time.deltaTime;
        while(m_remainRealTime >= OneFrameSpan)
        {
            m_remainRealTime -= OneFrameSpan;

            PathFind.PathFindMgr.Instance.Update(OneFrameSpan);

            foreach (var iter in m_unitList)
            {
                iter.OnLogicUpdate(OneFrameSpan);
            }
        }

        foreach(var iter in m_unitList)
        {
            iter.OnRenderUpdate(Time.deltaTime);
        }
	}
}
