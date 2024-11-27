using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;

    private void Start() {
        if(!animator)
            animator = GetComponent<Animator>();
        if(!rb)
            rb = GetComponent<Rigidbody2D>();
    }

    public void Dead(){
        animator.SetTrigger("Die");
    }

    
}
