using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints;
    public int index = 0;


    public Vector3 NextSpawn() {
        if (spawnPoints == null || spawnPoints.Count == 0) {
            Debug.Log("Level is missing spawn points");
            return Vector3.zero;
        }
        Vector3 pos = spawnPoints[index].transform.position;
        index = (index + 1) % spawnPoints.Count;
        return pos;
    }

}
