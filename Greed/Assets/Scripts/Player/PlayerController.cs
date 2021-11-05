using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private InputManager input;

    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAnimationAtlas animationAtlas;
    [SerializeField] private SpriteAtlas portraitAtlas;
    
// Info Card  
    [SerializeField] private GameObject infoCardPrefab;

    private PlayerInfoManager infoCard;

//movement
    [SerializeField] private float speed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private int jumps = 1;
    [SerializeField] private int JUMPS_MAX = 1;

    private int facingModifier = 1;

    [SerializeField] GroundedCheck groundCheck;

//Attacking
    [SerializeField] private GameObject attackBoxPrefab;
    private GameObject attackBox;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRecovery;

    [SerializeField] private float attackDrag;
    private float attackTime;
    private float inputLockout;
    private bool inputToggle;

//Inventory
    [SerializeField] private InventoryManager inventoryManager;
    private List<PickupType> inventory = new List<PickupType>();

//Gems
    [SerializeField] private GameObject gem;
    private int gems_blue, gems_green, gems_red;

//Attacks
    [SerializeField] private GameObject crystalDart;

//Other
    [SerializeField] private BarController stunTimer;


    bool grounded;

    [SerializeField] private Collider2D hurtbox;
    [SerializeField] private Collider2D pickupbox;

    [SerializeField] private GameObject crown;

    private bool collidersLocked = false;
    private float colliderLockout;

