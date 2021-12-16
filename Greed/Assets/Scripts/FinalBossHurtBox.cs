using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossHurtBox : MonoBehaviour
{
    [SerializeField] int health; 
    [SerializeField] GameObject gPickupPrefab;
    [SerializeField] private GameObject hitspark;
    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private FinalBossMovement boss;

    void Start() {
        health = health * FindObjectOfType<PlayerManager>().GetPlayers().Count;
    }
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
            //player.Knockback(dir, 30f);
            return;
        }

        health--;
        if(health <= 0){
            Destroy(transform.parent.gameObject);
            GemBurst(99);
            FindObjectsOfType<LevelDirector>()[0].StartGameEndSequence();
        }
        
        Transform t1 = transform.parent;
        Transform t2 = other.transform;
        if (other.transform.parent != null)
            t2 = other.transform.parent;
        Instantiate(hitspark, 0.5f * (transform.position + other.transform.position), hitspark.transform.rotation);
        Instantiate(dustPrefab, t1);
        boss.Hurt();
        EffectsMaster.Instance.ScreenShake(0.5f, 0.2f);
        if (t2.TryGetComponent(out PlayerController p2))
            p2.Pause(0.2f);
    }
    public void GemBurst(int drop) {
        for (int i = 0; i < drop / 10; i++){
            DropGravPickup(PickupType.GEM_GREEN_LARGE);
            DropGravPickup(PickupType.GEM_RED_LARGE);
            DropGravPickup(PickupType.GEM_BLUE_LARGE);
        }
        for (int i = 0; i < drop % 10; i++){
            DropGravPickup(PickupType.GEM_GREEN);
            DropGravPickup(PickupType.GEM_RED);
            DropGravPickup(PickupType.GEM_BLUE); 
        }
    }
    private void DropGravPickup(PickupType type) {
        GameObject newGem = Instantiate(gPickupPrefab, transform.position, gPickupPrefab.transform.rotation);
        newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(20f, 40f)), ForceMode2D.Impulse);
        newGem.GetComponent<ItemPickup>().SetType(type);
    }
    
} 
