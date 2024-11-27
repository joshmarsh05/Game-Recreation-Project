using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
   [SerializeField] Rigidbody2D rb;
	public GameObject explosion;
	public Vector2 velocity;

	// Use this for initialization
	void Start() 
    {
        Destroy(this.gameObject, 10);
        if(!rb)
            rb = GetComponent<Rigidbody2D>();
        velocity = rb.velocity;
	}
	
	void Update() 
    {
        if (rb.velocity.y < velocity.y)
            rb.velocity = velocity;
	}


    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = new Vector2 (velocity.x, -velocity.y);
        if (collision.gameObject.tag == "Enemy") {
            Destroy(collision.gameObject);
            Explode();
        }
    }

    void Explode()
    {
        Instantiate (explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
