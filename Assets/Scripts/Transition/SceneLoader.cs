using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour,ISaveable
{
    public Transform playerTrans;   //玩家坐标
    public Vector3 firstPosition;   //玩家初始坐标
    public Vector3 meauPosition;   //菜单界面人物的坐标
    private Vector3 posotionToGo;
    private bool fadeScreen;   //是否渐入渐出
    private bool isLoading;  //场景是否正在加载
    public float fadeDuration;  //渐入渐出的等待时间

    [Header("事件监听")]
    public SceneLoadEventSO LoadEventSO;   //场景加载事件
    public VoidEventSO newGameEvent;   
    public VoidEventSO backToEvent;
    

    [Header("广播")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent; 
    public SceneLoadEventSO unLoadedSceneEvent;


    [Header("场景")]
    public GameSceneSO firstLoadScene;  //第一个加载的场景
    public GameSceneSO meauScene;   //菜单场景
    public GameSceneSO currentLoadedScene;  //当前加载场景
    private GameSceneSO sceneToLoad;

    

    private void Awake()
    {
        //Addressables.LoadSceneAsync(firstLoadScene.sceneReference, LoadSceneMode.Additive);
        // currentLoadedScene = firstLoadScene;
        // currentLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);

    }

    private void Start()
    {
        //NewGame();
        LoadEventSO.RaiseLoadRequestEvent(meauScene, meauPosition, true);  //加载菜单场景
    }

    private void OnEnable()
    {
        LoadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;
        backToEvent.OnEventRaised += OnBackToEvent;

        ISaveable saveable = this;
        saveable.RegisterSaveData();   //注册
    }
    private void OnDisable()
    {
        LoadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;
        backToEvent.OnEventRaised -= OnBackToEvent;

        ISaveable saveable = this;
        saveable.UnregisterSaveData();   //注销
    }

    private void OnBackToEvent()
    {
        sceneToLoad = meauScene;
        LoadEventSO.RaiseLoadRequestEvent(sceneToLoad,meauPosition,true);   //退回到菜单场景
    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        //OnLoadRequestEvent(sceneToLoad, firstPosition, true);
        LoadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstPosition, true);  //加载游戏场景
    }



    //去往新场景的请求
    /// <summary>
    /// 场景加载事件请求
    /// </summary>
    /// <param name="locationToLoad"></param>
    /// <param name="posToGo"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading)
        {
            return;
        }

        isLoading = true;
        sceneToLoad = locationToLoad;
        posotionToGo = posToGo;
        this.fadeScreen = fadeScreen;

        Debug.Log("新场景！！！");


        //卸载场景需要时间计算，用协程的方法实现
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }

    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            //TODO：实现渐入渐出: 场景逐渐变黑
            fadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);

        //广播事件 调整血条显示
        unLoadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad,posotionToGo,true);

        currentLoadedScene.sceneReference.UnLoadScene();  //卸载场景


        playerTrans.gameObject.SetActive(false);  //关闭人物
        LoadNewScene();         //加载新场景
    }

    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);  //加载场景
        loadingOption.Completed += OnLoadCompleted;   //场景加载好之后执行的函数
    }

    /// <summary>
    /// 场景加载完成后
    /// </summary>
    /// <param name="handle"></param>
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        currentLoadedScene = sceneToLoad;    //当前场景赋值

        playerTrans.gameObject.SetActive(true);  //启动人物
        playerTrans.position = posotionToGo;  //移动玩家坐标（在场景切换时）
        if (fadeScreen)
        {
            //场景逐渐变透明
            fadeEvent.FadeOut(fadeDuration);
        }

        isLoading = false;

        if(currentLoadedScene.sceneType != SceneType.Menu)   //如果当前场景不是菜单
            afterSceneLoadedEvent.RaiseEvent();  //场景加载完成后事件 如：获取新的摄像机的边界,更新人物和敌人的血量
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();   
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadedScene);
    }

    public void LoadData(Data data)
    {
        var playerID = playerTrans.GetComponent<DataDefination>().ID;
        if(data.characterPosDict.ContainsKey(playerID))
        {
            posotionToGo = data.characterPosDict[playerID];
            sceneToLoad = data.GetSavedScene();

            OnLoadRequestEvent(sceneToLoad, posotionToGo, true);
        }
    }
}
