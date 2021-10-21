using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private InputManager input;

//movement
    [SerializeField] private float speed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private int jumps = 1;
    [SerializeField] private int JUMPS_MAX = 1;

    [SerializeField] GroundedCheck groundCheck;

//Attacking
    [SerializeField] private GameObject attackBox;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRecovery;

    [SerializeField] private float attackDrag;
    private float attackTime;
    private float inputLockout;

//Other
    [SerializeField] private BarController stunTimer;


    bool grounded;

    [SerializeField] private GameObject gem;
    [SerializeField] private TextMeshPro gemCounter;
    private int gems;


    [SerializeField] private GameObject hurtbox;
    [SerializeField] private GameObject pickupbox;

    private bool collidersLocked = false;
    private float colliderLockout;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = groundCheck.CheckGrounded();
        if (Time.time > inputLockout)
            ManageInputs();
        
        CheckAttack();

        CheckColliderLockout();
    }

    public void ManageInputs() {
        
        rbody.velocity = new Vector3(input.Dir.x * speed, rbody.velocity.y, 0);

        SetFacing(input.Dir.x);

        if (grounded) {
            jumps = JUMPS_MAX;
        }
        if (input.X && (grounded || jumps > 0)) {
            rbody.velocity = new Vector3(rbody.velocity.x, jumpVelocity, 0);
            if (!grounded)
                jumps--;
        }
        if (input.A) {
            if (Time.time > attackTime)
                StartAttack();
        }
        if (input.B) {

        }
    }

    private void StartAttack() {
        if (grounded) {
            rbody.velocity = 0.75f * rbody.velocity;
            rbody.drag = attackDrag;
        }
        attackBox.SetActive(true);
        attackTime = Time.time + attackDuration;
        InputLockout(attackRecovery);
    }

    private void EndAttack() {
        rbody.drag = 0;
        attackBox.SetActive(false);
        attackTime = default;
    }

    private void CheckAttack() {
        if (Time.time >= attackTime)
        EndAttack();
    }

    private void SetFacing(float dir) {
        if (dir == 0)
            return;

        Vector3 newScale = new Vector3(dir / Mathf.Abs(dir), transform.localScale.y, transform.localScale.z);
        transform.localScale = newScale;
        gemCounter.transform.localScale = newScale;
    }

    public void OnHit() {
        Debug.Log("Yeouch!");
        rbody.velocity = Vector3.zero;
        InputLockout(2);
        stunTimer.StartTimer(2);

        ColliderLockout(3);

        if (gems != 0) {
            for (int i = 0; i < gems / 2 + gems % 2; i++) {
                GameObject newGem = Instantiate(gem, transform.position, gem.transform.rotation);
                newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(20f, 40f)), ForceMode2D.Impulse);
            }
            gems -= gems / 2 + gems % 2;
            gemCounter.text = gems.ToString();
        }
    }

    public void InputLockout(float duration) {
        inputLockout = Time.time + duration;
    }

    public void AddGem() {
        gems++;
        gemCounter.text = gems.ToString();
    }

    public void ColliderLockout(float duration) {
        collidersLocked = true;
        colliderLockout = Time.time + duration;

        hurtbox.SetActive(false);
        pickupbox.SetActive(false);
    }

    private void CheckColliderLockout() {
        if (collidersLocked && Time.time >= colliderLockout) {
            collidersLocked = false;

            hurtbox.SetActive(true);
            pickupbox.SetActive(true);
        }
    }




}
