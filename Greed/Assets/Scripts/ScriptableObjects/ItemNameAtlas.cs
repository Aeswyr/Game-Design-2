using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ItemNameAtlas", menuName = "Greed/ItemNameAtlas", order = 0)]
public class ItemNameAtlas : ScriptableObject {
    [SerializeField] private List<TypeNamePair> atlas;

    public string GetString(PickupType type) {
        foreach (var val in atlas) {
            if (val.type == type)
                return val.name;
        }
        return null;
    }
}

[Serializable] public struct TypeNamePair {
    public PickupType type;
    public string name;
}
