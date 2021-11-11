using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CharacterDataAtlas", menuName = "Greed/CharacterDataAtlas", order = 0)]
public class CharacterDataAtlas : ScriptableObject {
    [SerializeField] private List<CharacterData> data;

    public CharacterData GetCharacterData(int index) {
        if (index >= data.Count)
            return default(CharacterData);
        return data[index];
    }

    public int GetCharacterCount() {
        return data.Count;
    }
}


[Serializable] public struct CharacterData {
    public Sprite sprite;
    public string name;
    public AnimatorOverrideController animator;
    public Sprite portrait;
}