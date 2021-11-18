using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelAtlas", menuName = "Greed/LevelAtlas", order = 0)]
public class LevelAtlas : ScriptableObject {
    [SerializeField] private List<GameObject> levels;
    private static int index = -1;

    public GameObject GetRandom() {
        int newIndex = Random.Range(0, levels.Count);
        while (newIndex == index) {
            newIndex = Random.Range(0, levels.Count);
        }
        index = newIndex;
        return levels[index];
    }

    public GameObject Get0(){
        return levels[0];   
    }
}

