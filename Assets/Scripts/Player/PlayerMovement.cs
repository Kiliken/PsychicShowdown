using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] GameObject playerModel;
    public Transform playerCam;
    public bool playerActive = true;

    [Space(10)]

    [Header("Movement")]
    private float moveSpeed = 0f;
    [SerializeField] float moveSpeedDefault = 10f;
    //public float speedModifier = 0f;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    Transform orientation;
    private Quaternion targetModelRotation;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpCooldown = 0.25f; //cooldown in-between
    [SerializeField] float airMultiplier = 0.4f;
    private bool canJump = true;
    public int jumpsLeft = 3;
    public int maxJumps = 3;
    // [SerializeField] float jumpCdTime = 2f; // recharge cooldown
    // float jumpCdTimer = 0f;

    [Header("Dashing")]
    [SerializeField] float dashForce = 70f;
    [SerializeField] float dashCooldown = 0.5f;
    private bool canDash = true;
    public int dashesLeft = 3;
    public int maxDashes = 3;
    [SerializeField] float dashCdTime = 3f;
    public float dashCdTimer = 0f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask groundLayer;
    [SerializeField] bool grounded = false;
    [SerializeField] bool hitGround = false; // for jumping

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    //private bool exitingSlope;


    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        orientation = gameObject.transform;

        jumpsLeft = maxJumps;
    }


    // Update is called once per frame
    void Update(){
        if(playerActive){
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f, groundLayer);
            
            // restore player jumps once they landed
            if(grounded && !hitGround && canJump){
                hitGround = true;
                jumpsLeft = maxJumps;
                Debug.Log("Jumps recharged");
            }

            PlayerInput();
            

            // if(jumpsLeft != maxJumps){
            //     if(jumpCdTimer < jumpCdTime){
            //         jumpCdTimer += Time.deltaTime;
            //     }
            //     else{
            //         jumpsLeft++;
            //         jumpCdTimer = 0;
            //         Debug.Log("Jump Count Restored: "+ jumpsLeft);
            //     }
            // }

            if(dashesLeft != maxDashes){
                if(dashCdTimer < dashCdTime){
                    dashCdTimer += Time.deltaTime;
                }
                else{
                    dashesLeft++;
                    dashCdTimer = 0;
                    Debug.Log("Dash restored: " + dashesLeft);
                }
            }
        }
    }


    private void FixedUpdate(){
        if(playerActive){
            Move();
            SpeedControl();
            if (moveDirection != Vector3.zero){
                // calculate target rotation
                targetModelRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
            }
        }
    }


    private void LateUpdate(){
        // if(moveDirection != Vector3.zero){
        //     RotatePlayer();
        // }

        // player rotation
        playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, targetModelRotation, 10f * Time.deltaTime);
    }


    private void PlayerInput(){
        // set up for player 1 and 2 later
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(horizontalInput != 0 || verticalInput != 0)
            moveSpeed = moveSpeedDefault;
        else
            moveSpeed = 0f;

        // removed grounded since there is a triple jump
        if (Input.GetButtonDown("Jump") && canJump){
            if(jumpsLeft > 0){
                jumpsLeft--;
                Debug.Log("Jump Count: "+ jumpsLeft);
                Jump();
            }
            else{
                Debug.Log("Max jumped");
            }
        }

        if(Input.GetButtonDown("DashK1") && canDash){
            if(dashesLeft > 0){
                dashesLeft--;
                Debug.Log("Dash Count: "+ dashesLeft);
                Dash();
            }
            else{
                Debug.Log("Max dashed");
            }
        }
    }


    private void Move(){
        // get camera forward/right directions
        Vector3 camForward = playerCam.forward;
        Vector3 camRight = playerCam.right;
        camForward.y = 0; // no vertical rotation
        camRight.y = 0;

        // get relative directions
        Vector3 forwardRelative = verticalInput * camForward;
        Vector3 rightRelative = horizontalInput * camRight;

        //moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection = forwardRelative + rightRelative;

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }


    // controls the speed so that it doesn't go over the desired velocity
    private void SpeedControl(){
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    // private void RotatePlayer()
    // {
    //     // float angle = Mathf.Atan2(moveDirection.x, moveDirection.y);
    //     // angle = angle * Mathf.Rad2Deg;
    //     // playerModel.transform.eulerAngles = new Vector3(0, angle, 0);

    //     // Quaternion toRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
    //     // playerModel.transform.rotation = Quaternion.RotateTowards(playerModel.transform.rotation, toRotation, 720 * Time.deltaTime);
    // }


    private void Jump(){
        grounded = false;
        hitGround = false;
        canJump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        Invoke(nameof(ResetJump), jumpCooldown);
    }


    private void ResetJump(){
        //exitingSlope = false;
        canJump = true;
    }


    private void Dash(){
        canDash = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(horizontalInput != 0 || verticalInput != 0)
            rb.AddForce(moveDirection.normalized * dashForce, ForceMode.Impulse);
        else
            rb.AddForce(transform.forward * dashForce * 2, ForceMode.Impulse);

        Invoke(nameof(ResetDash), dashCooldown);
    }


    private void ResetDash(){
        canDash = true;
    }


    // see if slope movement is needed later
    private bool OnSlope(){
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }


    private Vector3 GetSlopeMoveDirection(){ 
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
