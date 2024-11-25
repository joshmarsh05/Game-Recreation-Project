using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType{
    Unbreakable = 0,
    PowerUp = 1,
    Coin = 2,
    MultiCoin,
    Empty = 3
}

public class Block : MonoBehaviour
{
    public Animator animator;
    public BlockType block;
    public GameObject[] powerUps = new GameObject[3];
    private int multiCoinHits = 7;
    private int hitCount = 0;

    void Start()
    {
        if(!animator)
            animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger entered with: " + other.name);
        if (other.gameObject.tag == "Player"){
            if(block == BlockType.Empty){
                animator.SetTrigger("Break");
            } else if(block == BlockType.PowerUp){
                animator.SetBool("Unbreakable", true);
                block = BlockType.Unbreakable;
            } else if(block == BlockType.Coin){
                animator.SetBool("Unbreakable", true);
                block = BlockType.Unbreakable;
            } else if(block == BlockType.MultiCoin){
                animator.SetTrigger("Hit");
                hitCount++;
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
