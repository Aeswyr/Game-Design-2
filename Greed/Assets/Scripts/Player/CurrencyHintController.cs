using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyHintController : MonoBehaviour
{

    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private TextMeshPro text;
    int gemsDisp = 0, gemsTarget;
    private static readonly int ALPHA_MAX = 60;
    private int alphaTimer = ALPHA_MAX;
    
    private bool filled;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gemsDisp == gemsTarget) {
            alphaTimer--;
            SetAlpha(1f * alphaTimer / ALPHA_MAX);
        } else {
            if (gemsDisp > gemsTarget) {
                gemsDisp--;
            } else {
                gemsDisp++;
            }
            text.text = gemsDisp.ToString();
        }
    }

    private void SetAlpha(float a) {
        Color c1 = icon.color;
        Color c2 = text.color;
        c1.a = a;
        c2.a = a;
        icon.color = c1;
        text.color = c2;
    }

    public void PushGems(int gems) {
        gemsTarget = gems;
        text.text = gemsDisp.ToString();
        SetAlpha(1f);
        alphaTimer = ALPHA_MAX;
    }
}
