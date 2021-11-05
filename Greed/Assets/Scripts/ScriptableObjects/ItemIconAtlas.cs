using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ItemIconAtlas", menuName = "Greed/ItemIconAtlas", order = 0)]
public class ItemIconAtlas : ScriptableObject {
    [SerializeField] private List<TypeSpritePair> atlas;

    public Sprite GetSprite(PickupType type) {
        foreach (var val in atlas) {
            if (val.type == type)
                return val.sprite;
        }
        return null;
    }

    
    
}


[Serializable] public struct TypeSpritePair {
    public PickupType type;
    public Sprite sprite;
}