using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsMaster : MonoBehaviour
{
    public static EffectsMaster Instance => FindObjectOfType<EffectsMaster>();
    private float startShake, endShake, intensity;
    bool shaking = true;
    Vector3 defaultPos;
    void Start() {
        defaultPos = transform.position;
    }
    public void FixedUpdate() {
        if (Time.time <= endShake) {
            transform.position = defaultPos + Shake();
        } if (shaking && Time.time > endShake) {
            shaking = false;
            transform.position = defaultPos;
        }
    }

    public void ScreenShake(float intensity, float duration) {
        startShake = Time.time;
        endShake = startShake + duration;
        this.intensity = intensity;
        shaking = true;
        reset = 0;
    }

    int reset = 6;
    private Vector3 target;
    private Vector3 Shake() {
        if (reset > 0) {
            reset--;
        } else if (reset == 0) {
            reset = 6;
            target = defaultPos + (intensity * (endShake - Time.time) / (endShake - startShake) * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0));
        }
        return 0.8f * (target - transform.position);
    }

    public void SFXPlay(string name) {

    }
}
