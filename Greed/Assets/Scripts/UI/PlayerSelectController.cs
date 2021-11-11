using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSelectController : MonoBehaviour
{
    
    private InputManager input;
    [SerializeField] private CharacterDataAtlas characterData;
    private PlayerSelectManager selectManager;

    private ControllerManager controller;

    [SerializeField] private TextMeshProUGUI chName;
    [SerializeField] private Image portrait;
    [SerializeField] private GameObject ready;
    private bool isReady = false;

    private int chindex = 0;
    private int coindex = 0;

    void Start()
    {
        selectManager = FindObjectsOfType<PlayerSelectManager>()[0];
        selectManager.RegisterPlayer(this);

        SetCharacterIndex(0);
        SetColorIndex(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isReady) {
            if (input.A)
                SetReady(true);
            if (input.Move) {
                ProcessPortraitChange(input.Dir);
            }
        } else {
            if (input.B)
                SetReady(false);
        }
        input.NextInputFrame();
    }

    private void ProcessPortraitChange(Vector2 change) {
        if (change.y != 0) {
            SetCharacterIndex((int)(change.y / Mathf.Abs(change.y)));
        }
        if (change.x != 0) {
            SetColorIndex((int)(change.x / Mathf.Abs(change.x)));
        }
    }

    private void SetCharacterIndex(int increment) {
        chindex = Increment(chindex, increment, characterData.GetCharacterCount());
        CharacterData chdata = characterData.GetCharacterData(chindex);

        portrait.sprite = chdata.portrait;
        chName.text = chdata.name;
    }

    private void SetColorIndex(int increment) {
        coindex = Increment(coindex, increment, 1);
    }

    private int Increment(int val, int i, int cap) {
        val = (val + i) % cap;
        if (val < 0)
            val = cap + val;
        return val;
    }

    private void SetReady(bool val) {
        ready.SetActive(val);
        isReady = val;
        selectManager.CheckReady();
    }

    public bool GetReady() {
        return isReady;
    }

    public CharacterData GetCharacterData() {
        return characterData.GetCharacterData(chindex);
    }

    public void BindInputs(InputManager input) {
        this.input = input;
    }

    public void BindController(ControllerManager controller) {
        this.controller = controller;
    }

    public ControllerManager GetController() {
        return controller;
    }
}
