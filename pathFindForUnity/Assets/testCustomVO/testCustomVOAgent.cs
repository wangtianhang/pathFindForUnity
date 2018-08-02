using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class testCustomVOAgent : MonoBehaviour
{
    public Vector3 m_desPos = new Vector3(0, 0, 10);

    public Vector3 m_bPos = new Vector3();
    public float m_bRadius = 0.5f;
    public float m_aRadius = 0.5f;

    void Awake()
    {
        transform.position = new Vector3(0, 0, -10);
    }
}

