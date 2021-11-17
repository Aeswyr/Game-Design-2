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

//Items
    [SerializeField] private GameObject drill;
    [SerializeField] private GameObject bomb;
    [SerializeField] private GameObject dartStorm;
    [SerializeField] private GameObject turret;

    [SerializeField] private float speedDuration;
    [SerializeField] private float fastSpeed;
    private float speedTime;
    private bool speedActive = false;
    [SerializeField] private float magnetDuration;
    [SerializeField] private float magnetSize;
    [SerializeField] private float pickupSize;
    [SerializeField] private GameObject magnetRender;
    private bool magnetActive = false;
    private float magnetTime;
    [SerializeField] private float juiceDuration;
    private float juiceTime;
    private bool juiceActive;
    [SerializeField] private int armorHits;
    [SerializeField] private Animator armorAnimator;
    private int armor;
    [SerializeField] private GameObject pushBoxPrefab;
    private bool groundPoundEnabled = false;
    [SerializeField] private LayerMask blinkMask;

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
        
        CheckBuffs();

        if (groundPoundEnabled && (grounded || clinging)) {
            GemBurst(17);
            ColliderLockout(0.25f);
            PushBack();
            groundPoundEnabled = false;
        }

        CornerCheck();

        AdjustStamina();
        input.NextInputFrame();
    }

    public void ManageInputs() {
        float speedMod = speed;
        if (speedActive)
            speedMod = fastSpeed;
        rbody.velocity = new Vector3(input.Dir.x * speedMod, rbody.velocity.y, 0);

        animator.SetBool("running", grounded && Mathf.Abs(input.Dir.x) > 0);

        SetFacing(input.Dir.x);

        if (clinging && Time.time >= regrab) {
            rbody.gravityScale = 0;
            if (input.Dir.y > 0 && !juiceActive)
                stamina--;
            float climbSpeed = input.Dir.y * speedMod;
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
        if (input.A && (grounded || (wallHangTime > Time.time  && (stamina >= STAMINA_COST || juiceActive)) || jumps > 0)) {
            rbody.velocity = new Vector3(rbody.velocity.x, jumpVelocity, 0);
            if (!grounded || wallHangTime <= Time.time)
                jumps--;
            if (wallHangTime > Time.time) {
                regrab = Time.time + regrabTime;
                if (!juiceActive)
                    stamina -= STAMINA_COST;
            }
        }
        if (input.X && !attacking) {
            animator.SetTrigger("attack");
        }
        if (input.Y) {
            TryUseItem();
        }
        if (input.B && !sliding && grounded && (stamina >= MAX_STAMINA || juiceActive)) {
            animator.SetTrigger("slide");
            if (!juiceActive)
                stamina -= MAX_STAMINA;
        }
    }

    private void CheckBuffs() {
        if (magnetActive && Time.time > magnetTime) {
            EndMagnet();
        }
        if (juiceActive && Time.time > juiceTime) {
            EndJuice();
        }
        if (speedActive && Time.time > speedTime) {
            EndSpeed();
        }
    }

    private void StartMagnet() {
        ((CircleCollider2D)pickupbox).radius = magnetSize;
        magnetActive = true;
        magnetTime = Time.time + magnetDuration;
        magnetRender.SetActive(true);
    }

    private void EndMagnet() {
        ((CircleCollider2D)pickupbox).radius = pickupSize;
        magnetActive = false;
        magnetRender.SetActive(false);
    }

    private void StartSpeed() {
        speedActive = true;
        speedTime = Time.time + speedDuration;
    }

    private void EndSpeed() {
        speedActive = false;
    }

    private void StartJuice() {
        juiceActive = true;
        juiceTime = Time.time + juiceDuration;
    }

    private void EndJuice() {
        juiceActive = false;
    }

    private void SetArmor() {
        armorAnimator.gameObject.SetActive(true);
        armorAnimator.SetTrigger("start");
        armor = armorHits;
    }
    private void TryUseItem() {
        if (inventory.Count > 0) {
                switch (inventory[0]) {
                    case PickupType.DRILL:
                        GameObject dart1 = Instantiate(drill);
                        dart1.GetComponent<Projectile>().Init(input.Dir, hurtbox);
                        dart1.transform.position = transform.position;
                        break;
                    case PickupType.BOMB:
                        GameObject dart2 = Instantiate(bomb);
                        dart2.GetComponent<Projectile>().Init(input.Dir, hurtbox);
                        dart2.transform.position = transform.position;
                        break;
                    case PickupType.DART:
                        GameObject dart3 = Instantiate(dartStorm);
                        dart3.GetComponent<DartStorm>().Init(input.Dir, hurtbox);
                        dart3.transform.position = transform.position;
                        break;
                    case PickupType.TURRET:
                        GameObject dart4 = Instantiate(turret);
                        dart4.GetComponent<Turret>().Init(hurtbox);
                        dart4.transform.position = transform.position;
                        break;
                    case PickupType.ARMOR:
                        SetArmor();
                        break;
                    case PickupType.JUICE:
                        StartJuice();
                        break;
                    case PickupType.MAGNET:
                        StartMagnet();
                        break;
                    case PickupType.SPEED:
                        StartSpeed();
                        break;
                    case PickupType.JUMP:
                        rbody.velocity = new Vector3(rbody.velocity.x, jumpVelocity * 1.3f, 0);
                        ColliderLockout(0.25f);
                        GemBurst(18);
                        PushBack();
                        break;
                    case PickupType.GROUND:
                        rbody.velocity = new Vector3(rbody.velocity.x, jumpVelocity * -1.5f, 0);
                        ColliderLockout(0.25f);
                        GemBurst(6);
                        groundPoundEnabled = true;
                        break;
                    case PickupType.DASH:
                        StartBlink();
                        ColliderLockout(0.25f);
                        GemBurst(18);
                        PushBack();
                        break;
                    case PickupType.TREASURE:
                        ColliderLockout(0.25f);
                        GemBurst(59);
                        PushBack();
                        break;
                    default:
                        break;
                }
            inventory.RemoveAt(0);
            inventoryManager.Display(inventory);
        }
    }

    private void StartBlink() { 
        Vector2 dir = input.Dir;
        dir.Normalize();
        transform.Translate(10f * dir);

        for (int i = 0; i < 20; i++) {
            if (CornerCheck())
                break;
            transform.Translate(-0.5f * dir);
        }

        if (rbody.velocity.y < 10)
            rbody.velocity = new Vector2(rbody.velocity.x, 10);
            
    }

    private bool CornerCheck() {
        RaycastHit2D tr = Utils.Raycast(transform.position, new Vector2(1f, 1.5f), 1f, blinkMask);
        RaycastHit2D tl = Utils.Raycast(transform.position, new Vector2(-1f, 1.5f), 1f, blinkMask);
        RaycastHit2D br = Utils.Raycast(transform.position, new Vector2(1f, -1.5f), 1f, blinkMask);
        RaycastHit2D bl = Utils.Raycast(transform.position, new Vector2(-1f, -1.5f), 1f, blinkMask);
        return !(tr || tl || br || bl);
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
        if (armor > 0) {
            armorAnimator.SetTrigger("hit");
            armor--;
            if (armor == 0)
                armorAnimator.gameObject.SetActive(false);
            return;
        }

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

    public void GemBurst(int drop) {
        for (int i = 0; i < drop / 10; i++)
            DropGravPickup(PickupType.GEM_GREEN_LARGE);
        for (int i = 0; i < drop % 10; i++)
            DropGravPickup(PickupType.GEM_GREEN);
    }

    public void PushBack() {
        Instantiate(pushBoxPrefab, transform);
    }

    public void ClearVelocity() {
        rbody.velocity = Vector2.zero;
    }

    public void Knockback(Vector2 dir, float magnitude) {
        InputLockout(0.25f);
        dir.Normalize();

        rbody.AddForce(magnitude * dir, ForceMode2D.Impulse);
    }

    public int GetItem(PickupType type) {
        switch(type) {
            case PickupType.GEM_BLUE:
            case PickupType.GEM_BLUE_LARGE:
                return gems_blue;
            case PickupType.GEM_RED:
            case PickupType.GEM_RED_LARGE:
                return gems_red;
            case PickupType.GEM_GREEN:
            case PickupType.GEM_GREEN_LARGE:
                return gems_green;
            default:
                return -1;
        }
    }

    public bool IsHurtboxOwner(Collider2D other) {
        return other == hurtbox;
    }
    public bool RemoveItem(PickupType type, int amount) {
        switch(type) {
            case PickupType.GEM_BLUE:
            case PickupType.GEM_BLUE_LARGE:
                if (gems_blue >= amount) {
                    gems_blue -= amount;
                    infoCard.PushGemCount(gems_blue, PickupType.GEM_BLUE);
                    return true;
                }
                return false;
            case PickupType.GEM_RED:
            case PickupType.GEM_RED_LARGE:
                if (gems_red >= amount) {
                    gems_red -= amount;
                    infoCard.PushGemCount(gems_red, PickupType.GEM_RED);
                    return true;
                }
                return false;
            case PickupType.GEM_GREEN:
            case PickupType.GEM_GREEN_LARGE:
                if (gems_green >= amount) {
                    gems_green -= amount;
                    infoCard.PushGemCount(gems_green, PickupType.GEM_GREEN);
                    return true;
                }
                return false;
            default:
                return false;
        }
    }
}
