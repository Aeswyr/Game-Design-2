using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour
{

    [SerializeField] private GameObject gem;
    [SerializeField] private int amount;
    [SerializeField] private LootType type;


    public void Loot() {
        for (int i = 0; i < amount % 10; i++) {
            PickupType pick = GetPickupType(false);
            GameObject newGem = Instantiate(gem, transform.position, gem.transform.rotation);
            newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(0.5f, 20f)), ForceMode2D.Impulse);
            newGem.GetComponent<ItemPickup>().SetType(pick);
        }
        for (int i = 0; i < amount / 10; i++) {
            PickupType pick = GetPickupType(true);
            GameObject newGem = Instantiate(gem, transform.position, gem.transform.rotation);
            newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(0.5f, 20f)), ForceMode2D.Impulse);
            newGem.GetComponent<ItemPickup>().SetType(pick);
        }
    }

    private PickupType GetPickupType(bool big) {
        switch (type) {
            case LootType.DEFAULT:
                if (big)
                    return (PickupType)Random.Range(4, 7);
                return (PickupType)Random.Range(1, 4);
            case LootType.RED:
                if (big)
                    return PickupType.GEM_RED_LARGE;
                return PickupType.GEM_RED;
            case LootType.GREEN:
                if (big)
                    return PickupType.GEM_GREEN_LARGE;
                return PickupType.GEM_GREEN;
            case LootType.BLUE:
                if (big)
                    return PickupType.GEM_BLUE_LARGE;
                return PickupType.GEM_BLUE;
        }
        return PickupType.DEFAULT;
    }

    public void SetLootType(LootType type) {
        this.type = type;
    }
}

public enum LootType {
    DEFAULT, RED, BLUE, GREEN,
}
