using UnityEngine;
using System.Collections;

/// <summary>
/// ai层统计寻路的时间，如果一段时间内（比如3秒）一直无法靠近目标位置，则认为路径阻塞或不可达，重新走决策逻辑
/// </summary>
public class Unit : MonoBehaviour 
{
    int m_unitId = 0;
    public Common.Vector2 m_lastLogicPos = new Common.Vector2(0, 0);
    float m_raidus = 0;
    float m_speed = 0;
    int m_moveAgentId = 0;

    Vector3 m_renderSrcPos = Vector3.zero;
    Vector3 m_renderDesPos = Vector3.zero;
    float m_renderPosAccTimer = 0;

    public void Init(int unitId, Vector3 pos, float radius, float speed)
    {
        m_unitId = unitId;

        m_lastLogicPos = new Common.Vector2(pos.x, pos.z);
        transform.position = pos;

        m_raidus = radius;
        m_speed = speed;

        m_moveAgentId = PathFind.PathFindMgr.Instance.AddAgent(new Common.Vector2(pos.x, pos.z), radius, m_speed);
    }

    public Vector3 GetLogicPos()
    {
        // 静止的unit也可能被别的unit挤走，所以需要从rvo模块获取位置
        Common.Vector2 logicPos = PathFind.PathFindMgr.Instance.GetCurPos(m_moveAgentId);
        return new Vector3(logicPos.x, 0, logicPos.y);
    }

    public void MoveTo(Vector3 pos)
    {
        PathFind.PathFindMgr.Instance.MoveTo(m_moveAgentId, new Common.Vector2(pos.x, pos.z), ArriveCallback, BlockCallback);
    }

    void ArriveCallback()
    {
        Debug.Log("ArriveCallback " + m_unitId + " " + PathFind.PathFindMgr.Instance.GetCurPos(m_moveAgentId));
    }

    void BlockCallback()
    {
        Debug.Log("BlockCallback " + m_unitId);
    }

    /// <summary>
    /// 需要稳定帧率
    /// </summary>
    /// <param name="delta"></param>
    public void OnLogicUpdate(float delta)
    {
        Common.Vector2 logicPos = PathFind.PathFindMgr.Instance.GetCurPos(m_moveAgentId);
        if(m_lastLogicPos != logicPos)
        {
            m_lastLogicPos = logicPos;

            m_renderSrcPos = gameObject.transform.position;
            m_renderDesPos = new Vector3(m_lastLogicPos.x, 0, m_lastLogicPos.y);
            m_renderPosAccTimer = 0;
        }
    }

    public void OnRenderUpdate(float delta)
    {
        m_renderPosAccTimer += delta;
        float weight = m_renderPosAccTimer / testPathFinder1.OneFrameSpan;
        Vector3 curRenderPos = Vector3.Lerp(m_renderSrcPos, m_renderDesPos, weight);
        gameObject.transform.position = curRenderPos;
    }

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        
	}
}
