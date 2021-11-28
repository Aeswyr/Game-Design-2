using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField] private GameObject hitsparkPrefab;
    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private GameObject debrisPrefab;

    [SerializeField] private int health = 1;
    [SerializeField] Lootable lootable;
    private void OnTriggerEnter2D(Collider2D other) {
        health--;

            Transform t1 = transform.parent;
            Transform t2 = other.transform;
            if (other.transform.parent != null)
                t2 = other.transform.parent;
            Vector2 dir = new Vector2(t1.position.x - t2.position.x, Mathf.Max(Mathf.Abs(t1.position.y - t2.position.y), 5.5f));
            Instantiate(hitsparkPrefab, 0.5f * (transform.position + other.transform.position), hitsparkPrefab.transform.rotation);
            Instantiate(dustPrefab, 0.5f * (transform.position + other.transform.position), dustPrefab.transform.rotation);
            Instantiate(debrisPrefab, transform.position, debrisPrefab.transform.rotation);
            EffectsMaster.Instance.ScreenShake(0.5f, 0.2f);
            if (t2.TryGetComponent(out PlayerController p2))
                p2.Pause(0.2f);
        if (health == 0) {
            if (lootable != null)
                lootable.Loot();
            Destroy(gameObject);
        }
    }
}
