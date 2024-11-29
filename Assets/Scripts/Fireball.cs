using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
   [SerializeField] Rigidbody2D rb;
   [SerializeField] Animator animator;
   public Player player;
	public Vector2 velocity;

    void Awake(){
        if(!player)
            player =  GameObject.Find("Player").GetComponent<Player>();
        velocity = new Vector2(player.fireballOffset * velocity.x, velocity.y);
    }

	// Use this for initialization
	void Start() 
    {
        Destroy(this.gameObject, 5);
        if(!rb)
            rb = GetComponent<Rigidbody2D>();
        
        if(!animator)
            animator = GetComponent<Animator>();
        rb.velocity = velocity;
	}

    void FixedUpdate(){
        if (rb.velocity.y < velocity.y)
			rb.velocity = velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = new Vector2 (velocity.x, -velocity.y);
        if (collision.gameObject.tag != "Ground") {
            if(collision.gameObject.tag == "Enemy")
                Destroy(collision.gameObject);
            animator.SetTrigger("Explode");
        }
    }

    void Explode()
    {
        Destroy(this.gameObject);
    }
}
