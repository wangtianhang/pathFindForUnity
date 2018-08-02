using UnityEngine;
using System.Collections;

public class CustomRVOAgent : MonoBehaviour
{
    public int m_id = 0;
    public void Init(int id, Vector3 pos)
    {
        m_id = id;
        transform.position = pos;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
