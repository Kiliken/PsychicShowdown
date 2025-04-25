using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public bool playerActive = true;
    [Header("Movement")]
    private float moveSpeed = 0f;
    [SerializeField] float moveSpeedDefault = 10f;
    //public float speedModifier = 0f;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    Transform orientation;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpCooldown = 0.25f;
    //[SerializeField] float airMultiplier = 0.4f;
    private bool canJump = true;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask groundLayer;
    bool grounded = false;

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    //private bool exitingSlope;


    [Space(10)]
    Rigidbody rb;


    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        orientation = gameObject.transform;
    }


    // Update is called once per frame
    void Update(){
        if(playerActive){
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
            PlayerInput();
        }
    }


    private void FixedUpdate(){
        if(playerActive){
            Move();
            SpeedControl();
        }
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
            canJump = false;
            //Debug.Log("jump");
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }


    private void Move(){
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
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

    private void Jump(){
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }


    private void ResetJump()
    {
        //exitingSlope = false;
        canJump = true;
    }


    // see if slope movement is needed later
    private bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }


    private Vector3 GetSlopeMoveDirection() { 
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
