using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rbody;
    private InputManager input;
    [SerializeField] private Animator animator;
    private CharacterData characterData;
    
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

//Wall cling/climb
    [SerializeField] private Vector2 clingCastOffset;
    [SerializeField] private float clingCastDistance;
    [SerializeField] private LayerMask wallDetectMask;
    [SerializeField] private float wallJumpTime;
    [SerializeField] private float regrabTime;
    [SerializeField] private float wallSlideMod;
    private float wallHangTime;
    private float regrab;
    bool clinging;

//Attacking
    [SerializeField] private GameObject attackBoxPrefab;
    private GameObject attackBox;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRecovery;

    [SerializeField] private float attackDrag;
    private bool attacking = false;
    private float inputLockout;
    private bool inputToggle;

//Slide
    private bool sliding = false;
    [SerializeField] private float slideSpeed;

//Inventory
    [SerializeField] private InventoryManager inventoryManager;
    private List<PickupType> inventory = new List<PickupType>();

//Crown Inventory
    [SerializeField] private CrownInventoryManager crownInventoryManager;
    [SerializeField] private int crownDropThreshold;
    private List<PickupType> crownInventory = new List<PickupType>();
    private int battleCrownHits = 3;

//Drops
    [SerializeField] private GameObject gPickupPrefab;
    [SerializeField] private GameObject fPickupPrefab;
    private int gems_blue, gems_green, gems_red;

//Attacks
    [SerializeField] private GameObject crystalDart;

//Stamina
    [SerializeField] private StaminaHintController staminaHint;
    public readonly static int STAMINA_COST = 100;
    public readonly static int MAX_STAMINA = 200;
    private int stamina = MAX_STAMINA;

//Other
    [SerializeField] private BarController stunTimer;


    bool grounded;

    [SerializeField] private Collider2D hurtbox;
    [SerializeField] private Collider2D pickupbox;

    [SerializeField] private GameObject crown;

    private bool collidersLocked = false;
    private float colliderLockout;

    private float gravity;

