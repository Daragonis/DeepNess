using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemies : MonoBehaviour {

    // --- Ennemies --- \\
    // Manage the general Ennemies behaviour. \\
    // --- Variables of Enemies --- \\
    public bool patrol = false;
    private Vector2[] patrolPoint = new Vector2[2];
    private int targetPatrolPoint = 0;
    private bool patrolLastMoveIsRight;
    public float patrolScale;
    public float patrolCenter;
    private Vector2 respawnzone = new Vector2();
    public float gravity = 8;
    private bool blockPatrolDirection = false;
    private bool dead = false;
    public float knockBackStrength = 1;
    public int damageDealt = 1;
    public int healthPoint = 3; // Actual life the character has.
    public float moveSpeed = 65; // Speed of the character when he move right or left.
    public Rigidbody2D myRigidbody; // Rigidbody component of the character.
    protected bool grounded = true; // True if the Character is on the ground.
    protected bool isLookingRight = false; // False = Look to left - True = Look to Right
    protected SpriteRenderer mySprite; // The SpriteRenderer use by the Entity.
    protected bool invulnerabilityFrame = false; // Bool that check if the entity is on invincibility frame or not.
    protected GameManager gameManager;
    protected bool freeze = false; // Bool that check if the entity is frozen.
    protected int actHealthPoint = 3;
    public GameObject projectile;
    public float timeBetweenShoot = 3;
    public float shootSpeed = 20;
    public bool canShootProjectile = false;
    public GameObject player;
    public float timer=999;
    public float playerDistance = 1000;
    public float attackRange = 10;
    public enum CreatureType {
        Grounded,
        Flying,
        Projectile
    }
    public CreatureType typeOfCreature = CreatureType.Grounded;



    private void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        myRigidbody = GetComponent<Rigidbody2D>();
        if (typeOfCreature != CreatureType.Grounded) gravity = 0;
        myRigidbody.gravityScale = gravity;
        if (typeOfCreature != CreatureType.Projectile)
            mySprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        patrolPoint[0] = new Vector2(patrolCenter - patrolScale/2, transform.position.y);
        patrolPoint[1] = new Vector2(patrolCenter + patrolScale / 2, transform.position.y);
        respawnzone = transform.position;
    }

    private void FixedUpdate() {
        if (typeOfCreature != CreatureType.Projectile) {
            if (!dead) {
                playerDistance = Vector2.Distance(transform.position, player.transform.position);
                if (playerDistance < attackRange) {
                    if (canShootProjectile) {
                        timer += Time.deltaTime;
                        if (timer >= timeBetweenShoot) {
                            Shoot(projectile, shootSpeed);
                            timer = 0;
                        }
                    }
                    if (player.transform.position.x > transform.position.x) isLookingRight = true;
                    else isLookingRight = false;
                }
                else {
                    if (patrol) Patrol();
                    if (myRigidbody.velocity.x > 0) {
                        isLookingRight = true;
                    }
                    else isLookingRight = false;
                }
                UpdateSprite();
            }
        }

    }
    // Check if the Entity is on the ground.
    protected bool CheckIfGrounded(Vector3 groundPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(groundPosition, -Vector2.up, 1f, 512); // Cast a raycast that hit only Ground in a close range.
        if (hit.collider != null && myRigidbody.velocity.y < 0.1f)
            return true;
        else
            return false;
    }

    // Update the flip of the sprite depending his rotation.
    protected void UpdateSprite()
    {
        if (isLookingRight)
        {
            mySprite.flipX = false;
        }
        else mySprite.flipX = true;
    }

    // Make the entity stop moving by annuling his current Force.
    protected void StopMove()
    {
        myRigidbody.AddForce(-myRigidbody.velocity * 10);
    }

    public void Freeze()
    {
        freeze = true;
    }

    public void Unfreeze()
    {
        freeze = false;
    }

    // Check in something collide with an ennemy.
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 10) { //If the thing that collide with the ennemy was a player attack, he lost health.
            LoseLife();
        }
    }

    // Make the ennemi lost health point when he get hit.
    private void LoseLife() {
        if (!invulnerabilityFrame) {
            actHealthPoint -= 1;
            if (actHealthPoint <= 0) { // Make the enemy die if he lost his last health.
                Death();
            }
            else { // Make the enemy flash for showing he lost health.
                StartCoroutine(WhiteFlash());
            }
        }
    }

    // Destroy the ennemies when he lose his last health point.
    private void Death() {
        dead = true;
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void Respawn() {
        dead = false;
        actHealthPoint = healthPoint;
        GetComponent<Rigidbody2D>().gravityScale = gravity;
        transform.position = respawnzone;
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = true;

    }

    private void Patrol() {
        if (patrolPoint[targetPatrolPoint].x > transform.position.x) { // if the patrol point is at the right of the character.
            Move(1); // move to right.
            if (myRigidbody.velocity.x < 0 && !blockPatrolDirection) {
                StartCoroutine("BlockPatrolDirection");
                Move(3);
                if (targetPatrolPoint == 0) targetPatrolPoint = 1;
                else targetPatrolPoint = 0;
            }
        }
        else { // if the patrol point is at the left of the character.
            Move(-1); // move to left.
            if (myRigidbody.velocity.x > 0 && !blockPatrolDirection) {
                StartCoroutine("BlockPatrolDirection");
                Move(-3);
                if (targetPatrolPoint == 0) targetPatrolPoint = 1;
                else targetPatrolPoint = 0;
            }
        }
    }

    private void Shoot(GameObject projectile, float speed) {
        GameObject shoot = Instantiate(projectile, transform); // Generate the projectile gameobject.
        Vector2 direction = player.transform.position - transform.position;
        direction = direction.normalized;
        shoot.GetComponent<Rigidbody2D>().AddForce(direction * speed);
    }

    private void Move(float MoveQuantity) {
        Vector2 movement = new Vector2(MoveQuantity, 0);
        myRigidbody.AddForce(movement * moveSpeed); // Apply the additional speed to the Object Force.
    }

    // Coroutine that make the enemy flash after taking a hit.
    IEnumerator WhiteFlash() {
        invulnerabilityFrame = true;
        for (var n = 0; n < 5; n++) {
            mySprite.enabled = true;
            yield return new WaitForSeconds(0.04f);
            mySprite.enabled = false;
            yield return new WaitForSeconds(0.04f);
        }
        mySprite.enabled = true;
        invulnerabilityFrame = false;
    }

    IEnumerator BlockPatrolDirection() {
        blockPatrolDirection = true;
        yield return new WaitForSeconds(0.2f);
        blockPatrolDirection = false;
    }


}
