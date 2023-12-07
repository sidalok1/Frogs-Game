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
    
    private float modjmp;
    private Rigidbody2D rb;
    public DistanceJoint2D joint;
    [SerializeField] private Rigidbody2D head; 
    //public Tongue tongue;
    public LineRenderer tongue;
    public Transform firePoint;
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
    private bool charging;
    private bool attacking;
    private bool isGrappling;
    private Dir wall;
    private Vector3 scaleLeft;
    private Vector3 scaleRight;
    // private int[] iter = {0, 0};

    private Coroutine coroutine;

    private RaycastHit2D _hit;
    private float i;
    private Vector2 p;
    private Vector2 target;

    [Header("Tongue Distance:")]
    [SerializeField] private float maxDistnace = 50;

    [Header("Speed Values:")]
    [SerializeField] private float speed;
    //Speed is the acceleration factor to be applied to the input in order to calculate the impulse added
    [SerializeField] private float spdLimit;
    //If the current x - velocity exceeds the speed limit, do not add force. Only works with drag and friction.
    [SerializeField] private float jmpVal;
    [SerializeField] private float rapSpeed;



    void Start() {
        startPosition = transform.position;

        rb = GetComponent<Rigidbody2D>(); 
        //tongue.GetComponent<LineRenderer>();
        //joint = GetComponent<DistanceJoint2D>();
        animator.SetBool("isMoving", false);
        facing = Dir.Right;
        modjmp = jmpVal * 0.8f;
        scaleLeft = new Vector3(-1f, 1f, 1f);
        scaleRight = new Vector3(1f, 1f, 1);

        tongue.enabled = false;
        joint.enabled = false;
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
        if (isGrappling == true) {
            tongue.SetPosition(0, firePoint.position);
        } else {
            animator.SetBool("Attack", false);
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
        if (joint.enabled) {
            joint.distance += movY * rapSpeed * Time.deltaTime;
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
        movY = v.y;
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
        if (angle > 10f && angle < 170f) {
            animator.SetBool("Forward", false);
            animator.SetBool("Up", true);
        } else if (angle > 215f && angle < 325f) {
            animator.SetBool("Forward", false);
            animator.SetBool("Down", true);
        } else {
            animator.SetBool("Up", false);
            animator.SetBool("Down", false);
            animator.SetBool("Forward", true);
        }
    }

    void OnFire() {
        charging = !charging;
        if (charging == true) {
            animator.SetBool("Charging", true);
            animator.SetBool("Attack", false);
            Debug.Log("Charging");
        }
        else {
            animator.SetBool("Charging", false);
            animator.SetBool("Attack", true);
            isGrappling = true;
            SetGrapplePoint();
            Debug.Log("Not Charging");
        }
    }

    void OnCancel() {
        if (isGrappling) {
            isGrappling = false;
            StopAllCoroutines();
            tongue.enabled = false;
            joint.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (isGrappling == true) {
            isGrappling = false;
            StopAllCoroutines();
            tongue.enabled = false;
            joint.enabled = false;
        }
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
        if (other.gameObject.CompareTag("Spike")) {
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

    IEnumerator sendTongue(Vector2 t) {
        Debug.Log("Coroutine");
        if (i <= 1f) {
            p = Vector2.Lerp(firePoint.position, t, Time.deltaTime);
            tongue.SetPosition(1, p);
            i = i + 0.1f;
            yield return null;
        } else {
            tongue.SetPosition(1, t);
            yield break;
        }
        
    }
    IEnumerator recieveTongue() {
        if (i >= 0f) {
            p = Vector2.Lerp(firePoint.position, target, i);
            tongue.SetPosition(1, p);
            i -= Time.deltaTime * 0.01f;
        } else {
            tongue.SetPosition(1, firePoint.position);
            tongue.enabled = false;
            yield break;
        }
        yield return null;
    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = cam.ScreenToWorldPoint(Input.mousePosition) - firePoint.position;
        tongue.enabled = true;
        tongue.SetPosition(0, firePoint.position);
        tongue.SetPosition(1, firePoint.position);
        
    
        _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
        Debug.Log("Casted");
        if (_hit.collider != null) {
            Debug.Log("Hit");
            Debug.Log(_hit.distance);
            Debug.Log(_hit.point);
            if (_hit.distance <= maxDistnace) {
                target = _hit.point;
                i = 0f;
                //coroutine = StartCoroutine(sendTongue(target));
                tongue.SetPosition(1, target);
                joint.enabled = true;
                joint.anchor = Vector2.zero;
                joint.connectedAnchor = _hit.point;
                //joint.connectedBody = _hit.rigidbody;
                //joint.distance = _hit.distance;
                
            } else {
                tongue.enabled = false;
            }
        } else {
            Debug.Log("Missed");
            target = distanceVector;
            i = 1f;
            tongue.enabled = false;
            isGrappling = false;
            //coroutine = StartCoroutine(recieveTongue());
        }
    }
}
