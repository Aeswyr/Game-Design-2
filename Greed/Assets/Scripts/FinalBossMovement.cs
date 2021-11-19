using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossMovement : MonoBehaviour
{
    [SerializeField] private int speedX;
    private int speedY;
    private int directionX;
    private int directionY;
    private Vector2 speed;
    private Vector2 direction;
    private Vector2 movement;
    private bool collided = false;
    [SerializeField] private Rigidbody2D rbody;
 
     void Start () {
         //speedY = Random.Range (1,);
         directionX = Random.Range (-1, 1);
         //directionY = Random.Range (-1, 1)
         speed = new Vector2 (speedX, speedY);
         direction = new Vector2 (directionX, directionY);
     }
 
     void FixedUpdate() {
        rbody.velocity = new Vector2 (speed.x * direction.x, speed.y * direction.y);
        //RaycastHit2D l = Utils.Raycast()
     }
 
 } 

