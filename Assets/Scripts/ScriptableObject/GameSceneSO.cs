using UnityEngine;
using UnityEngine.AddressableAssets;

/*
    GameSceneSO：Addressable场景信息:ScriptableObject
*/

[CreateAssetMenu(menuName = "Game Scene/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    public SceneType sceneType; 
    public AssetReference sceneReference; //资源文件引用 
}
