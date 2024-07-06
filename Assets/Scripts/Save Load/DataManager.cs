using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]   //保证Data Manager可以优先其他代码执行

public class DataManager : MonoBehaviour
{
    public static DataManager instance;   //基本写法

    [Header("事件监听")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;

    private List<ISaveable> saveableList = new List<ISaveable>();

    private Data saveData;

    private void Awake()
    {
        if (instance == null) //单例模式，保证类的唯一实例性，并提供一个全局访问点
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        saveData = new Data();
    }

    private void OnEnable()
    {
        saveDataEvent.OnEventRaised += save;
        loadDataEvent.OnEventRaised += Load;
    }

    private void OnDisable()
    {
        saveDataEvent.OnEventRaised -= save;
        loadDataEvent.OnEventRaised -= Load;
    }

    private void Update()
    {
        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            Load();
        }
    }

    public void RegisterSaveData(ISaveable saveable)
    {
        if (!saveableList.Contains(saveable))    //列表中是否包含当前saveable
        {
            saveableList.Add(saveable);
        }
    }

    public void UnRegisterLoadData(ISaveable saveable)
    {
        saveableList.Remove(saveable);   //
    }

    public void save()
    {
        foreach (var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }

        foreach (var item in saveData.characterPosDict)
        {
            Debug.Log(item.Key + "     " + item.Value);
        }


    }

    public void Load()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }

}
