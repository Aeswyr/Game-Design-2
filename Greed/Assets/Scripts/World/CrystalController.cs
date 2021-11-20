using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    [SerializeField] private Lootable loot;
    [SerializeField] private SpriteAtlas atlas;
    [SerializeField] private SpriteRenderer sprite;

    public void SetLootType(LootType type) {
        sprite.sprite = atlas.GetSprite((int)type - 1);
        loot.SetLootType(type);
    }

}
