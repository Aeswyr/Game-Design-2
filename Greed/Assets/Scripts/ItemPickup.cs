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
        spriteRenderer.sprite = pickupSprite.GetSprite(type);
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
        spriteRenderer.sprite = pickupSprite.GetSprite(type);
    }

    public void SetType(PickupType type, int amount) {
        this.amount = amount;
        this.type = type;
        spriteRenderer.sprite = pickupSprite.GetSprite(type);
    }
}

public enum PickupType {
    DEFAULT, GEM_BLUE, GEM_RED, GEM_GREEN, DART,
}
