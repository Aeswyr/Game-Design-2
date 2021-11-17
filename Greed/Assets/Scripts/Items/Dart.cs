using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    private void OnTriggerEnter2D(Collider2D other) {
        if (projectile.IsValidTarget(other.gameObject)) {
            Destroy(this.gameObject);
        }
    }
}
