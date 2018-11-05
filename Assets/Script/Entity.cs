using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    // --- Entity --- \\
    // Manage the Character controlled by the player. \\

    // --- Variables of Entity --- \\

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

    // Launch at the Start
    private void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        myRigidbody = GetComponent<Rigidbody2D>();
        mySprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        actHealthPoint = healthPoint;
    }

    // Check if the Entity is on the ground.
    protected bool CheckIfGrounded(Vector3 groundPosition) {
        RaycastHit2D hit = Physics2D.Raycast(groundPosition, -Vector2.up,1f,512); // Cast a raycast that hit only Ground in a close range.
        if (hit.collider != null && myRigidbody.velocity.y<0.1f)
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

    public void Freeze()
    {
        freeze = true;
    }

    public void Unfreeze()
    {
        freeze = false;
    }


}
