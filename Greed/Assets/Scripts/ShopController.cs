using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopController : MonoBehaviour
{

    [SerializeField] private ItemIconAtlas atlas;
    [SerializeField] private SpriteRenderer itemDisp;
    [SerializeField] private SpriteRenderer costTypeDisp;
    [SerializeField] private TextMeshPro costDisp;

    [SerializeField] private int cost;
    [SerializeField] private PickupType costType;
    [SerializeField] private PickupType item;

    // Start is called before the first frame update
    void Start()
    {
        Set(item, cost, costType);
    }

    public void Set(PickupType item, int cost, PickupType costType) {
        this.item = item;
        itemDisp.sprite = atlas.GetSprite(item);

        this.cost = cost;
        costDisp.text = cost.ToString();

        this.costType = costType;
        costTypeDisp.sprite = atlas.GetSprite(costType);
    }

    public void Purchase(PlayerController player) {
        if (player.RemoveItem(costType, cost)) {
            if (!player.AddItem(item)) {
                player.AddItem(costType, cost);
            }
        }
    }
}
