using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject parent;
    [SerializeField] private int speed;
    [SerializeField] Rigidbody2D rbody;

    
    public void Init(Vector2 dir, Collider2D parentHurtbox) {
        dir.Normalize();
        rbody.velocity = speed * dir;
        parent = parentHurtbox.gameObject;
        transform.localRotation = Quaternion.FromToRotation(Vector2.right, dir);
    }

    public bool IsValidTarget(GameObject other) {
        return other != parent;
    }
}
