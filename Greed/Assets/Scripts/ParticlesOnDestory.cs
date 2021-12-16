using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesOnDestory : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    void OnDisable() {
        Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);
    }
}
