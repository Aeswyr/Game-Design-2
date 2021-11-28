using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressPoint : MonoBehaviour
{
    private int hp = 3;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject dustPrefab;
    private float lastInteract;

    private void OnTriggerEnter2D(Collider2D other) {
        if (Time.time < lastInteract + 1)
            return;
        hp--;
        lastInteract = Time.time;
        Instantiate(dustPrefab, transform);
        animator.SetTrigger("Interact");
        EffectsMaster.Instance.ScreenShake(0.1f + 0.2f * (3f - hp), 0.2f);
        if (hp < 0) {
            FindObjectsOfType<LevelDirector>()[0].NextLevel();
        }
    }
}
