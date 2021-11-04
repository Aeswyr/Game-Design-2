using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelAtlas", menuName = "Greed/LevelAtlas", order = 0)]
public class LevelAtlas : ScriptableObject {
    [SerializeField] private List<GameObject> levels;

    public GameObject GetRandom() {
        return levels[Random.Range(0, levels.Count)];
    }
}

