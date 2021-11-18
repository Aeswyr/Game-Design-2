using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] private GameObject infoCardHolder;

    void Start() {
        foreach (ControllerManager player in FindObjectsOfType<ControllerManager>())
            player.SpawnPlayer();
    }

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

    public GameObject GetInfoHolder() {
        return infoCardHolder;
    }

    public List<GameObject> GetPlayers() {
        return players;
    }

    public List<PickupType> GetCrownsInPlay() {
        var crowns = new List<PickupType>();
        foreach (var player in players) {
            var crown = player.GetComponent<PlayerController>().GetCrowns();
            foreach (var c in crown)
                crowns.Add(c);
        }
        return crowns;
    }
}
