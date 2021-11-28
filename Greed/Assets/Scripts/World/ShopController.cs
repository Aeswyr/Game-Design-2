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
    [SerializeField] private GameObject glow;

    [SerializeField] private int cost;
    [SerializeField] private PickupType costType;
    [SerializeField] private PickupType item;

    [SerializeField] private bool singleUse = false;

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

        if (item == PickupType.CROWN_RED || item == PickupType.CROWN_GREEN ||
            item == PickupType.CROWN_BLUE || item == PickupType.CROWN_BATTLE)
            glow.SetActive(true);
    }

    public void Purchase(PlayerController player) {
        if (player.RemoveItem(costType, cost)) {
            if (!player.AddItem(item))
                player.AddItem(costType, cost);
            else
                if (singleUse)
                    Destroy(gameObject);
        }
    }
}
