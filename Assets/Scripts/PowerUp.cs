using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private float direction = 1;
    public float speed = 1f;
    public PowerUpType powerUp;
    [SerializeField] Rigidbody2D rb;

    void Strat(){
        if(!rb)
            rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(7, 8);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag != "Ground" && other.gameObject.tag != "Block")
            direction *= -1;
    }
}
