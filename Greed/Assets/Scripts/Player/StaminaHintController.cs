using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaHintController : MonoBehaviour
{

    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer barDepleted;
    [SerializeField] private SpriteRenderer barReady;
    [SerializeField] private GameObject mask;

    private static readonly int ALPHA_MAX = 15;
    private int alphaTimer = ALPHA_MAX;
    
    private bool filled;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (filled) {
            alphaTimer--;
            SetAlpha(1f * alphaTimer / ALPHA_MAX);
        } else if (alphaTimer != ALPHA_MAX) {
            alphaTimer = ALPHA_MAX;
            SetAlpha(1f);
        }
    }

    private void SetAlpha(float a) {
        Color c = background.color;
        c.a = a;
        background.color = c;
        barDepleted.color = c;
        barReady.color = c;
    }

    public void PushStamina(int currentStamina) {
        filled = false;
        if (currentStamina >= PlayerController.STAMINA_COST) {
            barReady.enabled = true;
            barDepleted.enabled = false;
        } else {
            barReady.enabled = false;
            barDepleted.enabled = true;
        }

        if (currentStamina >= PlayerController.MAX_STAMINA)
            filled = true;

        mask.transform.localPosition = new Vector3(2f * currentStamina / PlayerController.MAX_STAMINA, 0, 0);
    }
}
