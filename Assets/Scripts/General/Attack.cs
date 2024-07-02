using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("攻击的属性")]
    public int damage;
    public float attackRange;
    public float attackRate;


    private void OnTriggerStay2D(Collider2D other)
    {
        other.GetComponent<Character>()?.TakeDmage(this);
        //让被攻击，受到伤害的对象去计算自己受到的伤害，更新血量
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
