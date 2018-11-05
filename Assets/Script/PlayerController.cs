using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // --- CharacterController --- \\
    // Manage the Character controlled by the player. \\

    // --- Variables of CharacterController --- \\

    [SerializeField]private float jumpHeight = 2000; // Height the Character reach after a jump.
    [SerializeField]public GameObject slashPrefab;
    public bool inWater = false; // Check if the player is in the water.
    public float gravity = 12;
    public float waterSpeed=65;
    private bool lockInWater = false;
    private bool pressJump = false;
    public float jumpChartMaxTime = 0.16f;
    public int jumpChartNumberOfIncretions = 8;
    public float jumpChartSpeedDeceleration = 0.5f;
    public float gravityMultiplier = 1;
    public float jumpStrengthMultiplier = 1;
    public float moveSpeedMultiplier = 1;
    public float swimSpeedMultiplier = 1;
    private bool blob = false;
    public float linearDragBlob = 2;
    public float linearDragNormal = 8;
    private GameObject[] waterCollider;
    private float colliderSize;
    private Vector2 respawn = new Vector2();
    public Sprite spriteAxolotl;
    public Sprite spriteSlime;
    public Vector2[] damageHitBox = new Vector2[2];
    private OutOfWaterDetector outOfWaterDetector;
    public int healthPoint = 3; // Actual life the character has.
    public float moveSpeed = 65; // Speed of the character when he move right or left.
    protected Rigidbody2D myRigidbody; // Rigidbody component of the character.
    protected bool grounded = true; // True if the Character is on the ground.
    protected bool isLookingRight = false; // False = Look to left - True = Look to Right
    protected SpriteRenderer mySprite; // The SpriteRenderer use by the Entity.
    protected bool invulnerabilityFrame = false; // Bool that check if the entity is on invincibility frame or not.
    protected GameManager gameManager;
    protected bool freeze = false; // Bool that check if the entity is frozen.
    protected int actHealthPoint = 3;

    // ---------------------------------------------------------- \\

    // Update is called once per frame
    private void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        myRigidbody = GetComponent<Rigidbody2D>();
        mySprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        waterCollider = GameObject.FindGameObjectsWithTag("Water");
        colliderSize = gameObject.GetComponent<BoxCollider2D>().size.y;
        respawn = transform.position;
        outOfWaterDetector = transform.GetChild(3).GetComponent<OutOfWaterDetector>();
    }

    // Check if the Entity is on the ground.
    protected bool CheckIfGrounded(Vector3 groundPosition) {
        RaycastHit2D hit = Physics2D.Raycast(groundPosition, -Vector2.up, 1f, 512); // Cast a raycast that hit only Ground in a close range.
        if (hit.collider != null && myRigidbody.velocity.y < 0.1f)
            return true;
        else
            return false;
    }

    // Update the flip of the sprite depending his rotation.
    protected void UpdateSprite() {
        if (isLookingRight) {
            mySprite.flipX = false;
        }
        else mySprite.flipX = true;
    }

    // Make the entity stop moving by annuling his current Force.
    protected void StopMove() {
        myRigidbody.AddForce(-myRigidbody.velocity * 10);
    }

    public void Freeze() {
        freeze = true;
    }

    public void Unfreeze() {
        freeze = false;
    }

    private void Update() {
        if (Input.GetKeyDown("p")) {
            gameManager.GetComponent<GameManager>().LaunchMenu();
        }
    }
    void FixedUpdate() {
        if (!freeze) {
            //Store the current horizontal input in the float moveHorizontal.
            float moveHorizontal;
            if (blob) moveHorizontal = Input.GetAxis("HorizontalBlob");
            else moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical;
            if (!blob) moveVertical = Input.GetAxis("Vertical");
            else moveVertical = 0;
            bool moveHorizontalBool = Input.GetButtonUp("Horizontal");
            bool pressAttackButton = Input.GetButtonDown("Attack");
            //Store the current vertical input in the float moveVertical.
            if (!inWater) myRigidbody.gravityScale = gravity * gravityMultiplier;
            if (Input.GetKey("joystick button 0") || Input.GetKey("space")) pressJump = true;
            else pressJump = false;
            if (Input.GetKeyDown("joystick button 2") || Input.GetKeyDown("s")) MorphToBlob();
            if (inWater) {
                if (moveHorizontal !=0 || moveVertical != 0) {
                    Swim(moveHorizontal, moveVertical);
                }
            }
            if (Input.GetKeyDown("o")) KnockBack();
            if (Input.GetKeyDown("joystick button 3") || Input.GetKeyDown("x")) respawn = transform.position;

            else {
                // If player pressed Horizontal direction, make the character move.
                if (moveHorizontal != 0) {
                    moveHorizontal /= Mathf.Abs(moveHorizontal); // Make moveHorizontal get 1 or -1 value.
                    Movement(moveHorizontal); // The Rigidbody add moveHorizontal value to the Rigidbody Velocity.
                }
                // Instantly stop force apply to the player if he release direction.
                if (moveHorizontalBool && !blob) {
                    StopMove();
                }
                // If player pressed Up, make the character jump.
                if (pressJump && Input.GetButtonDown("Jump") && !blob) {
                    Jump();
                }
            }
            // If player pressed the Attack Button, make the character Attack.
            if (pressAttackButton) {
                lockInWater = true;
                Attack();
            }
            UpdateSprite();
            CheckEnemyCollision();
        }
    }
    void OnTriggerStay2D(Collider2D other) {
        if (other.GetComponent<Water>() != null) {
            inWater = true;
            if (blob) myRigidbody.gravityScale = -10*gravityMultiplier;
            else myRigidbody.gravityScale = 0;
        }
    }

    private void OnTriggerExit2D (Collider2D other) {
        if (other.GetComponent<Water>() != null && !lockInWater) {
            inWater = false;
            myRigidbody.gravityScale = gravity*gravityMultiplier;
            StartCoroutine(ResetGravity());
            if (blob) {
                Jump(0.6f, true);
                
                for (var n = 0; n < waterCollider.Length; n++) {
                    waterCollider[n].GetComponent<Collider2D>().isTrigger = false;
                }
            }
        }
    }

    // Make the character move with a certain speed.
    private void Swim (float xMoveQuantity, float yMoveQuantity) {
        Vector2 movement = new Vector2(xMoveQuantity, yMoveQuantity);
        myRigidbody.AddForce(movement * waterSpeed * swimSpeedMultiplier); // Apply the additional speed to the Object Force.
    }

    private void Movement (float MoveQuantity) {
        Vector2 movement = new Vector2(MoveQuantity, 0);
        myRigidbody.AddForce(movement * moveSpeed*moveSpeedMultiplier); // Apply the additional speed to the Object Force.
        if (MoveQuantity > 0) {
            isLookingRight = true;
        }
        else isLookingRight = false;
    }

    // Make the character jump.
    private void Jump(float multiplier=1, bool ignoreInput = false) {
        if (!outOfWaterDetector.inWater && inWater) {
            multiplier = 1.01f;
        }
        grounded = CheckIfGrounded(transform.GetChild(2).position);
        if (grounded || multiplier!=1) {
            Vector2 movement = new Vector2(0, multiplier);
            StartCoroutine(JumpEquation(movement, ignoreInput));
        }
    }

    // Make the Character use a basic attack in front of him.
    private void Attack() {
        StartCoroutine(AttackBlock());
        GameObject slashEffect = Instantiate (slashPrefab,transform.GetChild(1)); // Generate the attack gameobject.
        if (isLookingRight) slashEffect.transform.position = new Vector2((damageHitBox[0].x + damageHitBox[1].x) / 2 + transform.position.x, (damageHitBox[0].y + damageHitBox[1].y) / 2 + transform.position.y);
        else slashEffect.transform.position = new Vector2((damageHitBox[0].x + damageHitBox[1].x) / -2 + transform.position.x, (damageHitBox[0].y + damageHitBox[1].y) / 2 + transform.position.y);
        slashEffect.GetComponent<BoxCollider2D>().size = new Vector2(Mathf.Abs(damageHitBox[1].x - damageHitBox[0].x), Mathf.Abs(damageHitBox[1].y - damageHitBox[0].y));
        slashEffect.GetComponent<SpriteRenderer>().size = new Vector2(Mathf.Abs(damageHitBox[1].x - damageHitBox[0].x), Mathf.Abs(damageHitBox[1].y - damageHitBox[0].y));
    }

    // Make the character check if he collider with a enemy hitbox.
    private void CheckEnemyCollision()
    {
        bool directionIsLeft = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.4f, 2048); // Cast a raycast that hit only Enemy in a close range.
        if (hit.collider == null) {
            hit = Physics2D.Raycast(transform.position, Vector2.left, 0.4f, 2048);
            directionIsLeft = true;
        }
        else if (hit.collider == null) {
            hit = Physics2D.Raycast(transform.position, Vector2.down, 0.4f, 2048);
            directionIsLeft = true;
        }
        else if (hit.collider == null) {
            hit = Physics2D.Raycast(transform.position, Vector2.up, 0.4f, 2048);
            directionIsLeft = true;
        }
        if (hit.collider != null)
            LoseHealth(directionIsLeft,hit.transform.gameObject.GetComponent<Ennemies>().damageDealt, hit.transform.gameObject.GetComponent<Ennemies>().knockBackStrength);
    }

    // Make the character lose a health point.
    private void LoseHealth(bool directionIsLeft=true, int lifelost=1, float knockBackStrength=1) {
        if (!invulnerabilityFrame) {
            actHealthPoint -= lifelost;
            gameManager.GetCanvas().ChangeLife(actHealthPoint);
            if (actHealthPoint <= 0) {
                Death();
            }
            else { // Make the player flash for showing he lost health.
                KnockBack(directionIsLeft,knockBackStrength);
                StartCoroutine(WhiteFlash());
            }
        }
    }

    // Make the character die.
    private void Death() {
        actHealthPoint = healthPoint;
        gameManager.GetCanvas().ChangeLife(actHealthPoint);
        transform.position = respawn;
        gameManager.Respawn();
    }

    // Make the character morph or unmorph to blob form.
    private void MorphToBlob() {
        
        if (blob && !CheckIfSomethingUp(transform.GetChild(2).position)) { // Launch when you are a blob, and transforming to normal.
            mySprite.sprite = spriteAxolotl;
            myRigidbody.drag = linearDragNormal;
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(colliderSize, colliderSize);
            gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
            mySprite.size = new Vector2(1, 1);
            for (var n = 0; n < waterCollider.Length; n++) {
                waterCollider[n].GetComponent<Collider2D>().isTrigger = true;
            }
        }
        if (!blob) { // Launch when you are normal, and transforming to blob.
            mySprite.sprite = spriteSlime;
            myRigidbody.drag = linearDragBlob;
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(colliderSize, colliderSize / 2);
            gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, -colliderSize / 4);
            mySprite.size = new Vector2(1, 0.5f);
            if (!inWater) { 
                for (var n = 0; n < waterCollider.Length; n++) {
                    waterCollider[n].GetComponent<Collider2D>().isTrigger = false;
                }
            }
        }
        blob = !blob;
    }

    protected bool CheckIfSomethingUp(Vector3 groundPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(groundPosition, Vector2.up, 1f, 512); // Cast a raycast that hit only Ground in a close range.
        if (hit.collider != null && myRigidbody.velocity.y < 0.1f)
            return true;
        else
            return false;
    }

    public void KnockBack(bool directionIsLeft=true, float knockBackStrength=1) {
        if (directionIsLeft) myRigidbody.AddForce(new Vector2(1500*knockBackStrength, 1500* knockBackStrength));
        else myRigidbody.AddForce(new Vector2(-1500* knockBackStrength, 1500* knockBackStrength));
    }



    // Coroutine that make the enemy flash after taking a hit.
    IEnumerator WhiteFlash()
    {
        invulnerabilityFrame = true;
        for (var n = 0; n < 5; n++)
        {
            mySprite.enabled = true;
            yield return new WaitForSeconds(0.04f);
            mySprite.enabled = false;
            yield return new WaitForSeconds(0.04f);
        }
        mySprite.enabled = true;
        yield return new WaitForSeconds(1.25f);
        invulnerabilityFrame = false;
    }

    IEnumerator AttackBlock()
    {
        yield return new WaitForSeconds(0.03f);
        lockInWater = false;
        
    }

    IEnumerator ResetGravity()
    {
        yield return new WaitForSeconds(0.03f);
        if (myRigidbody.gravityScale < 0) {
        myRigidbody.AddForce(new Vector2(0, -500));
        myRigidbody.gravityScale = gravity * gravityMultiplier;
        }

    }

    IEnumerator JumpEquation(Vector2 movement, bool ignoreInput = false)
    {
        Vector2 jumpMove = movement;
        for (var n = 1; n < jumpChartNumberOfIncretions; n++)
        {
            if (pressJump || ignoreInput) {
                jumpMove *= jumpChartSpeedDeceleration;
                myRigidbody.AddForce(movement * jumpHeight * jumpChartSpeedDeceleration*jumpStrengthMultiplier);
                yield return new WaitForSeconds(jumpChartMaxTime/jumpChartNumberOfIncretions);
            }
        }
    }
}
