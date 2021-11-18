using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressPoint : MonoBehaviour
{
    private int hp = 3;

    private void OnTriggerEnter2D(Collider2D other) {
        hp--;

        if (hp == 0) {
            FindObjectsOfType<LevelDirector>()[0].NextLevel();
        }
    }
}
