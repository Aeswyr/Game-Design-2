using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheck : MonoBehaviour
{
   //Grounded Raycast
    [SerializeField] private float floorDetectDistance = 1f;
    [SerializeField] private Vector2 floorDetectOffset;
    [SerializeField] private Vector2 footOffset;
    [SerializeField] private LayerMask floorDetectMask;

    public bool CheckGrounded() {

        RaycastHit2D left = Raycast(transform.position + (Vector3)(floorDetectOffset + footOffset), Vector2.down, floorDetectDistance, floorDetectMask);
        RaycastHit2D right = Raycast(transform.position + (Vector3)(floorDetectOffset - footOffset), Vector2.down, floorDetectDistance, floorDetectMask);
        return left || right;
    }

    private RaycastHit2D Raycast(Vector3 start, Vector2 dir, float dist, LayerMask mask) {
        RaycastHit2D hit = Physics2D.Raycast(start, dir, dist, mask);
        Debug.DrawRay(start, dir * dist, Color.green);
        return hit;
    }
}
