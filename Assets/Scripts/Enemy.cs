using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType{
    Goomba = 0,
    KoopaTroopa = 1
}

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameManager gameManager;
    [SerializeField] AudioManager audioManager;
     private float direction = -1;
    public float speed = 2f;
    public bool jumpDeath = false;
    public EnemyType enemyType;
    private float timer = 0.0f;
    private float unShellTimer = 10.0f;
    private bool isNotMoving = true;
    public bool canKillPlayer = false;
    private bool startShellKillable = false;
    private float shellKillTimer = 0f;
    private float shellKillTime = 1f;
    

    private void Start() {
        if(!animator)
            animator = GetComponent<Animator>();
        if(!rb)
            rb = GetComponent<Rigidbody2D>();
        if(!gameManager)
            gameManager = GameObject.Find("HUD").GetComponent<GameManager>();
        if(!audioManager)
            audioManager = GameObject.Find("HUD").GetComponent<AudioManager>();
    }

    void Update(){
        if(enemyType == EnemyType.KoopaTroopa && jumpDeath && isNotMoving){
            timer += Time.deltaTime;
            speed = 0f;
            if(timer > unShellTimer){
                jumpDeath = false;
                timer = 0;
                animator.SetTrigger("Unshell");
                animator.SetBool("Stomp", false);
                speed = 2f;
                this.tag = "Enemy";
            }
        }
        if(startShellKillable){
            shellKillTimer += Time.deltaTime;
            if(shellKillTimer >= shellKillTime){
                canKillPlayer = true;
            }
        }
    }
    
    void FixedUpdate()
    {
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);

        if(direction < 0)
            transform.eulerAngles = new Vector3(0, 0, 0);
        else
            transform.eulerAngles = new Vector3(0, 180, 0);
    }

    public void Dead(){
        gameManager.AddScore(100, this.transform);
        speed = 0f;
        if(jumpDeath && this.tag == "Enemy"){
            animator.SetBool("Stomp", true);
            audioManager.PlaySFX("Stomp");
        }
        else{
            animator.SetBool("Dead", true);
            audioManager.PlaySFX("EnemyDeath");
        }
        this.tag = "Dead";
    }

    void DeathAnimationComplete(){
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag != "Ground" && other.gameObject.tag != "Player" && other.gameObject.tag != "Block"){
            Debug.Log(other.gameObject.tag);
            direction *= -1;
        }
        if(other.gameObject.tag == "Attack"){
            if(other.gameObject.GetComponent<Enemy>())
                other.gameObject.GetComponent<Enemy>().direction *= -1;
            Dead();
        }
    }

    void OnCollisionExit2D(Collision2D other) {
        if(enemyType == EnemyType.KoopaTroopa && isNotMoving)
            if(other.gameObject.tag == "Player" && jumpDeath){
                direction = other.gameObject.GetComponent<Player>().fireballOffset;
                speed = 4f;
                isNotMoving = false;
                this.tag = "Attack";
                startShellKillable = true;
            }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(this.tag != "Attack"){
            if(other.gameObject.tag == "Player"){
                jumpDeath = true;
                Dead();
            }
        }

    }
}
