using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfoManager : MonoBehaviour
{
//GemCount
    [SerializeField] private TextMeshProUGUI blueText;
    [SerializeField] private TextMeshProUGUI greenText;
    [SerializeField] private TextMeshProUGUI redText;

//Stamina
    private Image active;
    [SerializeField] private Image barOn;
    [SerializeField] private Image barOff;

//Display
    [SerializeField] private Image Portrait;

    

    public void PushGemCount(int count, PickupType type) {
        switch (type) {
            case PickupType.GEM_BLUE:
                blueText.text = count.ToString();
                break;
            case PickupType.GEM_RED:
                redText.text = count.ToString();
                break;
            case PickupType.GEM_GREEN:
                greenText.text = count.ToString();
                break;
            default:
                break;
        }
    }

    public void PushPortraitSprite(Sprite sprite) {
        Portrait.sprite = sprite;
    }

    public void PushStamina(int currentStamina) {
        if (currentStamina >= PlayerController.STAMINA_COST) {
            barOn.enabled = true;
            barOff.enabled = false;
            active = barOn;
        } else {
            barOn.enabled = false;
            barOff.enabled = true;
            active = barOff;
        }

        active.fillAmount = 1f * currentStamina / PlayerController.MAX_STAMINA;
    }
}
