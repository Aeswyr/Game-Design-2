using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject dart;
    [SerializeField] private float dartDelay;
    [SerializeField] private float range;
    [SerializeField] private int health;
    private float nextTime;
    private Collider2D owner;
    [SerializeField] private Collider2D hurtbox;
    private PlayerManager playerManager;
    public void Init(Collider2D owner) {
        this.owner = owner;
        playerManager = FindObjectsOfType<PlayerManager>()[0];
    }
    // Start is called before the first frame update
    void Start()
    {
        nextTime = Time.time + dartDelay;
        range = range * range;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time >= nextTime && TryFindPlayer(out Vector2 dir)) {
            nextTime = Time.time + dartDelay;
            
            GameObject dart1 = Instantiate(dart);
            dart1.GetComponent<Projectile>().Init(dir, hurtbox);
            dart1.transform.position = transform.position;
        }
    }

    private bool TryFindPlayer(out Vector2 dir) {
        foreach (var player in playerManager.GetPlayers()) {
            if (IsInRange(player.transform.position) && !player.GetComponent<PlayerController>().IsHurtboxOwner(owner)) {
                Vector3 dif = player.transform.position - transform.position;
                dir = new Vector2(dif.x, dif.y);
                return true;
            }
        }
        dir = Vector2.zero;
        return false;
    }

    private bool IsInRange(Vector3 pos) {
        return (pos - transform.position).sqrMagnitude <= range;
    }

    private void OnTriggerEnter2D(Collider2D other) {

        GameObject obj = null;
        if (other.transform.parent != null)
            obj = other.transform.parent.gameObject;
        else
            obj = other.gameObject;
        if (obj.TryGetComponent(out PlayerController player)) {
            if (player.IsHurtboxOwner(owner))
                return;
             Hurt();
        } else if (TryGetComponent(out Projectile proj)) {
            if (proj.IsValidTarget(gameObject))
                 Hurt();
        }
    }

    private void Hurt() {
        health--;
        if (health <= 0)
            Destroy(gameObject);
    }
}
