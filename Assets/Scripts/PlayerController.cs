using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private float speed;
    //Speed is the acceleration factor to be applied to the input in order to calculate the impulse added

    [SerializeField] private float spdLimit;
    //If the current x - velocity exceeds the speed limit, do not add force. Only works with drag and friction.
    [SerializeField] private float jmpVal;
    private Rigidbody2D rb;
    private float movX;
    private float movY;
    private Vector2 movForce;
    private bool facingLeft;
    //If player puts left input while facing right and vice versa, the sprite flips directions
    private bool canJump;
    // private int[] iter = {0, 0};
    void Start() {
        rb = GetComponent<Rigidbody2D>(); 
        animator.SetBool("isMoving", false);
        facingLeft = false;
    }

    private void Update() {
        if (Mathf.Abs(rb.velocity.x) > 0.1f) {
            animator.SetBool("isMoving", true);
        } else {
            animator.SetBool("isMoving", false);
        }
        if (rb.velocity.y > 0.5f) {
            animator.SetBool("ascending", true);
            /* iter[0] += 1;
            if (iter[0] == 3) {
                // animator.SetBool("ascending", true);
                iter[0] = 0;
            } */
        } else {
            // iter[0] = 0;
        }
        if (rb.velocity.y < -0.5f) {
            animator.SetBool("falling", true);
            animator.SetBool("ascending", false);
            // iter[1] += 1;
            /* if (iter[1] == 3) {
                animator.SetBool("falling", true);
                animator.SetBool("ascending", false);
                iter[1] = 0;
            } */
        } else {
            // iter[1] = 0;
        }
        if (canJump == true) {
            
            animator.SetBool("ascending", false);
            animator.SetBool("falling", false);
        }
        Debug.Log(rb.velocity.y);
    }

    private void FixedUpdate() {
        //Need to add jump and ensure that jumping is not contingent on being under speed limit
        if (Mathf.Abs(rb.velocity.x) < spdLimit) {
            movForce = new Vector2(movX, 0.0f);
            rb.AddForce(movForce * speed, ForceMode2D.Impulse);
        }
        
    }

    void OnMove(InputValue mov) {
        Vector2 v = mov.Get<Vector2>();

        if (v.x > 0.0f && facingLeft) {
            transform.Rotate(0.0f, -180.0f, 0.0f, Space.Self);
            facingLeft = false;
        } else if (v.x < 0.0f && !facingLeft) {
            transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            facingLeft = true;
        }
        
        movX = v.x;
        
    }

    void OnJump () {
        if (canJump) {
            movForce = new Vector2(0.0f, jmpVal);
            rb.AddForce(movForce, ForceMode2D.Impulse);
        } 
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ground")) {
            canJump = true;
            animator.SetBool("onGround", true);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ground")) {
            canJump = false;
            animator.SetBool("onGround", false);
        }
    }
}
