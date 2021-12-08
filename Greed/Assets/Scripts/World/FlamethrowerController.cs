using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerController : MonoBehaviour
{

    private bool on = false;
    [SerializeField] private GameObject hint;
    [SerializeField] private GameObject priceTag;
    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private GameObject firePrefab;
    private float nextFire;
    [SerializeField] private float reload;
    private float firingUntil;
    [SerializeField] private float duration;
    private Collider2D owner;

    private void OnTriggerEnter2D(Collider2D other) {
        if (on)
            return;

        if (other.transform.parent != null
            && other.transform.parent.gameObject.TryGetComponent(out PlayerController player)
            && player.RemoveItem(PickupType.GEM_RED, 10)) {
        
                on = true;

                priceTag.SetActive(false);
                hint.SetActive(false);
                owner = player.GetHurtbox();
        }
    }

    int alt = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (on && Time.time > nextFire) {
            nextFire = Time.time + reload;
            firingUntil = Time.time + duration;
            Instantiate(dustPrefab, transform.position + new Vector3(1, 0, 0), dustPrefab.transform.rotation);
            Instantiate(dustPrefab, transform.position + new Vector3(-1, 0, 0), dustPrefab.transform.rotation);
        }

        if (on && Time.time < firingUntil) {
            alt++;
            if (alt % 2 == 0) {
                GameObject fire = Instantiate(firePrefab, transform.position + new Vector3(1, -0.4f, 0), firePrefab.transform.rotation);
                fire.GetComponent<Projectile>().Init(Vector2.right, owner);
                GameObject fire2 = Instantiate(firePrefab, transform.position + new Vector3(-1, -0.4f, 0), firePrefab.transform.rotation);
                fire2.GetComponent<Projectile>().Init(Vector2.left, owner);
            }
        }
    }
}
