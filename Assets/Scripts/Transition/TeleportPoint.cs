using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, Iinteractable
{
    public SceneLoadEventSO LoadEventSO;
    public GameSceneSO sceneToGo;   //角色将要去的场景
    public Vector3 PositionToGo;    

    public void TriggerAction()
    {
        Debug.Log("传送！");
        
        LoadEventSO.RaiseLoadRequestEvent(sceneToGo,PositionToGo,true);   //事件响应
        
    }
}

