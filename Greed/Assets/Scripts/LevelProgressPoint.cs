using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressPoint : MonoBehaviour
{
    [SerializeField] private LevelAtlas levelAtlas;
    private int hp = 3;

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Impact!");
        hp--;
        if (hp == 0) {
            GameObject old = FindObjectsOfType<LevelData>()[0].gameObject;
            Destroy(old);
            Instantiate(levelAtlas.GetRandom());
            FindObjectsOfType<PlayerManager>()[0].SpawnPlayers();
            
        }
    }
}
