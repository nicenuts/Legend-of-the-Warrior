using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public string sceneToSave;
    public Dictionary<string, Vector3>  characterPosDict = new Dictionary<string, Vector3>();
    public Dictionary<string,float> floatSavedData = new Dictionary<string,float>();  //血量和能量条都存储在这个列表中

    public void SaveGameScene(GameSceneSO saveScene)
    {
        sceneToSave = JsonUtility.ToJson(saveScene);  //把Object类型变成string类型，Json字符串
        Debug.Log("保存场景:" + sceneToSave);
    }
    public GameSceneSO GetSavedScene()
    {
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();  //创建一个GameSceneSO实例
        JsonUtility.FromJsonOverwrite(sceneToSave, newScene);    //反序列化,将JSON字符串转换成对象
        Debug.Log("加载场景");
        
        return newScene;    
    }

}