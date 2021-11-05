using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    private int health = 2;
    [SerializeField] private GameObject gem;

    private void OnTriggerEnter2D(Collider2D other) {
        if (((1 << other.gameObject.layer) & mask) != 0) {
            health--;
            if (health == 0) {
                for (int i = 0; i < 10; i++) {
                    GameObject newGem = Instantiate(gem, transform.position, gem.transform.rotation);
                    newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(0.5f, 20f)), ForceMode2D.Impulse);
                    newGem.GetComponent<ItemPickup>().SetType((PickupType)Random.Range(1, 4));
                }
                Destroy(gameObject);
            }
        }
    }
}
