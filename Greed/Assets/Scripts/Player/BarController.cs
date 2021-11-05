using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    [SerializeField] private GameObject mask;

    private float duration;
    private float end;

    public void StartTimer(float duration) {
        this.duration = duration;
        this.end = Time.time + duration;
        mask.transform.localPosition = new Vector3(4, 0, 0);
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time <= end)
            mask.transform.localPosition = new Vector3(4 * (end - Time.time) / duration, 0, 0);

        if (Time.time > end)
            gameObject.SetActive(false);
    }
}
