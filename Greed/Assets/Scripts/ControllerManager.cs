using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{

    //Data and Refs
    [SerializeField] private InputManager input;
    [SerializeField] private CharacterDataAtlas dataAtlas;

    //Created Gameobjects
    [SerializeField] private GameObject playerPrefab;
    private PlayerController player;
    [SerializeField] private GameObject playerCardPrefab;
    private PlayerSelectController playerCard;

    private CharacterData characterData;

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectsOfType<PlayerManager>().Length > 0) {
            characterData = dataAtlas.GetCharacterData(0);
            SpawnPlayer();
        } else {
            GameObject parent = GameObject.Find("PlayerHolder");
            playerCard = Instantiate(playerCardPrefab, parent.transform).GetComponent<PlayerSelectController>();
            playerCard.BindInputs(input);
            playerCard.BindController(this);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void RecordCharacterData() {
        characterData = playerCard.GetCharacterData();
    }

    public void SpawnPlayer() {
        player = Instantiate(playerPrefab).GetComponent<PlayerController>();
        player.BindInputs(input);
        player.SetCharacterData(characterData);
    }

    public void Disconnect() {
        player.RemoveUI();
        Destroy(player.gameObject);
        Destroy(playerCard.gameObject);
    }
}
