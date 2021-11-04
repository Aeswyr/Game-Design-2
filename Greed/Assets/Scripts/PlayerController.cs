using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private InputManager input;

    [SerializeField] private Animator animator;

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
    private bool inputToggle;

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
        FindObjectsOfType<PlayerManager>()[0].RegisterPlayer(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = groundCheck.CheckGrounded();
        if (!InputLocked())
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
        if (input.A && (grounded || jumps > 0)) {
            rbody.velocity = new Vector3(rbody.velocity.x, jumpVelocity, 0);
            if (!grounded)
                jumps--;
        }
        if (input.X) {
            if (Time.time > attackTime)
                animator.SetTrigger("Attack");
        }
        if (input.B) {

        }
    }

    private void StartAttack() {
        if (grounded) {
            rbody.velocity = 0.75f * rbody.velocity;
            rbody.drag = attackDrag;
        }
        
        attackTime = Time.time + attackDuration;
        InputLockout(true);
        InputLockout(attackRecovery);
    }

    private void StartHit() {
        attackBox.SetActive(true);
    }

    private void EndHit() {
        attackBox.SetActive(false);
    }

    private void EndAttack() {
        rbody.drag = 0;
        InputLockout(false);
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

    public void InputLockout(bool toggle) {
        inputToggle = toggle;
    }

    private bool InputLocked() {
        return inputToggle || Time.time <= inputLockout;
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
