using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] float lifetime;
    private float start;
    // Start is called before the first frame update
    void Start()
    {
        start = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (Time.time > start + lifetime)
            Destroy(gameObject);
    }
}
