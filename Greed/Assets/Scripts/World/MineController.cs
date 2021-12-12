using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject hint;
    [SerializeField] private GameObject priceTag;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float range;
    private bool primed = false;
    private PlayerManager playerManager;
    private GameObject owner;

    void Start() {
        playerManager = FindObjectsOfType<PlayerManager>()[0];
        range = range * range;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (primed)
            return;
        
        if (other.transform.parent != null
            && other.transform.parent.gameObject.TryGetComponent(out PlayerController player)
            && player.RemoveItem(PickupType.GEM_RED, 10)) {
        
            animator.SetTrigger("prime");
            hint.SetActive(false);
            priceTag.SetActive(false);
            primed = true;
            
            owner = player.gameObject;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (primed) {
            foreach (var player in playerManager.GetPlayers()) {
                if ((player.transform.position - transform.position).sqrMagnitude <= range && player != owner) {
                    Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);
                    Destroy(gameObject);
                }
            }
        }
    }
}
