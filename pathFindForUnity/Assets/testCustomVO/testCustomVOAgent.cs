using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ThroughPointTangentWithCircleResult
{
    public Vector3 m_p1;
    public Vector3 m_p2;
}

public class ThroughPointTangentWithCircle
{
    public static ThroughPointTangentWithCircleResult Calculate(Vector3 pos, float radius, Vector3 point)
    {
        ThroughPointTangentWithCircleResult ret = new ThroughPointTangentWithCircleResult();
        // find tangents
        float dx = pos.x - point.x;
        float dz = pos.z - point.z;
        float dd = Mathf.Sqrt(dx * dx + dz * dz);
        float a = Mathf.Asin(radius / dd);
        float b = Mathf.Atan2(dz, dx);

        float t = b - a;
        ret.m_p1 = new Vector3(radius * Mathf.Sin(t), 0, radius * -Mathf.Cos(t) );

        t = b + a;
        ret.m_p2 = new Vector3(radius * -Mathf.Sin(t), 0, radius * Mathf.Cos(t) );

        return ret;
    }
}

public class testCustomVOAgent : MonoBehaviour
{
    public Vector3 m_desPos = new Vector3(0, 0, 10);

    public Vector3 m_bPos = new Vector3();
    public float m_bRadius = 0.5f;
    public float m_aRadius = 0.5f;
    public float m_speed = 1;

    void Awake()
    {
        transform.position = new Vector3(0, 0, -10);
    }

    void Update()
    {
        Vector3 wantDir = (m_desPos - transform.position).normalized;
        Vector3 obstacleDir = (m_bPos - transform.position).normalized;

//         float r = m_bRadius + m_aRadius;
//         float distance = (m_bPos - transform.position).magnitude;
//         float sinHalf = r / distance;
//         float cosHalf = Mathf.Sqrt(1 - sinHalf * sinHalf);
//         float cosTheta = Vector3.Dot(wantDir, obstacleDir);
//         if (cosTheta >= 0 && cosTheta < cosHalf)
//         {
//             // 处在夹角内
//             Vector3 edge1 = (m_desPos - transform.position) - ;
//         }
        ThroughPointTangentWithCircleResult result = ThroughPointTangentWithCircle.Calculate(m_bPos, m_bRadius, transform.position);
        float cosHalf = Vector3.Dot(obstacleDir, (result.m_p1 - transform.position).normalized);
        float cosTheta = Vector3.Dot(wantDir, obstacleDir);

        Vector3 finalSpeed = Vector3.zero;
        if (cosTheta >= 0 && cosHalf < cosTheta)
        {
            // 处在夹角内
            float param1 = Vector3.Dot(wantDir * m_speed, (result.m_p1 - transform.position).normalized);
            float param2 = Vector3.Dot(wantDir * m_speed, (result.m_p2 - transform.position).normalized);
            if(param1 >= param2)
            {
                finalSpeed = (result.m_p1 - transform.position).normalized * param1;
            }
            else
            {
                finalSpeed = (result.m_p2 - transform.position).normalized * param2;
            }
        }
        else
        {
            finalSpeed = wantDir * m_speed;
        }
        transform.position += finalSpeed * Time.deltaTime;
    }
}

