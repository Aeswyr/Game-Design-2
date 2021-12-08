using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    [SerializeField] private GameObject dustPrefab;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.parent != null
            && other.transform.parent.gameObject.TryGetComponent(out PlayerController player)
            && player.RemoveItem(PickupType.GEM_RED, 15)) {
        
                Instantiate(dustPrefab, transform.position + new Vector3(0, 1, 0), dustPrefab.transform.rotation);
                Destroy(gameObject);
        }
    }
}
