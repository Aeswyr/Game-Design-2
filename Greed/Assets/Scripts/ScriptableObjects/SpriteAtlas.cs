using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteAtlas", menuName = "Greed/SpriteAtlas", order = 0)]
public class SpriteAtlas : ScriptableObject {
    [SerializeField] private List<Sprite> sprites;

    public Sprite GetSprite(int index) {
        if (index >= sprites.Count)
            return null;
        return sprites[index];
    }
}
