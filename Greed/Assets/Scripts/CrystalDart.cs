using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalDart : MonoBehaviour
{

    private GameObject parent;
    [SerializeField] private int speed;
    [SerializeField] Rigidbody2D rbody;
    [SerializeField] float lifetime;
    private float start;
    public void Init(int dir, GameObject parentHurtbox) {
        rbody.velocity = new Vector3(speed * dir, 0, 0);
        parent = parentHurtbox;
    }

    public bool IsValidTarget(GameObject other) {
        return other != parent;
    }

    private void FixedUpdate() {
        if (Time.time > start + lifetime)
            Destroy(gameObject);
    }

    private void Start() {
        start = Time.time;
    }
}
