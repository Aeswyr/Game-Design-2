using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{
    [SerializeField] private ParticleSystem rubble;
    private Vector2 vel;
    private bool drilling = false;
    [SerializeField] private Rigidbody2D rbody;

    // Start is called before the first frame update
    void Start()
    {
        rubble.Stop();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!drilling) {
            rubble.Play();
            vel = rbody.velocity;
            rbody.velocity *= 0.3f;
            drilling = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (drilling) {
            rubble.Stop();
            rbody.velocity = vel;
            drilling = false;
        }
        
    }
}
