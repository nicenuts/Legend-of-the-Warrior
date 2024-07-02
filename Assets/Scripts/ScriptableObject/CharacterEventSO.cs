using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    public UnityAction<Character> OnEventRaised;    //事件

    public void RaiseEvent(Character character)
    {
        OnEventRaised?.Invoke(character);   //invoke事件调用
    }

}
