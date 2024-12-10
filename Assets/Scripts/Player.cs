using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    [Header("Important Game Mechanics")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] AudioManager audioManager;
    [SerializeField] GameManager gameManager;
    public AudioSource music;
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
    private bool win = false;
    private bool timeWarning = true;
    private bool winSounds = true;

    void Start()
    {
        audioManager.PlayMusic("MainTheme");
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
        if(!dead && !win){
            // camera movement
            if(!secretArea){
            if(previousPlayerX < currentPlayerX)
                previousPlayerX = currentPlayerX;
            currentPlayerX = transform.position.x;
            if(currentPlayerX > previousPlayerX){
                float clampCamera = Mathf.Clamp(currentPlayerX, 0.1f, 121.1f);
                m_Camera.transform.position = new Vector3(clampCamera, 0f, -10f);
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
                if(mini)
                    audioManager.PlaySFX("MiniJump");
                if(big || fire)
                    audioManager.PlaySFX("BigJump");
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
                    audioManager.PlayMusic("MainTheme");
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
                    audioManager.PlaySFX("Fireball");
                    }
                    else if(fireball2Timer > fireballTimer){
                    Instantiate(fireball, new Vector3(transform.position.x + fireballOffset, transform.position.y - 0.5f, 0), transform.rotation);
                    fireball2Timer = 0;
                    audioManager.PlaySFX("Fireball");
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
            if(gameManager.time <= 100 && timeWarning){
                timeWarning = false;
                audioManager.PlaySFX("TimeWarning");
                music.pitch = 1.25f;
            }
        }
    }

    private void FixedUpdate()
    {
        if(!dead && !win)
            rb.velocity = new Vector2(inputX * speed, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "PowerUp"){
            Debug.Log(other.gameObject.name);
            if(other.gameObject.GetComponent<PowerUp>().powerUp == PowerUpType.Mushroom){
                audioManager.PlaySFX("PowerUp");
                gameManager.AddScore(1000, other.gameObject.transform);
                if(!fire){
                    mini = false;
                    big = true;
                    animator.SetBool("Big", true);
                    animator.SetBool("Mini", false);
                }
            } else if(other.gameObject.GetComponent<PowerUp>().powerUp == PowerUpType.FireFlower){
                audioManager.PlaySFX("PowerUp");
                gameManager.AddScore(1000, other.gameObject.transform);
                mini = false;
                big = false;
                fire = true;
                animator.SetBool("Fire", true);
                animator.SetBool("Big", false);
                animator.SetBool("Mini", false);
            } else if(other.gameObject.GetComponent<PowerUp>().powerUp == PowerUpType.Star){
                star = true;
                audioManager.PlayMusic("StarTheme");
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
                audioManager.PlaySFX("PowerDown");
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
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isJumping){
            isJumping = false;
        }
        if(other.gameObject.tag == "Coin"){
            audioManager.PlaySFX("Coin");
            gameManager.AddCoin();
            Destroy(other.gameObject);
        }
        if(other.gameObject.tag == "Death"){
            Dead();
        }
        if(other.gameObject.tag == "Flag"){
            Win();
            if(transform.position.y > 3)
                gameManager.AddScore(8000, transform);
            else if(transform.position.y > 2)
                gameManager.AddScore(4000, transform);
            else if(transform.position.y > 1)
                gameManager.AddScore(2000, transform);
            else if(transform.position.y > 0)
                gameManager.AddScore(1000, transform);
            else if (transform.position.y > -0.5)
                gameManager.AddScore(800, transform);
            else if(transform.position.y > -1)
                gameManager.AddScore(400, transform);
            else if(transform.position.y > -1.8)
                gameManager.AddScore(200, transform);
            else if (transform.position.y > -3)
                gameManager.AddScore(100, transform);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Secret")
            if(Input.GetKeyDown(KeyCode.S)){
                Debug.Log("Secret entered");
                secretArea = true;
                transform.position = new Vector2(24.8f, -6f);
                audioManager.PlaySFX("PowerDown");
            }
        
        if(other.gameObject.tag == "Exit")
            if(Input.GetKeyDown(KeyCode.D)){
                secretArea = false;
                transform.position = new Vector2(101.43f, -1.8f);
                m_Camera.transform.position = new Vector3(112f, 0f, -10f);
                audioManager.PlaySFX("PowerDown");
            }   
    }

    void Dead(){
        this.tag = "Untagged";
        dead = true;
        audioManager.PlaySFX("Die");
        animator.SetBool("Dead", true);
        animator.SetBool("Running", false);
        animator.SetBool("Jump", false);
    }

    void DeathAnimationComplete(){ 
        SceneManager.LoadScene("Level 1");
    }

    void Win(){
        music.gameObject.SetActive(false);
        win = true;
        if(winSounds){
            audioManager.PlaySFX("Flagpole");
            winSounds = false;
        }
        animator.applyRootMotion = false;
        animator.SetBool("Win", true);
        gameManager.win = true;
    }

    void LevelClearSound(){
        audioManager.PlaySFX("StageClear");
    }

    void WinAnimtionComplete(){
        SceneManager.LoadScene("Credits");
    }
}
