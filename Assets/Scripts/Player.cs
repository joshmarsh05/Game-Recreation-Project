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
    private float inputX;
    private float inputY;
    private float jumpTimeCounter;
    private bool isJumping;
    private Camera m_Camera;
    private Vector2 screenBounds;

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

    void Start()
    {
        m_Camera = Camera.main;

        if(!rb)
            rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 4f;
        if(!animator)
            animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        // restrict player movement
        screenBounds = m_Camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        float clampedX = Mathf.Clamp(transform.position.x, -screenBounds.x, screenBounds.x);
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

        // Animation
        if(inputX != 0)
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

}
