using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float speed;
    //Speed is the acceleration factor to be applied to the input in order to calculate the impulse added

    [SerializeField] private float spdLimit;
    //If the current x - velocity exceeds the speed limit, do not add force. Only works with drag and friction.
    private Rigidbody2D rb;
    private float movX;
    private float movY;
    private Vector2 movForce;
    private bool facingLeft = false;
    //If player puts left input while facing right and vice versa, the sprite flips directions
    void Start() {
        rb = GetComponent<Rigidbody2D>(); 
    }

    private void FixedUpdate() {
        //Need to add jump and ensure that jumping is not contingent on being under speed limit
        if (Mathf.Abs(rb.velocity.x) < spdLimit) {
            movForce = new Vector2(movX, movY);
            rb.AddForce(movForce * speed, ForceMode2D.Impulse);
        }
        

    }

    void OnMove(InputValue mov) {
        Vector2 v = mov.Get<Vector2>();

        if (v.x > 0.0f && facingLeft) {
            transform.Rotate(0.0f, -180.0f, 0.0f, Space.Self);
            Debug.Log(1);
            facingLeft = false;
        } else if (v.x < 0.0f && !facingLeft) {
            Debug.Log(2);
            transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            facingLeft = true;
        }
        
        movX = v.x;
        
    }
}
