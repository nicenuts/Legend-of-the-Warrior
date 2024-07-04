using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    private PlayerController playerController;
    private Rigidbody2D rb;

    [Header("检测参数")]
    // public bool manual;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public float checkRadius;
    public LayerMask groundLayer;

    public bool isPlayer;  //因为敌人是不需要蹬墙跳这个动作的

    [Header("状态")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;   //蹬墙跳的时候，判断是否在墙上

    void Start()
    {

    }
    void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        if(isPlayer)
        {
            playerController = GetComponent<PlayerController>();
            rb = GetComponent<Rigidbody2D>();
        }

        // if(!manual)
        // {
        //     rightOffset = new Vector2(coll.bounds.size.x/2 + coll.offset.x, coll.offset.y);
        //     leftOffset = new Vector2(-coll.bounds.size.x/2 + coll.offset.x, coll.offset.y);
        // }
    }
    // Update is called once per frame
    void Update()
    {
        Check();
    }

    public void Check()
    {
        //检测人物是否站在地面上
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRadius, groundLayer);
        //检测墙面
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundLayer);

        //蹬墙跳检测
        if(isPlayer)
        {
            //if(((touchLeftWall && playerController.inputDirection.x<0f) || 
            //    (touchRightWall && playerController.inputDirection.x>0f))   &&   rb.velocity.y<0f  )
            if((touchLeftWall || touchRightWall )   &&   rb.velocity.y<0f  )
            {
                onWall = true;
            }
            else
            {
                onWall = false;
            }
        }


    }
    private void OnDrawGizmosSelected()
    {
        //使检测的距离可视化
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
    }

}
