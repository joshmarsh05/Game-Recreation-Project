using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject spawn;
    public Vector2 offset;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
            Instantiate(spawn, new Vector2(transform.position.x + offset.x, transform.position.y + offset.y), transform.rotation);
        Destroy(this.gameObject);
    }
}
