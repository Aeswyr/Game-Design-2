using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringAtlas", menuName = "Greed/StringAtlas", order = 0)]
public class StringAtlas : ScriptableObject {
    [SerializeField] private List<string> strings;

    public string GetString(int index) {
        if (index >= strings.Count)
            return null;
        return strings[index];
    }
}
