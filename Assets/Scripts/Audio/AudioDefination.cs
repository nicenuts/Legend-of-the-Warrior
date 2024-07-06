using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    public PlayAudioEventSO playAudioEvent;
    public AudioClip audioClip;
    public bool playOnEnable;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            StartCoroutine(PlayBGMWithDelay());
        }
    }
    public void PlayAudioClip()  //播放音乐片段
    {
        playAudioEvent.RaiseEvent(audioClip);   //呼叫
    }

    IEnumerator PlayBGMWithDelay()
    {
        yield return new WaitForSeconds(2.0f); // 等待2秒钟，音频系统需要准备时间
        PlayAudioClip();
    }

}
