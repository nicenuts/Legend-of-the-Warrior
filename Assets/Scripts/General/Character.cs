using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable
{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;

    [Header("基本属性")]
    public float maxHealth;  //最大生命值
    public float currentHealth;  //当前生命值
    public float maxPower;  //最大能量值
    public float currentPower;   //当前能量值
    public float powerRecoverSpeed;    //能量值恢复的速度
    public float sildePowerCost;  //滑铲消耗能量值

    [Header("受伤无敌")]
    public bool invulnerable;

    public float invulnerableDuration;
    public float invulnerableCounter;

    public UnityEvent<Character> OnHealthAndPowerChange;  //玩家血条和能量条变化的事件
    public UnityEvent<Transform> OnTakeDamage;   //受伤的事件
    public UnityEvent onDie;     //死亡的事件


    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        ISaveable saveable = this;    //因为在接口中写了函数的实现方法后，不会出现在Character中
        saveable.RegisterSaveData();   //所以这里强制执行   注册
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnregisterSaveData();  //注销
    }

    // Update is called once per frame
    void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
        }
        if (invulnerableCounter <= 0)
        {
            invulnerable = false;
        }

        if(currentPower < maxPower)   //能量条回复
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    public void TakeDmage(Attack attacker)
    {
        if (invulnerable)
        {
            return;
        }
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth = currentHealth - attacker.damage;
            //触发受伤,玩家碰到敌人，玩家受伤，但是敌人不受伤
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            //触发死亡
            onDie?.Invoke();
        }

        OnHealthAndPowerChange?.Invoke(this);  //触发受伤的血条变化

        TriggerInvulnerable();
    }

    public void OnSlidePowerCost(float sildePowerCost)  //滑动消耗能量
    {
        if(currentPower-sildePowerCost < 0)
        {
            return;
        }
        currentPower -= sildePowerCost;
        OnHealthAndPowerChange?.Invoke(this);  //能量条变化
    }
   
    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            if (currentHealth > 0)
            {
                currentHealth = 0;
                OnHealthAndPowerChange?.Invoke(this);   //跌落水中，死亡，更新血量
                onDie?.Invoke();
            }
        }
    }



    private void NewGame()
    {
        currentHealth = maxHealth;    //游戏启动时血条为最大的血量，能量值
        currentPower = maxPower;
        OnHealthAndPowerChange?.Invoke(this);   //更新UI
    }



    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();    //获得ID
    }

    public void GetSaveData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))  //如果ID存在
        {
            data.characterPosDict[GetDataID().ID] = transform.position;
            //更改ID对应的坐标
            data.floatSavedData[GetDataID().ID + "health"] = this.currentHealth;
            data.floatSavedData[GetDataID().ID + "power"] = this.currentPower;

        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, transform.position);
            //添加坐标   
            data.floatSavedData.Add(GetDataID().ID + "health", this.currentHealth);
            data.floatSavedData.Add(GetDataID().ID + "power", this.currentPower);
        }
    }

    public void LoadData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID];
            //更改坐标
            this.currentHealth = data.floatSavedData[GetDataID().ID + "health"];
            this.currentPower = data.floatSavedData[GetDataID().ID + "power"];

            //通知UI更新
            OnHealthAndPowerChange?.Invoke(this);
        }
    }
}
