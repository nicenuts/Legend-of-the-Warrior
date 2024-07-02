using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    Rigidbody2D rb;
    [HideInInspector] public Animator anim;    // protected:子类可以访问，但外部访问不了

    [HideInInspector] public PhysicsCheck physicsCheck;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    public Vector3 faceDir;
    public Transform attacker;
    public float hurtForce;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;


    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;
    public float lostTime;
    public float lostTimeCounter;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;

    private BaseState currentState;
    private BaseState newState;
     protected BaseState patrolState;  //巡逻状态
    protected BaseState chaseState;  //追击状态

    protected virtual void Awake()
    {
        //变量的获取
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        currentSpeed = normalSpeed;
        // waitTimeCounter = waitTime;
    }

    private void OnEnable()   //物体被激活时调用
    {
        currentState = patrolState;

        currentState.OnEnter(this);  //把当前对象传递给currentEnemy
    }



    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);  //敌人面朝左faceDir是-1，面朝右是1

        currentState.LogicUpdate();  //逻辑判定
        TimeCounter();
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isDead && !wait)
        {
            Move();
        }

        currentState.PhysicsUpdate();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    //检测敌人周围是否有玩家
    public bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
    } 

    public void SwitchState(NPCstate state)   //状态切换
    {
        var newState = state switch
        {
            NPCstate.patrol => patrolState,
            NPCstate.chase => chaseState,
            _ => null
        };

        currentState.OnExit();     //之前的状态 退出
        currentState = newState;   //切换到新状态
        currentState.OnEnter(this);
    }

    #region 移动
    public virtual void Move()   //虚函数，可用override重写其函数
    {
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }
    #endregion

    #region 计时器
    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1, 1);//敌人转向
            }
        }

        if (!FoundPlayer())
        {
            lostTimeCounter -= Time.deltaTime;
        }
        else
        {
            lostTimeCounter = lostTime;
        }



    }
    #endregion

    #region 事件执行方法

    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        //转身
        if (attackTrans.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (attackTrans.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //受伤被击退
        isHurt = true;
        anim.SetTrigger("hurt"); //执行受伤动画
        Vector2 dir = new Vector2(transform.position.x - attacker.transform.position.x, 0).normalized;

        StartCoroutine(OnHurt(dir));

    }

    private IEnumerator OnHurt(Vector2 dir)
    {
        wait = false;
        waitTimeCounter = waitTime;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }


    public void OnDie()
    {
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead = false;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        //射线的检测范围（检测敌人周围是否出现玩家）
        Gizmos.DrawWireSphere(transform.position+(Vector3)centerOffset+new Vector3(checkDistance*(-transform.localScale.x),0,0), 0.2f);
    }

}