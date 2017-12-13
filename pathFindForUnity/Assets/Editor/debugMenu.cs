using UnityEngine;
using System.Collections;
using UnityEditor;

public class debugMenu  {

    [MenuItem("debug/showColliderData")]
    static void showColliderData()
    {
        PathFind.PathFindMgr.Instance.GetMapMgr().ShowColliderData();
    }
}
