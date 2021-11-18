using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossMovement : MonoBehaviour
{public int speedX;
     public int speedY;
     public int directionX;
     public int directionY;
     public Vector2 speed;
     public Vector2 direction;
     public Vector2 movement;
     public bool collided = false;
 
     void Start () {
         speedX = Random.Range (1, 10);
         //speedY = Random.Range (1,);
         directionX = Random.Range (-1, 1);
         //directionY = Random.Range (-1, 1)
         speed = new Vector2 (speedX, speedY);
         direction = new Vector2 (directionX, directionY);
     }
 
     void Update () {
         movement = new Vector2 (speed.x * direction.x, speed.y * direction.y);
 
     }
 
     void FixedUpdate() {
         GetComponent<Rigidbody2D>().velocity = movement;
 
     }
 
 } 

