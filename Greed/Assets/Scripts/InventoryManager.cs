using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] slots;
    [SerializeField] private ItemIconAtlas icons;

    public void Display(List<PickupType> inv) {
        for (int i = 0; i < 3; i++) {
            if (i < inv.Count)
                slots[i].sprite = icons.GetSprite(inv[i]);
            else
                slots[i].sprite = null;
        }
    }
}
