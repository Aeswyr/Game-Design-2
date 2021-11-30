using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    private void OnTriggerEnter2D(Collider2D other) {
        if (projectile.IsValidTarget(other.gameObject)) {
            Destroy(this.gameObject);
        }
    }
}
