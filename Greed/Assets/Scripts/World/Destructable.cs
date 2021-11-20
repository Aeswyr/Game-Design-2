using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField] private int health = 1;
    private void OnTriggerEnter2D(Collider2D other) {
        health--;
        if (health == 0) {
            Destroy(gameObject);
        }
    }
}
