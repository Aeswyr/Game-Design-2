using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private PickupType type;
    [SerializeField] private int amount;
    [SerializeField] private GroundedCheck groundCheck;
    [SerializeField] private float pickupLockout;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] private ItemIconAtlas pickupSprite;

    private float startTime;
    void Start()
    {
        startTime = Time.time;
        SetType(type);
    }

    public bool CanPickup() {
        return /*(groundCheck == null || groundCheck.CheckGrounded()) 
        &&*/ (pickupLockout == 0 || Time.time >= startTime + pickupLockout);
    }

    public PickupType GetPickup() {
        return type;
    }

    public int GetAmount() {
        return amount;
    }

    public void SetType(PickupType type) {
        this.type = type;
        if (type == PickupType.GEM_RED_LARGE || type == PickupType.GEM_BLUE_LARGE || type == PickupType.GEM_GREEN_LARGE)
            amount = 10;
        spriteRenderer.sprite = pickupSprite.GetSprite(type);
    }
}

public enum PickupType {
    DEFAULT, GEM_BLUE, GEM_RED, GEM_GREEN, 
    GEM_BLUE_LARGE, GEM_GREEN_LARGE, GEM_RED_LARGE, 
    CROWN_RED, CROWN_BLUE, CROWN_GREEN, CROWN_BATTLE, CROWN_BONUS, 
    DRILL, BOMB, DART, TURRET, MAGNET, ARMOR, SPEED, JUICE, JUMP, DASH, GROUND, TREASURE
}
