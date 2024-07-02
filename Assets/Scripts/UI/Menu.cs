using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public GameObject newGameButton;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(newGameButton);  //固定写法
    }

    public void ExitGame()
    {
        Debug.Log("Quit!!!");
        Application.Quit();  //程序退出  固定写法
    }
}
