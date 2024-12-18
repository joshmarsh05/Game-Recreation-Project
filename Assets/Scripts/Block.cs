using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType{
    Unbreakable = 0,
    PowerUp = 1,
    Coin = 2,
    MultiCoin = 3,
    Empty = 4
}

public enum PowerUpType{
    Mushroom = 0,
    FireFlower = 1,
    Star = 2
}

public class Block : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] AudioManager audioManager;
    public Animator animator;
    public BlockType block;
    public PowerUpType powerUp;
    public GameObject[] powerUps = new GameObject[3];
    private int multiCoinHits = 7;
    private int hitCount = 0;
    public bool question = false;
    public float yOffset = 5f;

    void Start()
    {
        if(!animator)
            animator = GetComponent<Animator>();
        if(question)
            animator.SetBool("Question", true);
        if(!gameManager)
            gameManager = GameObject.Find("HUD").GetComponent<GameManager>();
        if(!audioManager)
            audioManager = GameObject.Find("HUD").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger entered with: " + other.name);
        if (other.gameObject.tag == "Player"){
            if(block == BlockType.Empty){
                animator.SetTrigger("Break");
                audioManager.PlaySFX("Break");
            } else if(block == BlockType.PowerUp){
                animator.SetBool("Unbreakable", true);
                block = BlockType.Unbreakable;
                audioManager.PlaySFX("PowerUpSpawn");
                if(powerUp == PowerUpType.Mushroom)
                    Instantiate(powerUps[0], new Vector3(transform.position.x, transform.position.y + yOffset, 0), transform.rotation);
                else if(powerUp == PowerUpType.FireFlower)
                    Instantiate(powerUps[1], new Vector3(transform.position.x, transform.position.y + yOffset, 0), transform.rotation);
                else if(powerUp == PowerUpType.Star)
                    Instantiate(powerUps[2], new Vector3(transform.position.x, transform.position.y + yOffset, 0), transform.rotation);

            } else if(block == BlockType.Coin){
                animator.SetBool("Unbreakable", true);
                block = BlockType.Unbreakable;
                gameManager.AddCoin();
                audioManager.PlaySFX("Coin");
            } else if(block == BlockType.MultiCoin){
                animator.SetTrigger("Hit");
                audioManager.PlaySFX("Coin");
                hitCount++;
                gameManager.AddCoin();
                if(hitCount == multiCoinHits){
                    animator.SetBool("Unbreakable", true);
                    block = BlockType.Unbreakable;
                }
            }
        }
    }

    void AnimationComplete(){
        Destroy(gameObject); 
    }
}
