using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxController : MonoBehaviour
{

    [SerializeField] private LayerMask mask;
    [SerializeField] private PlayerController player;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.TryGetComponent(out CrystalDart proj)) {
            if (!proj.IsValidTarget(gameObject))
                return;
            else
                Destroy(proj.gameObject);
        }
        player.OnHit();
    }
}
