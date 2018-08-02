using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class testCustomRVO : MonoBehaviour
{
    CustomRVOMgr m_customRVOMgr = null;

    void Start()
    {
        m_customRVOMgr = new GameObject("customRVOMgr").AddComponent<CustomRVOMgr>();
        m_customRVOMgr.Init();

        for (int i = -30; i <= 30; i++ )
        {
            for (int j = -1; j <= 1; ++j )
            {
                m_customRVOMgr.AddAgent(new Vector3(i, 0, j));
            }
        }

        int id = m_customRVOMgr.AddAgent(new Vector3(0, 0, -5));
        m_customRVOMgr.MoveAgent(id, new Vector3(0, 0, 5));
    }
}

