using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private LevelAtlas levelAtlas;
    [SerializeField] private int maxLevel = 20;
    private int level = 0;
    public void NextLevel() {
        level++;
        foreach (var rm in FindObjectsOfType<DisposeOnLevelTransition>())
            Destroy(rm.gameObject);
        GameObject old = FindObjectsOfType<LevelData>()[0].gameObject;
        Destroy(old);

        GameObject lvl = null;
        if (level > maxLevel)
            lvl = Instantiate(levelAtlas.GetBoss());
        else if (level % 5 == 0)
            lvl = Instantiate(levelAtlas.GetShop());
        else if (level % 5 == 4)
            lvl = Instantiate(levelAtlas.GetCrown());
        else
            lvl = Instantiate(levelAtlas.GetRandom());
        lvl.GetComponent<LevelData>().LevelSetup(level, playerManager.GetCrownsInPlay());

        playerManager.SpawnPlayers();
    }
}
