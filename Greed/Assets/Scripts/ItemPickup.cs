using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private GroundedCheck groundCheck;
    [SerializeField] private float pickupLockout;
    private float startTime;
    void Start()
    {
        startTime = Time.time;
    }

    public bool CanPickup() {
        return /*(groundCheck == null || groundCheck.CheckGrounded()) 
        &&*/ (pickupLockout == 0 || Time.time >= startTime + pickupLockout);
    }
}
