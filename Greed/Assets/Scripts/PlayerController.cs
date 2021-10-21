using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private InputManager input;

//movement
    [SerializeField] private float speed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private int jumps = 1;
    [SerializeField] private int JUMPS_MAX = 1;

//Grounded Raycast
    [SerializeField] private float floorDetectDistance = 1f;
    [SerializeField] private Vector2 floorDetectOffset;
    [SerializeField] private Vector2 footOffset;
    [SerializeField] private LayerMask floorDetectMask;

//Attacking
    [SerializeField] private GameObject attackBox;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRecovery;

    [SerializeField] private float attackDrag;
    private float attackTime;
    private float inputLockout;

//Other
    [SerializeField] BarController stunTimer;


    bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = CheckGrounded();
        if (Time.time > inputLockout)
            ManageInputs();
        
        CheckAttack();
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
        rbody.velocity = 0.75f * rbody.velocity;
        rbody.drag = attackDrag;
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

        transform.localScale = new Vector3(dir / Mathf.Abs(dir), transform.localScale.y, transform.localScale.z);
    }

    public void OnHit() {
        Debug.Log("Yeouch!");
        rbody.velocity = Vector3.zero;
        InputLockout(2);
        stunTimer.StartTimer(2);
    }

    public void InputLockout(float duration) {
        inputLockout = Time.time + duration;
    }

    private bool CheckGrounded() {

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
