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

        PlayerController first = players[0].GetComponent<PlayerController>();
        PlayerController last = players[0].GetComponent<PlayerController>();

        foreach(var player in players) {
            PlayerController p = player.GetComponent<PlayerController>();
            p.DisableDoubleJump();
            if (p.GetGems() > first.GetGems())
                first = p;
            if (p.GetGems() < last.GetGems())
                last = p;
        }

        first.EnableDoubleJump();
        last.AddItem(PickupType.DART);
    }
}