// ID
    static int id_source = 0;
    int id;

    // Start is called before the first frame update
    void Start()
    {
        id = id_source;
        id_source++;

        PlayerManager pmanager = FindObjectsOfType<PlayerManager>()[0];

        pmanager.RegisterPlayer(gameObject);

        animator.runtimeAnimatorController = animationAtlas.GetAnimator(id);
        inventoryManager.Display(inventory);

        GameObject ic = Instantiate(infoCardPrefab, pmanager.GetInfoHolder().transform);
        infoCard = ic.GetComponent<PlayerInfoManager>();

        infoCard.PushGemCount(gems_red, PickupType.GEM_RED);
        infoCard.PushGemCount(gems_blue, PickupType.GEM_BLUE);
        infoCard.PushGemCount(gems_green, PickupType.GEM_GREEN);

        infoCard.PushPortraitSprite(portraitAtlas.GetSprite(id));
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
        if (input.Y) {
            TryUseItem();
        }
        if (input.B) {
            
        }
    }

    private void TryUseItem() {
        if (inventory.Count > 0) {
                switch (inventory[0]) {
                    case PickupType.DART:
                        GameObject dart = Instantiate(crystalDart);
                        dart.GetComponent<CrystalDart>().Init(facingModifier, hurtbox);
                        dart.transform.position = transform.position;
                        break;
                    default:
                        break;
                }
            inventory.RemoveAt(0);
            inventoryManager.Display(inventory);
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
        attackBox = Instantiate(attackBoxPrefab, transform);
        attackBox.transform.localPosition = new Vector3(2, 0.5f, 0);
    }

    private void EndHit() {
        Destroy(attackBox);
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
        facingModifier = (int)(dir / Mathf.Abs(dir));
        Vector3 newScale = new Vector3(facingModifier, transform.localScale.y, transform.localScale.z);
        transform.localScale = newScale;
        inventoryManager.transform.localScale = newScale;
    }

    public void EnableDoubleJump() {
        crown.SetActive(true);
        jumps = 1;
        JUMPS_MAX = 1;
    }

    public void DisableDoubleJump() {
        crown.SetActive(false);
        jumps = 0;
        JUMPS_MAX = 0;
    }

    public void OnHit() {
        Debug.Log("Yeouch!");
        rbody.velocity = Vector3.zero;
        InputLockout(2);
        stunTimer.StartTimer(2);

        ColliderLockout(3);

        if (gems_red != 0) {
            int drop = gems_red / 2 + gems_red % 2;
            for (int i = 0; i < drop / 10; i++) {
                GameObject newGem = Instantiate(gem, transform.position, gem.transform.rotation);
                newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(20f, 40f)), ForceMode2D.Impulse);
                newGem.GetComponent<ItemPickup>().SetType(PickupType.GEM_RED_LARGE);
            }
                for (int i = 0; i < drop % 10; i++) {
                GameObject newGem = Instantiate(gem, transform.position, gem.transform.rotation);
                newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(20f, 40f)), ForceMode2D.Impulse);
                newGem.GetComponent<ItemPickup>().SetType(PickupType.GEM_RED);
            }
            gems_red -= drop;
            infoCard.PushGemCount(gems_red, PickupType.GEM_RED);
        }
        if (gems_blue != 0) {
            int drop = gems_blue / 2 + gems_blue % 2;
            for (int i = 0; i < drop / 10; i++) {
                GameObject newGem = Instantiate(gem, transform.position, gem.transform.rotation);
                newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(20f, 40f)), ForceMode2D.Impulse);
                newGem.GetComponent<ItemPickup>().SetType(PickupType.GEM_BLUE_LARGE);
            }
                for (int i = 0; i < drop % 10; i++) {
                GameObject newGem = Instantiate(gem, transform.position, gem.transform.rotation);
                newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(20f, 40f)), ForceMode2D.Impulse);
                newGem.GetComponent<ItemPickup>().SetType(PickupType.GEM_BLUE);
            }
            gems_blue -= drop;
            infoCard.PushGemCount(gems_blue, PickupType.GEM_BLUE);
        }
        if (gems_green != 0) {
            int drop = gems_green / 2 + gems_green % 2;
            for (int i = 0; i < drop / 10; i++) {
                GameObject newGem = Instantiate(gem, transform.position, gem.transform.rotation);
                newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(20f, 40f)), ForceMode2D.Impulse);
                newGem.GetComponent<ItemPickup>().SetType(PickupType.GEM_GREEN_LARGE);
            }
                for (int i = 0; i < drop % 10; i++) {
                GameObject newGem = Instantiate(gem, transform.position, gem.transform.rotation);
                newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(20f, 40f)), ForceMode2D.Impulse);
                newGem.GetComponent<ItemPickup>().SetType(PickupType.GEM_GREEN);
            }
            gems_green -= drop;
            infoCard.PushGemCount(gems_green, PickupType.GEM_GREEN);
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

    public bool AddItem(PickupType type, int amount = 1) {
        if (type == PickupType.GEM_BLUE || type == PickupType.GEM_RED || type == PickupType.GEM_GREEN
        || type == PickupType.GEM_RED_LARGE || type == PickupType.GEM_BLUE_LARGE || type == PickupType.GEM_GREEN_LARGE) {
            switch(type) {
                case PickupType.GEM_RED:
                case PickupType.GEM_RED_LARGE:
                    gems_red += amount;
                    infoCard.PushGemCount(gems_red, PickupType.GEM_RED);
                    break;
                case PickupType.GEM_BLUE:
                case PickupType.GEM_BLUE_LARGE:
                    gems_blue += amount;
                    infoCard.PushGemCount(gems_blue, PickupType.GEM_BLUE);
                    break;
                case PickupType.GEM_GREEN:
                case PickupType.GEM_GREEN_LARGE:
                    gems_green += amount;
                    infoCard.PushGemCount(gems_green, PickupType.GEM_GREEN);
                    break;
                default:
                break;
            }
            return true;
        } else {
            if (inventory.Count >= InventoryManager.INVENTORY_SIZE)
                return false;
            for (int i = 0; i < amount; i++){
                    inventory.Add(type);
                    if (inventory.Count >= InventoryManager.INVENTORY_SIZE)
                        break;
            }
            inventoryManager.Display(inventory);
            return true;
        }
    }

    public void ColliderLockout(float duration) {
        collidersLocked = true;
        colliderLockout = Time.time + duration;

        hurtbox.enabled = false;
        pickupbox.enabled = false;
    }

    private void CheckColliderLockout() {
        if (collidersLocked && Time.time >= colliderLockout) {
            collidersLocked = false;

            hurtbox.enabled = true;
            pickupbox.enabled =true;
        }
    }




}
