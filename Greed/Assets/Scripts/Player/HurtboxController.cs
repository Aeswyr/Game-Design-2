using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxController : MonoBehaviour
{

    [SerializeField] private GameObject hitspark;
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
            Instantiate(hitspark, 0.5f * (transform.position + other.transform.position), hitspark.transform.rotation);
            return;
        }

        if (player.OnHit()) {
            Transform t1 = transform.parent;
            Transform t2 = other.transform;
            if (other.transform.parent != null)
                t2 = other.transform.parent;
            Vector2 dir = new Vector2(t1.position.x - t2.position.x, Mathf.Max(Mathf.Abs(t1.position.y - t2.position.y), 5.5f));
            Instantiate(hitspark, 0.5f * (transform.position + other.transform.position), hitspark.transform.rotation);
            player.Knockback(dir, 20f);
            player.Pause(0.2f);
            EffectsMaster.Instance.ScreenShake(0.5f, 0.2f);
            if (t2.TryGetComponent(out PlayerController p2))
                p2.Pause(0.2f);
        }
    }
}
