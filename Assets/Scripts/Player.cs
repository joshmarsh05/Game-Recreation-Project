using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    [Header("Animation & Rigidbody")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Rigidbody2D rb;
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
    public float fireballOffset;
    private float fireball1Timer = 0f;
    private float fireball2Timer = 0f;
    private float fireballTimer = 0.5f;
    private bool star = false;
    private float timer = 0f;
    private float starTimer = 8f;
    private bool invulnerable = false;
    private float invulnerableTimerCount = 0f;
    private float invulnerableTimer = 3f;
    private bool secretArea = false;
    private bool dead = false;

    [Space]
    [Header("Game Manager")]
    [SerializeField] GameManager gameManager;

    void Start()
    {
        m_Camera = Camera.main;

        if(!rb)
            rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 4f;
        if(!animator)
            animator = GetComponent<Animator>();
        if(!gameManager)
            gameManager = GameObject.Find("HUD").GetComponent<GameManager>();
    }

    void Update()
    {
        if(!dead){
            // camera movement
            if(!secretArea){
            if(previousPlayerX < currentPlayerX)
                previousPlayerX = currentPlayerX;
            currentPlayerX = transform.position.x;
            if(currentPlayerX > previousPlayerX){
                m_Camera.transform.position = new Vector3(currentPlayerX, 0f, -10f);
            }
            // restrict player movement
            float clampedX = Mathf.Clamp(transform.position.x, minX + m_Camera.transform.position.x, maxX + m_Camera.transform.position.x);
            Vector2 pos = transform.position;
            pos.x = clampedX;
            transform.position = pos;
            } else
                m_Camera.transform.position = new Vector3(29f, -10.2f, -10f);

            // Input
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");

            //Flip the player based on the Input
            if (inputX > 0){
                transform.eulerAngles = new Vector3(0, 0, 0);
                fireballOffset = 2f;
            }
            else if (inputX < 0) {
                transform.eulerAngles = new Vector3(0, 180, 0);
                fireballOffset = -2f;
            }

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
                timer += Time.deltaTime;
                if(timer > starTimer){
                    star = false;
                    animator.SetBool("Star", false);
                    if(mini)
                        animator.SetBool("Mini", true);
                    timer = 0;
                }
            }

            if(fire){
                fireball1Timer += Time.deltaTime;
                fireball2Timer += Time.deltaTime;
                if(Input.GetKeyDown(KeyCode.LeftShift)){
                    if(fireball1Timer > fireballTimer){
                    Instantiate(fireball, new Vector3(transform.position.x + fireballOffset, transform.position.y - 0.5f, 0), transform.rotation);
                    fireball1Timer = 0;
                    }
                    else if(fireball2Timer > fireballTimer){
                    Instantiate(fireball, new Vector3(transform.position.x + fireballOffset, transform.position.y - 0.5f, 0), transform.rotation);
                    fireball2Timer = 0;
                    }
                }
            }

            // if hit by enemy and transform down 
            if(invulnerable){
                Physics2D.IgnoreLayerCollision(3, 8, true);
                invulnerableTimerCount += Time.deltaTime;
                if(invulnerableTimerCount > invulnerableTimer){
                    invulnerable = false;
                    Physics2D.IgnoreLayerCollision(3, 8, false);
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
    }

    private void FixedUpdate()
    {
        if(!dead)
            rb.velocity = new Vector2(inputX * speed, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "PowerUp"){
            Debug.Log(other.gameObject.name);
            if(other.gameObject.name == "Mushroom(Clone)"){
                gameManager.AddScore(1000, other.gameObject.transform);
                if(!fire){
                    mini = false;
                    big = true;
                    animator.SetBool("Big", true);
                    animator.SetBool("Mini", false);
                }
            } else if(other.gameObject.name == "FireFlower(Clone)"){
                gameManager.AddScore(1000, other.gameObject.transform);
                mini = false;
                big = false;
                fire = true;
                animator.SetBool("Fire", true);
                animator.SetBool("Big", false);
                animator.SetBool("Mini", false);
            } else if(other.gameObject.name == "Star(Clone)"){
                star = true;
                animator.SetBool("Star", true);
                animator.SetBool("Mini", false);
            }
                
            Destroy(other.gameObject);
        }

        if(other.gameObject.tag == "Enemy" || (other.gameObject.tag == "Attack" && other.gameObject.GetComponent<Enemy>().canKillPlayer)){
            if(star)
                other.gameObject.GetComponent<Enemy>().Dead();
            else if(mini && !invulnerable)
                Dead();
            else if(big || fire){
                mini = true;
                big = false;
                fire = false;
                invulnerable = true;
                animator.SetBool("Fire", false);
                animator.SetBool("Big", false);
                animator.SetBool("Mini", true);
                animator.SetBool("Invulnerable", true);
            }
        }

        if(other.gameObject.tag == "Flag")
            Win();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isJumping){
            isJumping = false;
        }
        if(other.gameObject.tag == "Coin"){
            gameManager.AddCoin();
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Secret")
            if(Input.GetKeyDown(KeyCode.S)){
                Debug.Log("Secret entered");
                secretArea = true;
                transform.position = new Vector2(24.8f, -6f);
            }
        
        if(other.gameObject.tag == "Exit")
            if(Input.GetKeyDown(KeyCode.D)){
                secretArea = false;
                transform.position = new Vector2(101.43f, -1.8f);
                m_Camera.transform.position = new Vector3(112f, 0f, -10f);
            }   
    }

    void Dead(){
        dead = true;
        animator.SetTrigger("Dead");
        animator.SetBool("Running", false);
        animator.SetBool("Jump", false);
    }

    void DeathAnimationComplete(){ 
        SceneManager.LoadScene("Level 1");
    }

    void Win(){

    }
}
