using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] slots;
    [SerializeField] private ItemIconAtlas icons;
    public const int INVENTORY_SIZE = 5;

    public void Display(List<PickupType> inv) {
        for (int i = 0; i < INVENTORY_SIZE; i++) {
            if (i < inv.Count)
                slots[i].sprite = icons.GetSprite(inv[i]);
            else
                slots[i].sprite = null;
        }
        transform.localPosition = new Vector3(-0.5f * (inv.Count - 1) * transform.localScale.x, transform.localPosition.y, 0);
    }
}
