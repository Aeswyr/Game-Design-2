using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private Projectile projectile;
    private void OnTriggerEnter2D(Collider2D other) {
        if (projectile.IsValidTarget(other.gameObject)) {
            Destroy(this.gameObject);
        }
    }

    void OnDisable() {
        Instantiate(explosion, transform.position, Quaternion.identity);
    }


}
