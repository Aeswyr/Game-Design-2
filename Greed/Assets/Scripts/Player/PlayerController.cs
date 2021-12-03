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
    private LevelDirector director;

//movement
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private int jumps = 1;
    [SerializeField] private int JUMPS_MAX = 1;

    private int facingModifier = 1;

    [SerializeField] private GroundedCheck groundCheck;
    [SerializeField] private GameObject dustPrefab;

//Wall cling/climb
    [Header("Wall Climb/Cling")]
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
    [Header("Attacking")]
    [SerializeField] private GameObject attackBoxPrefab;
    private GameObject attackBox;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRecovery;

    [SerializeField] private float attackDrag;
    private bool attacking = false;
    private float inputLockout;
    private bool inputToggle;

//Interact
    [Header("Interacting")]
    [SerializeField] private GameObject interactBoxPrefab;
    [SerializeField] private GameObject interactHint;

//Slide
    [Header("Sliding")]
    private bool sliding = false;
    [SerializeField] private float slideSpeed;

//Inventory
    [Header("Inventory")]
    [SerializeField] private InventoryManager inventoryManager;
    private List<PickupType> inventory = new List<PickupType>();
    [SerializeField] private CrownInventoryManager crownInventoryManager;
    [SerializeField] private int crownDropThreshold;
    private List<PickupType> crownInventory = new List<PickupType>();
    [SerializeField] private ItemNameAtlas itemNames;
    [SerializeField] private GameObject floatyTextPrefab;


//Drops
    [Header("Currency")]
    [SerializeField] private CurrencyHintController currencyDisplay;
    [SerializeField] private GameObject gPickupPrefab;
    [SerializeField] private GameObject fPickupPrefab;
    private int gems;

//Items
    [Header("Use Items")]
    private Vector2 aim;
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
    [SerializeField] private GameObject armorRender;
    private int armor;
    [SerializeField] private GameObject pushBoxPrefab;
    private bool groundPoundEnabled = false;
    [SerializeField] private LayerMask blinkMask;

//Stamina
    [Header("Stamina")]
    [SerializeField] private StaminaHintController staminaHint;
    public readonly static int STAMINA_COST = 100;
    public readonly static int MAX_STAMINA = 200;
    private int stamina = MAX_STAMINA;

//Other
    [Header("Other")]
    [SerializeField] private BarController stunTimer;
    bool grounded;
    [SerializeField] private Collider2D hurtbox;
    [SerializeField] private Collider2D pickupbox;
    [SerializeField] private GameObject crown;
    private bool collidersLocked = false;
    private float colliderLockout;
    private bool paused = false;
    private Vector2 pauseVelocity;
    private float pauseTime;
    private float gravity;

// ID
    static int id_source = 0;
    int id;

