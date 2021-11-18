using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private LevelAtlas levelAtlas;
    [SerializeField] private int maxLevel = 20;
    private int level = 0;
    private LevelType currentLevelType = LevelType.DEFAULT;

    private float gameEndTime;
    bool gameEnding = false;
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

        currentLevelType = lvl.GetComponent<LevelData>().GetLevelType();

        playerManager.SpawnPlayers();
    }

    private void FixedUpdate() {
        if (gameEnding && Time.time > gameEndTime) {
            gameEnding = false;
            GameEndSequence();
        }
    }

    public LevelType GetCurrentLevelType() {
        return currentLevelType;
    }

    public void StartGameEndSequence() {
        gameEndTime = Time.time + 20;
        gameEnding = true;
    }

    public void GameEndSequence() {

    }
}