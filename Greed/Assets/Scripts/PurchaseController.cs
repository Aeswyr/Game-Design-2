using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseController : MonoBehaviour
{

    [SerializeField] private ShopController shop;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other) {
        shop.Purchase(other.transform.parent.parent.GetComponent<PlayerController>());
    }
}
