using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectManager : MonoBehaviour {
    private List<PlayerSelectController> players = new List<PlayerSelectController>();
    [SerializeField] private GameObject tutorialPrefab;

    public void RegisterPlayer(PlayerSelectController player) {
        players.Add(player);
    }

    public void StartGame() {
        List<ControllerManager> controllers = new List<ControllerManager>();
        foreach (var player in players) {
            controllers.Add(player.GetController());
        }

        foreach (var controller in controllers) {
            controller.RecordCharacterData();
        }
        SceneManager.LoadScene(0);
    }

    public void ShowTutorial() {
        foreach (var player in players) {
            player.Lock();
            GameObject tutorial = Instantiate(tutorialPrefab, transform.parent);
            tutorial.GetComponent<TutorialController>().Init(this);
        }
    }
    public void CheckReady() {
        foreach (var player in players) {
            if (!player.GetReady())
                return;
        }
        ShowTutorial();
    }
}