using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxController : MonoBehaviour
{

    [SerializeField] private LayerMask mask;
    [SerializeField] private PlayerController player;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.TryGetComponent(out Projectile proj)) {
            if (!proj.IsValidTarget(gameObject))
                return;
            else
                Destroy(proj.gameObject);
        }

        if (other.gameObject.TryGetComponent(out ForceBox force)) {
            if (force.transform.parent == transform.parent)
                return;
            Vector2 dir = new Vector2(transform.position.x - other.transform.position.x, transform.position.y - other.transform.position.y);
            player.Knockback(dir, 30f);
            return;
        }

        player.OnHit();
    }
}
