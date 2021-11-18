using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private List<string> lines;
    private int index;
    [SerializeField] private float delay;
    [SerializeField] private TextMeshProUGUI text;
    private float timer;
    private PlayerSelectManager manager;
    public void Init(PlayerSelectManager manager) {
        this.manager = manager;
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = Time.time;
        PushLine();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > timer + delay) {
            index++;
            if (index < lines.Count) {
                PushLine();
                timer = Time.time;
            } else
                manager.StartGame();
        }
    }

    private void PushLine() {
        text.text = lines[index];
    }
}
