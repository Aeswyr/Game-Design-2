using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartStorm : MonoBehaviour
{

    [SerializeField] private GameObject dart;
    [SerializeField] private float dartDelay;
    [SerializeField] private float ammo;
    private float nextTime;
    private Vector2 dir;
    private Collider2D owner;
    public void Init(Vector2 dir, Collider2D owner) {
        this.dir = dir;
        this.owner = owner;
    }

    void Start() {
        nextTime = Time.time + dartDelay;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ammo > 0 && Time.time >= nextTime) {
            ammo--;
            nextTime = Time.time + dartDelay;
            
            GameObject dart1 = Instantiate(dart);
            dart1.GetComponent<Projectile>().Init(dir + new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f)), owner);
            dart1.transform.position = transform.position;
        }
    }
}
