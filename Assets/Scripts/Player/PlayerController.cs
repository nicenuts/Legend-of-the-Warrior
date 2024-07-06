using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    [Header("事件监听")]
    public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;

    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;
    public CapsuleCollider2D coll;
    public Character currentCharacter;
    
    [Header("基本参数")]
    public float speed;
    public float jumpForce;
    public float hurtForce;
    public float slideDistance;  //滑铲距离
    public float slideSpeed;     //滑铲速度                                                                                                                                                                                                                                                                                                                                            
    public float sildePowerCost;  //滑铲消耗的能量值                                                                                                                                                                    


    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;



    [Header("状态")]
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool isSlide;


    private void Awake()
    {
        inputControl = new PlayerInputControl();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerAnimation = GetComponent<PlayerAnimation>();
        coll = GetComponent<CapsuleCollider2D>();
        currentCharacter = GetComponent<Character>();

        //跳跃
        inputControl.Gameplay.Jump.started += jump;

        //攻击
        inputControl.Gameplay.Attack.started += PlayerAttack;

        //滑铲
        inputControl.Gameplay.Slide.started += Slide;

        inputControl.Enable();
    }


    private void OnEnable()
    {
        sceneLoadEvent.LoadRequestEvent += OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
    }
    private void OnDisable()
    {
        inputControl.Disable();
        sceneLoadEvent.LoadRequestEvent -= OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
    }


    // Update is called once per frame
    void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
    }


    private void FixedUpdate()
    {
        //如果受伤就停止移动
        if (!isHurt && !isAttack)
        {
            Move();
        }

        CheckState();
    }


    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.Gameplay.Disable();   //场景加载时候，人物不能移动
    }

    private void OnAfterSceneLoadedEvent()
    {
        inputControl.Gameplay.Enable();    //场景加载完成后，人物可以移动
    }

    private void OnLoadDataEvent()   //读取游戏进度
    {
        isDead = false;
    }

    //移动
    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        int faceDir = (int)transform.localScale.x;

        if (inputDirection.x > 0)
        {
            faceDir = 1;
        }
        if (inputDirection.x < 0)
        {
            faceDir = -1;
        }

        //人物翻转
        transform.localScale = new Vector3(faceDir, 1, 1);

    }

    //跳跃
    private void jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround || physicsCheck.onWall)   //正常跳跃
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);  //向上的力

            GetComponent<AudioDefination>()?.PlayAudioClip();   //播放跳跃的音效,只有跳跃的时候才播放
            
            //打断滑铲协程
            isSlide = false;
            StopAllCoroutines();
        }
    }

    //攻击
    private void PlayerAttack(InputAction.CallbackContext context)
    {
        playerAnimation.PlayerAttack();
        isAttack = true;
    }

    //滑铲
    private void Slide(InputAction.CallbackContext context)  //滑铲的动画
    {
        if(currentCharacter.currentPower<currentCharacter.sildePowerCost)
        {
            return;  //当前能量值不满足滑动所需能量，不执行滑动
        }
        float slideDirection = transform.localScale.x;   //滑动方向
        if (!isSlide && physicsCheck.isGround && currentCharacter.currentPower >= 0)
        {
            isSlide = true;

            //滑动结束后的目标点
            Vector3 targetPos = new Vector3(transform.position.x + slideDistance * slideDirection, transform.position.y);

            //gameObject.layer = LayerMask.NameToLayer("Enemy");  //实现在滑铲时，与敌人碰撞不受伤害
            StartCoroutine(TriggerSlider(targetPos, slideDirection));

            currentCharacter.OnSlidePowerCost(currentCharacter.sildePowerCost);  //滑铲动作消耗能量

        }
    }

    private IEnumerator TriggerSlider(Vector3 target, float slideDirection)
    {
        do
        {
            yield return null;

            if (!physicsCheck.isGround)  //如果滑动时脱离地面
            {
                break;
            }
            if ((physicsCheck.touchLeftWall && slideDirection < 0f) || (physicsCheck.touchRightWall && slideDirection > 0f))  //滑动撞墙
            {
                isSlide = false;
                yield break;
            }

            //滑动动画的时候，移动人物
            rb.MovePosition(new Vector2(transform.position.x + slideDirection * slideSpeed, transform.position.y));
        }
        while (MathF.Abs(target.x - transform.position.x) > 0.5f);
        //如果滑动的距离距离目标滑动的距离差值大于0.5f,则继续滑动

        isSlide = false;  //滑动结束后 修改状态
        gameObject.layer = LayerMask.NameToLayer("Player");
    }



    #region UnityEvent
    //受伤
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        //使角色停下来
        rb.velocity = Vector2.zero;
        //受伤时反弹的方向
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        //施加反弹的力
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    //死亡
    public void PlayerDead()
    {
        isDead = true;
        //关闭输入控制
        inputControl.Gameplay.Disable();
    }

    public void PlayerDestroy()   //玩家角色销毁
    {
        Destroy(this.gameObject);
    }
    #endregion

    public void CheckState()
    {
        //判断人物是否在地面，如果在地面就使用normal材质，否则就使用wall材质
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;

        if(physicsCheck.onWall)
        {
            //蹬墙跳的滑墙下滑速度
            rb.velocity = new Vector2(0, rb.velocity.y/4);  
        }
    }
}
