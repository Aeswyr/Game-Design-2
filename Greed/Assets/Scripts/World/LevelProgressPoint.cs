using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressPoint : MonoBehaviour
{
    private int hp = 3;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject dustPrefab;

    private void OnTriggerEnter2D(Collider2D other) {
        hp--;
        Instantiate(dustPrefab, transform);
        animator.SetTrigger("Interact");
        EffectsMaster.Instance.ScreenShake(0.5f, 0.2f);
        if (hp < 0) {
            FindObjectsOfType<LevelDirector>()[0].NextLevel();
        }
    }
}