// Bonus Crowns
    private int getsHit = 0;
    private int itemUse = 0;

    // Start is called before the first frame update
    void Start()
    {
        id = id_source;
        id_source++;

        gravity = rbody.gravityScale;

        PlayerManager pmanager = FindObjectsOfType<PlayerManager>()[0];
        director = FindObjectsOfType<LevelDirector>()[0];

        pmanager.RegisterPlayer(gameObject);

        animator.runtimeAnimatorController = characterData.animator;
        inventoryManager.Display(inventory);
        crownInventoryManager.Display(crownInventory);

        currencyDisplay.PushGems(gems);
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        bool gprev = grounded;
        grounded = groundCheck.CheckGrounded();
        animator.SetBool("grounded", grounded);
        if (gprev == false && gprev != grounded)
            Instantiate(dustPrefab, transform.position + new Vector3(0, -1.25f, 0), dustPrefab.transform.rotation);

        bool cprev = clinging;
        clinging = Utils.Raycast(   transform.position + 
                                    new Vector3(facingModifier * clingCastOffset.x, clingCastOffset.y, 0),
                                    new Vector2(facingModifier, 0),
                                    clingCastDistance,
                                    wallDetectMask) &&
                                    input.LeftShoulder_Held &&
                                    stamina > 0;
        if (clinging == true && cprev != clinging)
            Instantiate(dustPrefab, transform.position + new Vector3(facingModifier * 1f, 0.5f, 0), dustPrefab.transform.rotation);

        if (!paused)
            rbody.gravityScale = gravity;

        if (!InputLocked())
            ManageInputs();

        CheckColliderLockout();
        CheckPaused();

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

        if (input.Dir != Vector2.zero)
            aim = input.Dir;

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
            animator.SetTrigger("jump");
            rbody.velocity = new Vector3(rbody.velocity.x, jumpVelocity, 0);
            Instantiate(dustPrefab, transform.position + new Vector3(0, -1f, 0), dustPrefab.transform.rotation);
            EffectsMaster.Instance.SFXPlay("jump");
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
        if (input.B) {
            TryUseItem();
        }
        if (input.RightShoulder && !sliding && grounded && (stamina >= MAX_STAMINA || juiceActive)) {
            animator.SetTrigger("slide");
            if (!juiceActive)
                stamina -= MAX_STAMINA;
        }
        if (input.Y) {
            TryInteract();
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

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int[] getGems(){
        
        int [] gems = {this.gems, this.gems, this.gems};
        Debug.Log(this.gems.ToString());
        return gems;
    }
    public bool[] getCrowns(){  //battle, red, green, blue
        bool[] toReturn = new bool[4]{false,false,false,false};
        if(crownInventory.Contains(PickupType.CROWN_BLUE)){
            toReturn[0] = true;
        }
        if(crownInventory.Contains(PickupType.CROWN_GREEN)){
            toReturn[1] = true;
        }
        if(crownInventory.Contains(PickupType.CROWN_RED)){
            toReturn[2] = true;
        }
        if(crownInventory.Contains(PickupType.CROWN_BATTLE)){
            toReturn[3] = true;
        }
        return toReturn;
    }
    public int[] getBonusCrowns(){
        int [] crowns = {itemUse, getsHit};
        return crowns;
    }
    
    public Sprite GetSprite(){
        return characterData.sprite;
    }
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void StartMagnet() {
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

    public void StartSpeed() {
        speedActive = true;
        speedTime = Time.time + speedDuration;
    }

    private void EndSpeed() {
        speedActive = false;
    }

    public void StartJuice() {
        juiceActive = true;
        juiceTime = Time.time + juiceDuration;
    }

    private void EndJuice() {
        juiceActive = false;
    }

    public void SetArmor() {
        armorRender.SetActive(true);
        armor = armorHits;
    }
    private void TryUseItem() {
        if (inventory.Count > 0) {
                switch (inventory[0]) {
                    case PickupType.DRILL:
                        GameObject dart1 = Instantiate(drill);
                        dart1.GetComponent<Projectile>().Init(aim, hurtbox);
                        dart1.transform.position = transform.position;
                        break;
                    case PickupType.BOMB:
                        GameObject dart2 = Instantiate(bomb);
                        dart2.GetComponent<Projectile>().Init(aim, hurtbox);
                        dart2.transform.position = transform.position;
                        break;
                    case PickupType.DART:
                        GameObject dart3 = Instantiate(dartStorm);
                        dart3.GetComponent<DartStorm>().Init(aim, hurtbox);
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
            itemUse++;
        }
    }

    private void StartBlink() { 
        Vector2 dir = aim;
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
        EffectsMaster.Instance.SFXPlay("hit");
    }

    private void EndHit() {
        Destroy(attackBox);
    }

    private void EndAttack() {
        rbody.drag = 0;
        InputLockout(false);
        attacking = false;
    }

    private void TryInteract() {
        attackBox = Instantiate(interactBoxPrefab, transform);
    }

    private void StartSlide() {
        sliding = true;
        EffectsMaster.Instance.SFXPlay("slide");
        hurtbox.enabled = false;
        rbody.velocity = facingModifier * new Vector2(slideSpeed, 0);
        GameObject dust = Instantiate(dustPrefab, transform);
        dust.transform.localPosition = new Vector3(0, -1, 0);
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
            interactHint.transform.localPosition = new Vector3(interactHint.transform.localPosition.x * -1, interactHint.transform.localPosition.y, 0);
            interactHint.transform.localScale = newScale;
            currencyDisplay.transform.localScale = newScale;
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

    public bool OnHit() {
        if (director.GetCurrentLevelType() == LevelType.SHOP)
            return false;
            
        if (armor > 0) {
            armor--;
            if (armor == 0)
                armorRender.SetActive(false);
            return true;
        }

        animator.SetTrigger("hurt");

        rbody.velocity = Vector3.zero;
        
        EndHit();
        EndAttack();

        InputLockout(2);
        stunTimer.StartTimer(2);

        ColliderLockout(3);

        TryDropCrowns();
        TryDropGems();
        getsHit++;
        EffectsMaster.Instance.SFXPlay("hurt");
        return true;
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
        if (gems != 0) {
            int drop = gems / 2 + gems % 2;
            for (int i = 0; i < drop / 10; i++)
                switch(Random.Range(0, 3)) {
                    case 0:
                        DropGravPickup(PickupType.GEM_RED_LARGE);
                        break;
                    case 1:
                        DropGravPickup(PickupType.GEM_GREEN_LARGE);
                        break;
                    case 2:
                        DropGravPickup(PickupType.GEM_BLUE_LARGE);
                        break;
                }
            for (int i = 0; i < drop % 10; i++)
                switch(Random.Range(0, 3)) {
                    case 0:
                        DropGravPickup(PickupType.GEM_RED);
                        break;
                    case 1:
                        DropGravPickup(PickupType.GEM_GREEN);
                        break;
                    case 2:
                        DropGravPickup(PickupType.GEM_BLUE);
                        break;
                }
            gems -= drop;
            currencyDisplay.PushGems(gems);
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
        if (crownInventory.Count > 0) {
            PickupType crown = crownInventory[0];
            DropFloatPickup(crown);

            crownInventory.RemoveAt(0);
            crownInventoryManager.Display(crownInventory);
            TryDisableCrown(crown);
        }
    }


    public void InputLockout(float duration) {
        if (Time.time + duration < inputLockout)
            return;
        inputLockout = Time.time + duration;
    }

    public void InputLockout(bool toggle) {
        inputToggle = toggle;
    }

    private bool InputLocked() {
        return inputToggle || Time.time <= inputLockout;
    }

    public void Pause(float duration) {
        InputLockout(duration);
        pauseTime = Time.time + duration;
        paused = true;
        rbody.gravityScale = 0;
        pauseVelocity = rbody.velocity;
        rbody.velocity = Vector2.zero;
        animator.speed = 0;
    }

    private void CheckPaused() {
        if (paused && Time.time >= pauseTime) {
            paused = false;
            rbody.gravityScale = gravity;
            rbody.velocity = pauseVelocity;
            animator.speed = 1;
        }
    }

    public bool AddItem(PickupType type, int amount = 1) {
        if (type == PickupType.GEM_BLUE || type == PickupType.GEM_RED || type == PickupType.GEM_GREEN
        || type == PickupType.GEM_RED_LARGE || type == PickupType.GEM_BLUE_LARGE || type == PickupType.GEM_GREEN_LARGE) {
            gems += amount;
            currencyDisplay.PushGems(gems);
            return true;
        } else if (type == PickupType.CROWN_BLUE || type == PickupType.CROWN_GREEN || type == PickupType.CROWN_BATTLE
                || type == PickupType.CROWN_RED) {
                    if (crownInventory.Count >= CrownInventoryManager.INVENTORY_SIZE)
                       return false;
                    crownInventory.Add(type);
                    crownInventoryManager.Display(crownInventory);
                    TryEnableCrown(type);
                    Instantiate(floatyTextPrefab, transform.position + new Vector3(0, 2, 0), floatyTextPrefab.transform.rotation)
                        .GetComponent<FloatyText>().Set(itemNames.GetString(type));
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
            Instantiate(floatyTextPrefab, transform.position + new Vector3(0, 2, 0), floatyTextPrefab.transform.rotation)
                        .GetComponent<FloatyText>().Set(itemNames.GetString(type));
            return true;
        }
    }

    private void AdjustStamina() {
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

    }

    public void GemBurst(int drop) {
        for (int i = 0; i < drop / 10; i++)
            DropGravPickup((PickupType)Random.Range(4, 7));
        for (int i = 0; i < drop % 10; i++)
            DropGravPickup((PickupType)Random.Range(1, 4));
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
            case PickupType.GEM_RED:
            case PickupType.GEM_RED_LARGE:
            case PickupType.GEM_GREEN:
            case PickupType.GEM_GREEN_LARGE:
                return gems;
            default:
                return -1;
        }
    }

    public List<PickupType> GetCrowns() {
        return crownInventory;
    }

    public bool IsHurtboxOwner(Collider2D other) {
        return other == hurtbox;
    }
    public bool RemoveItem(PickupType type, int amount) {
        switch(type) {
            case PickupType.GEM_BLUE:
            case PickupType.GEM_BLUE_LARGE:
            case PickupType.GEM_RED:
            case PickupType.GEM_RED_LARGE:
            case PickupType.GEM_GREEN:
            case PickupType.GEM_GREEN_LARGE:
                if (gems >= amount) {
                    gems -= amount;
                    currencyDisplay.PushGems(gems);
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    public void Respawn() {
        interactHint.SetActive(false);
    }

    public void ReturnToStanding() {
        Destroy(attackBox);
        rbody.drag = 0;
        InputLockout(false);
        attacking = false;
    }
}
