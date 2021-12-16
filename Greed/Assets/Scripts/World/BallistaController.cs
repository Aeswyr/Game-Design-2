using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallistaController : MonoBehaviour
{
    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject price;
    private float nextFireTime;
    [SerializeField] private float reload;
    private Vector2 dir;
    void Start() {
        dir = new Vector2(transform.localScale.x, 0);
        price.transform.localScale = transform.localScale;
        Vector3 p = price.transform.localPosition;
        p.x *= transform.localScale.x;
        price.transform.localPosition = p;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (Time.time < nextFireTime)
            return;

        if (other.transform.parent != null
            && other.transform.parent.gameObject.TryGetComponent(out PlayerController player)
            && player.RemoveItem(PickupType.GEM_RED, 5)) {
        
                Instantiate(dustPrefab, transform.position + new Vector3(dir.x, 0, 0), dustPrefab.transform.rotation);
                GameObject arrow = Instantiate(arrowPrefab, transform.position + new Vector3(dir.x, 0, 0), dustPrefab.transform.rotation);
                arrow.GetComponent<Projectile>().Init(dir, player.GetHurtbox());
                nextFireTime = Time.time + reload;
        }
    }
}
