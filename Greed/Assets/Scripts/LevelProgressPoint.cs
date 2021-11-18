using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressPoint : MonoBehaviour
{
    [SerializeField] private LevelAtlas levelAtlas;
    private int hp = 3;
    private int gameOver = 5;

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Impact!");
        hp--;
        gameOver--;
        if (hp == 0 && gameOver != 0) {
            GameObject old = FindObjectsOfType<LevelData>()[0].gameObject;
            Destroy(old);
            Instantiate(levelAtlas.GetRandom());
            FindObjectsOfType<PlayerManager>()[0].SpawnPlayers();

            
        }else if(hp == 0 && gameOver == 0){
            
        }
    }
}
