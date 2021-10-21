using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private PlayerController player;
    private void OnTriggerEnter2D(Collider2D other) {
        if (((1 << other.gameObject.layer) & mask) != 0) {
            if (TryGetComponent<ItemPickup>(out ItemPickup pickup)) {
                if (!pickup.CanPickup())
                    return;
            }
            player.AddGem();
            Destroy(other.gameObject);
        }
    }
}
