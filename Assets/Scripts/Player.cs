using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    [Header("Animation & Rigidbody")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D boxCol;
    public float minX = -8.3f;
    public float maxX = 8.3f;
    private float inputX;
    private float inputY;
    private float jumpTimeCounter;
    private bool isJumping;
    private Camera m_Camera;
    private Vector2 screenBounds;
    private float previousPlayerX;
    private float currentPlayerX;

    // Use these variable to the check if the player is grounded or not
    [Space]
    [Header("Ground check")]
    public Transform feetPos;
    public LayerMask ground;
    public float checkRadius;
    public bool isGrounded;

    [Space]
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 20f;
    public float jumpTime = 0.25f;   

    private bool mini = true;
    private bool big = false;
    private bool fire = false;
    public GameObject fireball;
    public float fireBallOffset;
    private bool star = false;
    private float timer = 0f;
    private float starTimer = 5f;
    private bool invulnerable = false;
    private float invulnerableTimerCount = 0f;
    private float invulnerableTimer = 3f;


    void Start()
    {
        m_Camera = Camera.main;

        if(!rb)
            rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 4f;
        if(!boxCol)
            boxCol = GetComponent<BoxCollider2D>();
        if(!animator)
            animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        // camera movement
        if(previousPlayerX < currentPlayerX)
            previousPlayerX = currentPlayerX;
        currentPlayerX = transform.position.x;
        if(currentPlayerX > previousPlayerX){
            m_Camera.transform.position = new Vector3(currentPlayerX, 0, -10);
        }
        // restrict player movement
        float clampedX = Mathf.Clamp(transform.position.x, minX + m_Camera.transform.position.x, maxX + m_Camera.transform.position.x);
        Vector2 pos = transform.position;
        pos.x = clampedX;
        transform.position = pos;

        // Input
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        //Flip the player based on the Input
        if (inputX > 0){
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (inputX < 0)
            transform.eulerAngles = new Vector3(0, 180, 0);

        // Check whether the player is grounded or not
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, ground);

        // If the player is grounded enable him to jump 
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector2.up * jumpForce;
            isJumping = true;
            jumpTimeCounter = jumpTime;
        }

        /*
         * Enabling the player Mario like jump
         * As long as the player keeps holding the space button in this case the space key
         * we add a little bit of jump force in the time he presses it and immediately apply the gravity back
         */
        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
                isJumping = false;
        }
        if (Input.GetKeyUp(KeyCode.Space))
            isJumping = false;

        
        // Power Ups
        if(star){
            timer++;
            if(timer > starTimer){
                star = false;
                animator.SetBool("Star", false);
                timer = 0;
            }
        }

        if(fire && Input.GetKeyDown(KeyCode.LeftShift))
            Instantiate(fireball, new Vector3(transform.position.x + fireBallOffset, transform.position.y, 0), transform.rotation);

        // if hit by enemy and transform down 
        if(invulnerable){
            invulnerableTimerCount++;
            if(invulnerableTimerCount > invulnerableTimer){
                invulnerable = false;
                animator.SetBool("Invulnerable", false);
                invulnerableTimerCount = 0;
            }
        }

        // Animation
        if(inputX != 0 && currentPlayerX != previousPlayerX)
            animator.SetBool("Running", true);
        else
            animator.SetBool("Running", false);
        
        if(!isGrounded)
            animator.SetBool("Jump", true);
        else 
            animator.SetBool("Jump", false);
    }

    private void FixedUpdate()
    {
        // Moving the player in X-axis using the InputX every physics cycle
        rb.velocity = new Vector2(inputX * speed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "PowerUp"){
            if(other.gameObject.name == "Mushroom"){
                mini = false;
                big = true;
                animator.SetBool("Big", true);
                animator.SetBool("Mini", false);
            } else if(other.gameObject.name == "FireFlower"){
                mini = false;
                big = true;
                fire = true;
                animator.SetBool("Fire", true);
                animator.SetBool("Mini", false);
            } else if(other.gameObject.name == "Star"){
                star = true;
                animator.SetBool("Star", true);
            }
            if(big){
                
            }
                
            Destroy(other.gameObject);
        }

        if(other.gameObject.tag == "Enemy"){
            if(mini && !invulnerable)
                Dead();
            else if(big || fire){
                mini = true;
                big = false;
                fire = false;
                invulnerable = true;
            } else if(star){
                other.gameObject.GetComponent<Enemy>().Dead();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isJumping){
            isJumping = false;
        }
    }

    void Dead(){

    }
}
