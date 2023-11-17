using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    enum Dir {Left, Right};
    public Animator animator;

    public Vector3 startPosition;
    [SerializeField] private float speed;
    //Speed is the acceleration factor to be applied to the input in order to calculate the impulse added

    [SerializeField] private float spdLimit;
    //If the current x - velocity exceeds the speed limit, do not add force. Only works with drag and friction.
    [SerializeField] private float jmpVal;
    private float modjmp;
    private Rigidbody2D rb;
    [SerializeField] private Rigidbody2D head; 
    [SerializeField] private Camera cam;
    private float movX;
    private float movY;
    private Vector2 movForce;
    private Vector2 mousePos;
    private float angle;
    //If player puts left input while facing right and vice versa, the sprite flips directions
    private Dir facing;
    private bool canJump;
    private bool wallJump;
    private bool jumping;
    private Dir wall;
    private Vector3 scaleLeft;
    private Vector3 scaleRight;
    // private int[] iter = {0, 0};
    void Start() {
        startPosition = transform.position;

        rb = GetComponent<Rigidbody2D>(); 
        animator.SetBool("isMoving", false);
        facing = Dir.Right;
        modjmp = jmpVal * 0.8f;
        scaleLeft = new Vector3(-1f, 1f, 1f);
        scaleRight = new Vector3(1f, 1f, 1);
    }

    private void Update() {
        if (Mathf.Abs(rb.velocity.x) > 0.1f) {
            animator.SetBool("isMoving", true);
        } else {
            animator.SetBool("isMoving", false);
        }
        if (rb.velocity.y > 0.5f) {
            animator.SetBool("ascending", true);
        }
        if (rb.velocity.y < -0.5f) {
            animator.SetBool("falling", true);
            animator.SetBool("ascending", false);
        }
        if (canJump == true) {
            animator.SetBool("ascending", false);
            animator.SetBool("falling", false);
        }
        
        //Debug.Log(rb.velocity.y);
    }

    private void FixedUpdate() {
        if (Mathf.Abs(rb.velocity.x) < spdLimit) {
           movForce = new Vector2(movX, 0.0f);
           rb.AddForce(movForce * speed, ForceMode2D.Impulse);
        }
        if (jumping) {
            jump();
        }
        
        
        
    }

    void jump() {
        if (canJump) {
            rb.velocity = new Vector2(rb.velocity.x, jmpVal);
            canJump = false;
            animator.SetBool("onGround", false); 
            return;
        } else if (wallJump) {
            if (wall == Dir.Left) {
                rb.velocity = new Vector2(modjmp, jmpVal);
            } else if (wall == Dir.Right) {
                rb.velocity = new Vector2(-modjmp, jmpVal);
            }
        }
    }
    void OnJump () {
        jumping = !jumping;
    }

    void OnMove(InputValue mov) {
        Vector2 v = mov.Get<Vector2>();
        movX = v.x;
    }

    void OnLook(InputValue look) {
        Vector2 p = look.Get<Vector2>();
        p = cam.ScreenToWorldPoint(p);
        mousePos = rb.position - p;
        if (mousePos.x < 0.0f && facing == Dir.Left) {
            //transform.Rotate(0.0f, -180.0f, 0.0f, Space.Self);
            transform.localScale = scaleRight;
            facing = Dir.Right;
        } else if (mousePos.x > 0.0f && facing == Dir.Right) {
            //transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            transform.localScale = scaleLeft;
            facing = Dir.Left;
        }
        angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg + 180f;
        if (angle > 90f && angle < 270f) {
            head.SetRotation(angle - 180f);
        } else {
            head.SetRotation(angle);
        }
    }

    

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ground")) {
            canJump = true;
            animator.SetBool("onGround", true);
        }
        if (other.gameObject.CompareTag("Wall")) {
            ContactPoint2D conPoint = other.GetContact(other.contactCount - 1);
            if (conPoint.normal.y > 0) {
                canJump = true;
                animator.SetBool("onGround", true);
            }else if (conPoint.normal.x < 0) {
                wallJump = true;
                wall = Dir.Right;
            } else if (conPoint.normal.x > 0) {
                wallJump = true;
                wall = Dir.Left;
            }
        }
        if (other.gameObject.CompareTag("Spike") || other.gameObject.CompareTag("Fire") || other.gameObject.CompareTag("Block")) {
            // player should go back to the restart point
            transform.position = startPosition;
            Debug.Log("colliding");
        }
        if (other.gameObject.CompareTag("Enemy")) {
            // player should go back to the restart point
            transform.position = startPosition;
            Debug.Log("colliding");
        }
        if (other.gameObject.CompareTag("ReactorPart")) {
            SceneManager.LoadScene(2);
        }
        
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ground")) {
            /* canJump = false;
            animator.SetBool("onGround", false); */
        }
        if (other.gameObject.CompareTag("Wall")) {
            wallJump = false;
        }
    }
}
