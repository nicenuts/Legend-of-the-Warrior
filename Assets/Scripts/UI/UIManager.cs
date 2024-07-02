using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStateBar playerStateBar;

    [Header("事件监听")]
    public CharacterEventSO healthAndPowerEvent;
    public SceneLoadEventSO unLoadedSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO backToMenuEvent;
    public FloatEventSO syncVolumeEvent;

    [Header("事件广播")]
    public VoidEventSO pauseEvent;


    [Header("组件")]
    public GameObject gameOverPanel;
    public GameObject restartBtn;
    public Button settingsBtn;
    public GameObject pausePanel;
    public Slider volumeSlider;

    private void Awake()
    {
        settingsBtn.onClick.AddListener(TogglePausePanel);     //添加监听事件
    }


    private void OnEnable()   //注册事件
    {
        healthAndPowerEvent.OnEventRaised += OnhealthEvent;     
        healthAndPowerEvent.OnEventRaised += OnPowerEvent;
        unLoadedSceneEvent.LoadRequestEvent += OnUnLoadedSceneEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
    }

    private void OnDisable()    //取消注册  固定写法
    {
        healthAndPowerEvent.OnEventRaised -= OnhealthEvent;  
        healthAndPowerEvent.OnEventRaised -= OnPowerEvent;
        unLoadedSceneEvent.LoadRequestEvent -= OnUnLoadedSceneEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
    }

    private void OnSyncVolumeEvent(float amount)
    {
        volumeSlider.value = (amount+80) / 100;
    }

    private void TogglePausePanel()
    {
        if (pausePanel.activeInHierarchy)   //如果pausePanel是激活的
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;    //游戏正常运行，非暂停状态
        }
        else
        {
            pauseEvent.RaiseEvent();   
            pausePanel.SetActive(true);
            Time.timeScale = 0;   //游戏暂停，固定写法 
        }
    }


    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);  //开启人物死亡时的UI面板
        EventSystem.current.SetSelectedGameObject(restartBtn);  //选择重新开始按钮
    }

    private void OnLoadDataEvent()
    {
        gameOverPanel.SetActive(false);  //关闭人物死亡时的UI面板
    }

    private void OnhealthEvent(Character character)
    {
        var persentageHealth = character.currentHealth / character.maxHealth;
        playerStateBar.OnHealthChange(persentageHealth);    //血条变化
    }
    private void OnPowerEvent(Character character)   
    {
        var persentagePower = character.currentPower / character.maxPower ;
        playerStateBar.OnPowerChange(character);      //能量条变化
    }

    private void OnUnLoadedSceneEvent(GameSceneSO sceneToLoad, Vector3 arg1, bool arg2)
    {
        if (sceneToLoad.sceneType == SceneType.Menu)
        {
            playerStateBar.gameObject.SetActive(false);   //如果是菜单场景就关闭血条显示
        }
        else
        {
            playerStateBar.gameObject.SetActive(true);
        }

    }
}


