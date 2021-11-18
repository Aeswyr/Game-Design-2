using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints;


    [SerializeField] private List<ItemPickup> pickups;
    [SerializeField] private LevelType levelType = LevelType.DEFAULT;
    private int index = 0;
    [SerializeField] private List<ShopController> shops;


    public Vector3 NextSpawn() {
        if (spawnPoints == null || spawnPoints.Count == 0) {
            Debug.Log("Level is missing spawn points");
            return Vector3.zero;
        }
        Vector3 pos = spawnPoints[index].transform.position;
        index = (index + 1) % spawnPoints.Count;
        return pos;
    }

    private void SetPickups() {
        foreach(var pickup in pickups) {
            pickup.SetType(GetItemType());
        }
    }

    private PickupType GetItemType() {
        if (levelType == LevelType.DEFAULT || levelType == LevelType.CROWN || levelType == LevelType.SHOP)
            return (PickupType)(12 + Random.Range(0, 12));
        return (PickupType)(12 + ((int)levelType - 1) * 4 + Random.Range(0, 4));
    }

    
    private PickupType GetItemType(LevelType type) {
        if (type == LevelType.DEFAULT || type == LevelType.CROWN || type == LevelType.SHOP)
            return (PickupType)(12 + Random.Range(0, 12));
        return (PickupType)(12 + ((int)type - 1) * 4 + Random.Range(0, 4));
    }

    private PickupType GetGemType(LevelType type) {
        switch (type) {
            case LevelType.RED:
                return PickupType.GEM_RED;
            case LevelType.GREEN:
                return PickupType.GEM_GREEN;
            case LevelType.BLUE:
                return PickupType.GEM_BLUE;
            default:
                return PickupType.GEM_GREEN;
        }
    }

    public void LevelSetup(int lvl, List<PickupType> crownsInPlay) {
        if (pickups.Count > 0)
            SetPickups();
        if (shops.Count > 0) {
            if (levelType == LevelType.CROWN) {
                FindCrownAndCost(crownsInPlay, out PickupType crown, out PickupType cost, out int costVal);
                foreach (var shop in shops) {
                    shop.Set(crown, costVal, cost);
                }
            } else if (levelType == LevelType.SHOP) {
                int type = (int)LevelType.RED;
                foreach (var shop in shops) {
                    LevelType lt = (LevelType)type;
                    if (type > (int)LevelType.GREEN)
                        lt = (LevelType)Random.Range((int)LevelType.RED, (int)LevelType.GREEN + 1);
                    else
                        type++;
                    shop.Set(GetItemType(lt), lvl * 2, GetGemType(lt));
                }
            }
        }
    }

    private void FindCrownAndCost(List<PickupType> crownsInPlay, out PickupType crown, out PickupType cost, out int costVal) {
        costVal = 25;
        cost = (PickupType)Random.Range(1, 4);
        crown = PickupType.CROWN_BONUS;
        if (!crownsInPlay.Contains(PickupType.CROWN_RED)) {
            crown = PickupType.CROWN_RED;
            cost = PickupType.GEM_RED;
            return;
        }
        if (!crownsInPlay.Contains(PickupType.CROWN_BLUE)) {
            crown = PickupType.CROWN_BLUE;
            cost = PickupType.GEM_BLUE;
            return;
        }
        if (!crownsInPlay.Contains(PickupType.CROWN_GREEN)) {
            crown = PickupType.CROWN_GREEN;
            cost = PickupType.GEM_GREEN;
            return;
        }
        if (!crownsInPlay.Contains(PickupType.CROWN_BATTLE)) {
            crown = PickupType.CROWN_BATTLE;
            cost = (PickupType)Random.Range(1, 4);
            costVal = 50;
            return;
        }
    }

}

public enum LevelType {
    DEFAULT, RED, BLUE, GREEN, CROWN, SHOP,
}
