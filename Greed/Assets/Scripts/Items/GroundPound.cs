using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPound : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private bool groundPound = false;
    private void OnTriggerEnter2D(Collider2D other) {
        if (groundPound) {
            groundPound = false;
            player.GemBurst(17);
            player.PushBack();
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (groundPound) {
            groundPound = false;
            player.GemBurst(17);
            player.PushBack();
        }
    }

    public void EnableGroundPound() {
        groundPound = true;
    }
}
