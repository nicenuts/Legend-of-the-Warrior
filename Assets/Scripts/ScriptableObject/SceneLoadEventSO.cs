using UnityEngine;
using UnityEngine.Events;

/*
    SceneLoadEventSO：传递场景加载参数的事件
    此文件作用
    传递参数：人物下一个去的场景，场景坐标，屏幕是否有渐隐渐出效果

*/ 
[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO,Vector3,bool> LoadRequestEvent;
    

    /// <summary>
    /// 场景加载请求
    /// </summary>
    /// <param name="locationToLoad"> 要加载的场景 </param>
    /// <param name="posToGo"> Player的目的坐标 </param>
    /// <param name="fadeScreen"> 是否渐入渐出 </param>
    public void RaiseLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        LoadRequestEvent?.Invoke(locationToLoad, posToGo,fadeScreen);
    }

}
