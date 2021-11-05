using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfoManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI blueText;
    [SerializeField] private TextMeshProUGUI greenText;
    [SerializeField] private TextMeshProUGUI redText;
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
}
