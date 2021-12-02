using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossMovement : MonoBehaviour
{
    [SerializeField] private int speedX;
    [SerializeField] private GameObject punchPrefab;
    [SerializeField] private float PunchDelay;
    [SerializeField] private LayerMask mask;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Collider2D hurtBox;
    private float punchTime;
    private int speedY;
    private int directionX;
    private int directionY;
    
   
    private Vector2 movement;
    private bool collided = false;
    [SerializeField] private Rigidbody2D rbody;
 
     void Start () {
         //speedY = Random.Range (1,);
         directionX = 1;
         //directionY = Random.Range (-1, 1)
        
         
     }
 
     void FixedUpdate() {
        rbody.velocity = new Vector2 (speedX * directionX, rbody.velocity.y); 
        RaycastHit2D l = Utils.Raycast(transform.position, Vector2.left, 12, mask); 
        RaycastHit2D r = Utils.Raycast(transform.position, Vector2.right, 12, mask);
        if((l && directionX < 0) || (r && directionX > 0)){
            Debug.Log("AHHHHHHHHHH");
            directionX *= -1;   

        } 
        if(Time.time >= punchTime + PunchDelay){ 
            punchTime = Time.time;
            GameObject G = Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);
            G.GetComponent<Projectile>().Init(new Vector2(directionX, 0), hurtBox); 
            /*GameObject G = Instantiate(punchPrefab, transform); 
            G.transform.localPosition = new Vector3(10, 0, 0);*/
        }
     } 
 
 }  

