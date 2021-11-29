using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatyText : MonoBehaviour
{
    [SerializeField] private TextMeshPro disp;
    private float startTime;
    void Start() {
        startTime = Time.time;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += new Vector3(0, 0.01f, 0);
        if (Time.time > startTime + 1) {
            Color col = disp.color;
            col.a = startTime + 2 - Time.time;
            disp.color = col;
        }
    }

    public void Set(string text) {
        disp.text = text;
    }
}