// ID
    static int id_source = 0;
    int id;

    // Start is called before the first frame update
    void Start()
    {
        id = id_source;
        id_source++;

        gravity = rbody.gravityScale;

        PlayerManager pmanager = FindObjectsOfType<PlayerManager>()[0];

        pmanager.RegisterPlayer(gameObject);

        animator.runtimeAnimatorController = characterData.animator;
        inventoryManager.Display(inventory);
        crownInventoryManager.Display(crownInventory);

        GameObject ic = Instantiate(infoCardPrefab, pmanager.GetInfoHolder().transform);
        infoCard = ic.GetComponent<PlayerInfoManager>();

        infoCard.PushGemCount(gems_red, PickupType.GEM_RED);
        infoCard.PushGemCount(gems_blue, PickupType.GEM_BLUE);
        infoCard.PushGemCount(gems_green, PickupType.GEM_GREEN);

        infoCard.PushPortraitSprite(characterData.sprite);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = groundCheck.CheckGrounded();

        clinging = Utils.Raycast(   transform.position + 
                                    new Vector3(facingModifier * clingCastOffset.x, clingCastOffset.y, 0),
                                    new Vector2(facingModifier, 0),
                                    clingCastDistance,
                                    wallDetectMask) &&
                                    input.B_Held &&
                                    stamina > 0;

        rbody.gravityScale = gravity;

        if (!InputLocked())
            ManageInputs();

        CheckColliderLockout();

        if (grounded && stamina <= MAX_STAMINA)
            stamina += 2;
        
        AdjustStamina();
        input.NextInputFrame();
    }

    public void ManageInputs() {
        
        rbody.velocity = new Vector3(input.Dir.x * speed, rbody.velocity.y, 0);

        animator.SetBool("running", grounded && Mathf.Abs(input.Dir.x) > 0);

        SetFacing(input.Dir.x);

        if (clinging && Time.time >= regrab) {
            rbody.gravityScale = 0;
            if (input.Dir.y > 0)
                stamina--;
            float climbSpeed = input.Dir.y * speed;
            if (input.Dir.y < 0)
                climbSpeed *= wallSlideMod;
            if (Time.time < regrab)
                climbSpeed = rbody.velocity.y;
            rbody.velocity = new Vector3(rbody.velocity.x, climbSpeed, 0);
            wallHangTime = Time.time + wallJumpTime;
        }
        animator.SetBool("clinging", clinging);

        if (grounded || clinging) {
            jumps = JUMPS_MAX;
        }
        if (input.A && (grounded || (wallHangTime > Time.time  && stamina >= STAMINA_COST) || jumps > 0)) {
            rbody.velocity = new Vector3(rbody.velocity.x, jumpVelocity, 0);
            if (!grounded || wallHangTime <= Time.time)
                jumps--;
            if (wallHangTime > Time.time) {
                regrab = Time.time + regrabTime;
                stamina -= STAMINA_COST;
            }
        }
        if (input.X && !attacking) {
            animator.SetTrigger("attack");
        }
        if (input.Y) {
            TryUseItem();
        }
        if (input.B && !sliding && grounded && stamina >= MAX_STAMINA) {
            animator.SetTrigger("slide");
            stamina -= MAX_STAMINA;
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
        
        attacking = true;
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
        attacking = false;
    }

    private void StartSlide() {
        sliding = true;
        hurtbox.enabled = false;
        rbody.velocity = facingModifier * new Vector2(slideSpeed, 0);
        InputLockout(true);
    }

    private void EndSlide() {
        sliding = false;
        hurtbox.enabled = true;
        rbody.velocity = Vector2.zero;
        InputLockout(false);
    }

    private void SetFacing(float dir) {
        if (dir == 0)
            return;
        if (facingModifier != (int)(dir / Mathf.Abs(dir))) {
            facingModifier = (int)(dir / Mathf.Abs(dir));
            Vector3 newScale = new Vector3(facingModifier, transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;
            inventoryManager.transform.localScale = newScale;
            inventoryManager.transform.localPosition = new Vector3(inventoryManager.transform.localPosition.x * -1, inventoryManager.transform.localPosition.y, 0);
            crownInventoryManager.transform.localScale = newScale;
            crownInventoryManager.transform.localPosition = new Vector3(crownInventoryManager.transform.localPosition.x * -1, crownInventoryManager.transform.localPosition.y, 0);
            staminaHint.transform.localScale = newScale;
        }
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

        TryDropCrowns();
        TryDropGems();
    }

    private void TryEnableCrown(PickupType type) {
        if (type == PickupType.CROWN_BLUE) {

        } else if (type == PickupType.CROWN_RED) {

        } else if (type == PickupType.CROWN_GREEN) {

        } else if (type == PickupType.CROWN_BATTLE) {

        }
    }

    private void TryDisableCrown(PickupType type) {
        if (type == PickupType.CROWN_BLUE) {

        } else if (type == PickupType.CROWN_RED) {

        } else if (type == PickupType.CROWN_GREEN) {

        } else if (type == PickupType.CROWN_BATTLE) {

        }
    }

    private void TryDropGems() {
        if (gems_red != 0) {
            int drop = gems_red / 2 + gems_red % 2;
            for (int i = 0; i < drop / 10; i++)
                DropGravPickup(PickupType.GEM_RED_LARGE);
            for (int i = 0; i < drop % 10; i++)
                DropGravPickup(PickupType.GEM_RED);
            gems_red -= drop;
            infoCard.PushGemCount(gems_red, PickupType.GEM_RED);
        }
        if (gems_blue != 0) {
            int drop = gems_blue / 2 + gems_blue % 2;
            for (int i = 0; i < drop / 10; i++)
                DropGravPickup(PickupType.GEM_BLUE_LARGE);
            for (int i = 0; i < drop % 10; i++)
                DropGravPickup(PickupType.GEM_BLUE);
            gems_blue -= drop;
            infoCard.PushGemCount(gems_blue, PickupType.GEM_BLUE);
        }
        if (gems_green != 0) {
            int drop = gems_green / 2 + gems_green % 2;
            for (int i = 0; i < drop / 10; i++)
                DropGravPickup(PickupType.GEM_GREEN_LARGE);
            for (int i = 0; i < drop % 10; i++)
                DropGravPickup(PickupType.GEM_GREEN);
            gems_green -= drop;
            infoCard.PushGemCount(gems_green, PickupType.GEM_GREEN);
        }
    }

    private void DropGravPickup(PickupType type) {
        GameObject newGem = Instantiate(gPickupPrefab, transform.position, gPickupPrefab.transform.rotation);
        newGem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(20f, 40f)), ForceMode2D.Impulse);
        newGem.GetComponent<ItemPickup>().SetType(type);
    }

    private void DropFloatPickup(PickupType type) {
        GameObject crown = Instantiate(fPickupPrefab, transform.position, fPickupPrefab.transform.rotation);
        crown.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), Random.Range(20f, 40f)), ForceMode2D.Impulse);
        crown.GetComponent<ItemPickup>().SetType(type);
    }

    private void TryDropCrowns() {
        if (crownInventory.Contains(PickupType.CROWN_BATTLE)) {
            battleCrownHits--;
            if (battleCrownHits == 0) {
                battleCrownHits = 3;
                
                DropFloatPickup(PickupType.CROWN_BATTLE);
                
                crownInventory.Remove(PickupType.CROWN_BATTLE);
                crownInventoryManager.Display(crownInventory);
                TryDisableCrown(PickupType.CROWN_BATTLE);
            }
        }

        if (crownInventory.Contains(PickupType.CROWN_RED) && gems_red < crownDropThreshold) {
                DropFloatPickup(PickupType.CROWN_RED);

                crownInventory.Remove(PickupType.CROWN_RED);
                crownInventoryManager.Display(crownInventory);
                TryDisableCrown(PickupType.CROWN_RED);
        }

        if (crownInventory.Contains(PickupType.CROWN_GREEN) && gems_green < crownDropThreshold) {
                DropFloatPickup(PickupType.CROWN_GREEN);

                crownInventory.Remove(PickupType.CROWN_GREEN);
                crownInventoryManager.Display(crownInventory);
                TryDisableCrown(PickupType.CROWN_GREEN);
        }

        if (crownInventory.Contains(PickupType.CROWN_BLUE) && gems_blue < crownDropThreshold) {
                DropFloatPickup(PickupType.CROWN_BLUE);

                crownInventory.Remove(PickupType.CROWN_BLUE);
                crownInventoryManager.Display(crownInventory);
                TryDisableCrown(PickupType.CROWN_BLUE);
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
        } else if (type == PickupType.CROWN_BLUE || type == PickupType.CROWN_GREEN || type == PickupType.CROWN_BATTLE
                || type == PickupType.CROWN_RED) {
                    if (crownInventory.Count >= CrownInventoryManager.INVENTORY_SIZE)
                        return false;
                    crownInventory.Add(type);
                    crownInventoryManager.Display(crownInventory);
                    TryEnableCrown(type);
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

    private void AdjustStamina() {
        infoCard.PushStamina(stamina);
        staminaHint.PushStamina(stamina);
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

    public void BindInputs(InputManager inputs) {
        input = inputs;
    }

    public void SetCharacterData(CharacterData data) {
        characterData = data;
    }

    public void RemoveUI() {
        Destroy(infoCard.gameObject);
    }
}
