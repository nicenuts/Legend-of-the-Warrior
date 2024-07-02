using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;
    public Character currentCharacter;
    public bool isRecovering;
    private void Update()
    {
        HealthReduce();  //血条减少的渐变效果
        PowerRecover(currentCharacter);   //能量条随时间回复效果
    }

    public void OnHealthChange(float persentageHealth)  //血条变化
    {
        healthImage.fillAmount = persentageHealth;
    }


    public void OnPowerChange(Character character)   //能量条变化
    {
        isRecovering = true;
        float persentagePower = character.currentPower / character.maxPower;
        powerImage.fillAmount = persentagePower;
        currentCharacter = character;
    }

    public void HealthReduce()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)  //如果红色血条大于绿色的血条
        {
            healthDelayImage.fillAmount -= Time.deltaTime * 0.3f;  //让红色的血条逐渐跟上绿色血条
        }
    }
    public void PowerRecover(Character character)
    {
        if (isRecovering == true)
        {
            float persentage = currentCharacter.currentPower / currentCharacter.maxPower;
            OnPowerChange(character);
            if(persentage >= 1)
            {
                isRecovering = false;
            }
        }
    }
}
