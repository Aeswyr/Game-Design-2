using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossMovement : MonoBehaviour
{
    [SerializeField] private int speedX;
    [SerializeField] private GameObject punchPrefab;
    [SerializeField] private float attackDelay;
    [SerializeField] private LayerMask mask;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject slamPrefab;
    [SerializeField] private Collider2D hurtBox;
    [SerializeField] private Animator animator;
    private float attackTime;
    private int directionX;
    [SerializeField] private Rigidbody2D rbody;
 
     void Start () {
         directionX = 1;
        
         
     }
 
     void FixedUpdate() {

        CheckPaused();

        if(Time.time >= attackTime + attackDelay){ 
            attackTime = Time.time;
            int val = Random.Range(0, 3);
            switch(val) {
                case 0:
                    animator.SetTrigger("slam");
                    break;
                case 1:
                    animator.SetTrigger("ranged");
                    break;
                case 2:
                    animator.SetTrigger("melee");
                    break;
            }
        }
     }

    public void Hurt() {
        Pause(0.2f);
        EffectsMaster.Instance.ScreenShake(0.5f, 0.2f);
    }

    public void Melee() {
        GameObject G = Instantiate(punchPrefab, transform); 
        G.transform.localPosition = new Vector3(8, -3, 0);
    }

    public void Ranged() {
        GameObject G = Instantiate(projectilePrefab, transform.position + new Vector3(0, -6.3f, 0), projectilePrefab.transform.rotation);
        G.GetComponent<Projectile>().Init(new Vector2(directionX, 0), hurtBox); 
    }

    public void Slam() {
        GameObject G = Instantiate(punchPrefab, transform); 
        G.transform.localPosition = new Vector3(0, -3, 0);
        EffectsMaster.Instance.ScreenShake(1f, 0.2f);

        int spikes = Random.Range(3, 6);
        int xoffset = 0;
        int dist = 16;
        int yoffset = 0;
        switch (spikes) {
            case 3:
                xoffset = -16;
                yoffset = 15;
                break;
            case 4:
                xoffset = -24;
                yoffset = 23;
                break;
            case 5:
                xoffset = -32;
                yoffset = 23;
                break;
        }
        for (int i = 0; i < spikes; i++) {
            GameObject g = Instantiate(slamPrefab, new Vector3(xoffset + i * dist, yoffset, 0), slamPrefab.transform.rotation);
            g.GetComponent<Projectile>().Init(new Vector2(1, 0), hurtBox); 
        }
    }

    public void Move() {
        rbody.velocity = new Vector2 (speedX * directionX, rbody.velocity.y); 
        RaycastHit2D l = Utils.Raycast(transform.position, Vector2.left, 12, mask); 
        RaycastHit2D r = Utils.Raycast(transform.position, Vector2.right, 12, mask);
        if((l && directionX < 0) || (r && directionX > 0)){
            directionX *= -1;
            Vector3 scale = transform.localScale;
            scale.x = directionX;
            transform.localScale = scale;
        } 
    }

    bool paused = false;
    float pauseTime;
    Vector2 pauseVelocity;
    public void Pause(float duration) {
        pauseTime = Time.time + duration;
        paused = true;
        pauseVelocity = rbody.velocity;
        rbody.velocity = Vector2.zero;
        animator.speed = 0;
    }

    private void CheckPaused() {
        if (paused && Time.time >= pauseTime) {
            paused = false;
            rbody.velocity = pauseVelocity;
            animator.speed = 1;
        }
    }

 
 }  

