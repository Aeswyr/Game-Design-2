using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private List<GameObject> players = new List<GameObject>();
    public void RegisterPlayer(GameObject obj) {
        players.Add(obj);
        obj.transform.position = FindObjectsOfType<LevelData>()[0].NextSpawn();
    }

    public void SpawnPlayers() {
        LevelData level = FindObjectsOfType<LevelData>()[0];
        foreach (var player in players)
            player.transform.position = level.NextSpawn();
    }
}
