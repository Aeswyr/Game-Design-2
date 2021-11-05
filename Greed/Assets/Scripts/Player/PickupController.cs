using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private PlayerController player;
    private void OnTriggerEnter2D(Collider2D other) {
        AttemptPickup(other);
    }

    private void OnTriggerStay2D(Collider2D other) {
        AttemptPickup(other);
    }

    private void AttemptPickup(Collider2D other) {
        if (((1 << other.gameObject.layer) & mask) != 0) {
            PickupType type = PickupType.DEFAULT;
            if (other.transform.parent.gameObject.TryGetComponent(out ItemPickup pickup)) {
                if (!pickup.CanPickup()){
                    Debug.Log("Simply too slippery");
                    return;
                } 
                type = pickup.GetPickup();
            }
            if (player.AddItem(type))
                Destroy(other.transform.parent.gameObject);
        }
    }
}
