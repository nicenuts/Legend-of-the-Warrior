using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, Iinteractable
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    public bool isDone;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        if(isDone)
        {
         
            spriteRenderer.sprite = openSprite;
        }
        else
        {   
            spriteRenderer.sprite = closeSprite;
        }
    }

    public void TriggerAction()
    {   
        if(!isDone) 
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        spriteRenderer.sprite = openSprite;   //打开宝箱
        isDone = true;   
        this.gameObject.tag = "Untagged";  //修改标签,使宝箱打开之后不能与玩家产生互动
    }
}
