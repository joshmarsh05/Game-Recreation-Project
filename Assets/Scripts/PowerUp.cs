using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private float direction = 1;
    public float speed = 1f;
    [SerializeField] Rigidbody2D rb;

    void Strat(){
        if(!rb)
            rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        direction *= -1;
    }
}
