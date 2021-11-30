using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    void OnDisable() {
        Instantiate(explosion, transform.position, Quaternion.identity);
    }


}
