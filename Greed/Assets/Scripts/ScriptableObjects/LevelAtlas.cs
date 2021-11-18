using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelAtlas", menuName = "Greed/LevelAtlas", order = 0)]
public class LevelAtlas : ScriptableObject {
    [SerializeField] private List<GameObject> levels;
    private static int index = -1;

    [SerializeField] private List<GameObject> shopLevels;

    [SerializeField] private List<GameObject> crownLevels;

    [SerializeField] private List<GameObject> bossLevels;

    public GameObject GetRandom() {
        int newIndex = Random.Range(0, levels.Count);
        while (newIndex == index) {
            newIndex = Random.Range(0, levels.Count);
        }
        index = newIndex;
        return levels[index];
    }

    public GameObject GetCrown() {
        return crownLevels[Random.Range(0, crownLevels.Count)];
    }

    public GameObject GetBoss() {
        return bossLevels[Random.Range(0, bossLevels.Count)];
    }

    public GameObject GetShop() {
        return shopLevels[Random.Range(0, shopLevels.Count)];
    }
}

