using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //variables used for walking and running
    [Header("Moving")]

    public float playerHeight = 2f;             //standard capsule height
    public float moveSpeed = 40f;               //the speed representing how fast the player is moving at any time
    public float crouchSpeed = 25f;
    public float walkSpeed = 40f;               // the standard walk speed
    public float sprintSpeed = 70f;             // speed when sprinting
    public float accel = 10f;                   // effects sprint speed up time
    public float rbDragGround = 6f;             // the drag the player rb experiences when grounded, higher stops slidey movement
    public float jumpHeight = 5f;               // initial impulse force behind player jump
    public float rbDragAir = 0.6f;              // the drag the player rb experiences when in air (should be much lower than ground drag to prevent floaty falling)
    //public float airSpeed = 2f;               
    float horiz;                                // AD input
    float vert;                                 // WS input

    public bool crouching;
    public bool sprinting;
    public bool sliding;
    public bool wallRunning;
    public bool scoped;

    Vector3 moveDirection;                      // vector formed from axis input
    Rigidbody rb;                               // the rigidbody

    public GameObject weaponHolder;

    // variables used for checking if the player is grounded
    [Header("Ground Detection")]

    public float groundDist = 0.4f;             //used for groundchecking, this value seems to work well even with slopes
    [SerializeField] bool isGrounded;                            // bool to show if the player is mid jump or has landed

    Vector3 slopeMoveDirection;                 // used to translate movedirection when on a slope or inclined surface
    
    public LayerMask groundMask;                //the layer mask for all walkable surfaces
    RaycastHit slopeHit;                        //raycast used for slope handling

    // variables used for looking around with the mouse

    [Header("Looking + Camera")]

    float xRot, yRot, mouseX, mouseY;           // used for rotating the player and cam with mouse input
    public float sensX;               //
    public float sensY;               // sensitivity values
    [SerializeField] float fov;
    [SerializeField] float wallRunFOV;
    [SerializeField] float wallRunFOVTime;
    [SerializeField] float camTilt;
    [SerializeField] float camTiltTime;
    [SerializeField] Camera fovCam;
    public float tilt { get; private set; }

    [SerializeField] Camera cam;             // the camera transform used by the player
    [SerializeField] Transform orientation;



    [Header("Wall Running")]
    [SerializeField] float wallDistance = 0.5f;
    [SerializeField] float minimumJumpHeight = 1.5f;
    public float wallRunGravity;

    public bool wallLeft = false;
    public bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;


    PlayerController playerController;


    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();                                 // get the rb component
        rb.freezeRotation = true;
        rb.drag = rbDragGround;                                         // set initial drag to the grounded value
        Cursor.lockState = CursorLockMode.Locked;                       // lock cursor movement
        Cursor.visible = false;                                         //and hide it

        playerController = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PlayerController>().isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;                       // lock cursor movement
            Cursor.visible = false;
            GetInput();                                                     // function to collect mouse and keyboard inputs

        }
        else 
        {
            Cursor.lockState = CursorLockMode.None;                       
            Cursor.visible = true;
        }

        cam.transform.localRotation = Quaternion.Euler(xRot, 0, tilt);                                                   // rotates the camera...
        transform.rotation = Quaternion.Euler(0f, yRot, 0f);                                                            //  ...and player to face the rotation governed by mouse input
        //Debug.Log("Moving cam by " + yRot);
        if(!crouching)
            isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDist, groundMask);            // boolean is set by casting a checksphere at the players feet and checking...
        else                                                                                                                //  ...if the ground is within a small distance.
            isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 0.5f, 0), groundDist, groundMask);

        if (crouching)
        {
            transform.localScale = new Vector3(1f, 0.5f, 1f);
            weaponHolder.transform.localScale = new Vector3(1f, 2f, 1f);
        }
        else 
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            weaponHolder.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !GetComponent<PlayerController>().isPaused) 
        {
            PlayerJump();                                                                               // if the player is grounded and presses space the jump function is called
        }

        if (isGrounded) 
        {
            rb.drag = rbDragGround;                                                                     // sets the drag on the rb to the right value when on the ground
            rb.useGravity = false;                                                                      // need to disable gravity when grounded to prevent sliding down slopes
            if (sprinting)                                                        // sprint key when on ground
            {
                crouching = false;
                moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, accel * Time.deltaTime);                 // lerps the current move speed to the sprint speed
            }
            else
            {
                if (!crouching)
                    moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, accel * Time.deltaTime);                // then back to walk speed when not sprinting
                else 
                {
                    if (moveSpeed > 20 && sliding)
                    {
                        fovCam.fieldOfView = Mathf.Lerp(fovCam.fieldOfView, wallRunFOV, wallRunFOVTime * Time.deltaTime);
                        moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, 2 * Time.deltaTime);

                    }
                    else 
                    {
                        fovCam.fieldOfView = Mathf.Lerp(fovCam.fieldOfView, fov, wallRunFOVTime * Time.deltaTime);
                        moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, 100 * Time.deltaTime);
                        sliding = false;
                        tilt = Mathf.Lerp(tilt, 0f, camTiltTime * Time.deltaTime);

                    }
                    

                }
                    
            }

        }
        else
        {

            rb.useGravity = true;                                                                       // use gravity when in mid air
            rb.drag = rbDragAir;                                                                        // set the rb drag to air value
        }
        if (sprinting)
        {
            fovCam.fieldOfView = Mathf.Lerp(fovCam.fieldOfView, 70, wallRunFOVTime * Time.deltaTime);
        }
        else 
        {
            fovCam.fieldOfView = Mathf.Lerp(fovCam.fieldOfView, 60, wallRunFOVTime * Time.deltaTime);
        }
        CheckWall();
    }


    void GetInput() 
    {
        //if (isGrounded)
       // {
        horiz = Input.GetAxisRaw("Horizontal");                                                         // AD inputs
        vert = Input.GetAxisRaw("Vertical");                                                            // WS inputs
        if (CanWallRun() && wallLeft)
        {
            horiz = Mathf.Clamp(horiz, 0f, 3f);
        }
        else if (CanWallRun() && wallRight)
        {
            horiz = Mathf.Clamp(horiz, -3f, 0f);
        }
        moveDirection = orientation.forward * vert + orientation.right * horiz;                         // creates a vector from the 2 axis 
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);                    // alternate vector if on a slope
      //  }


        mouseX = Input.GetAxisRaw("Mouse X");                                                           // mouse swipes
        mouseY = Input.GetAxisRaw("Mouse Y");                                                           //

        //Debug.Log("Moving cam by " + mouseY + " and " + mouseX);

        yRot += mouseX * sensX;                                                                         // increase input scale by sensitivity values
        xRot -= mouseY * sensY;                                                                         //

        xRot = Mathf.Clamp(xRot, -90f, 90f);                                                            // clamp xRot values so players cant snap their backs

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (!crouching)
            {
                crouching = true;
                sprinting = false;
                if (moveSpeed > (sprintSpeed * 0.9f))
                {
                    Debug.Log("Made it to function call!");
                    sliding = true;
                    PlayerSlide();
                }


            }
            else 
            {
                crouching = false;
            }
            
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) 
        {
            if (!sprinting)
            {
                crouching = false;
                sprinting = true;
                
            }
            else
            {
                sprinting = false;
            }

        }
    }

    private void FixedUpdate()
    {
        if (!GetComponent<PlayerController>().isPaused) 
        {

            MovePlayer();

        }                                                                      // called in fixedupdate because rigidbodies?
    }

    void MovePlayer()                                                                                                       // function to move player according to the inputs gathered
    {
        if(isGrounded && !OnSlope())
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Acceleration);                                      // on flat ground, move in the indicated direction in current speed
        else if (isGrounded && OnSlope())
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed, ForceMode.Acceleration);                                 // on a slope, adjust velocity accordingly 
        else
            rb.AddForce(moveDirection.normalized * ((rbDragAir / rbDragGround) * moveSpeed), ForceMode.Acceleration);       // if in the air, move at a speed consistent with ground speed, using the...
                                                                                                                            //  ...ratio of the drag values to calculate air speed.
    }

    void PlayerJump() 
    {
        rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);              // applies an impulse force to launch the player upwards with an initial velocity governed by jumpHeight
    }

    void PlayerSlide() 
    {
        sliding = true;
        Debug.Log("Sliding?");
        rb.AddForce(transform.forward * jumpHeight * 25, ForceMode.Force);

        tilt = Mathf.Lerp(tilt, 10f, camTiltTime * Time.deltaTime);


    }

    private bool OnSlope()                                                                                                  // function to determine if the player is on a slope or not
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.4f))                       // if there is a surface below the player to check, info is stored in slopeHit
        {
            if (slopeHit.normal != Vector3.up)                                                                              // if the normal doesnt point straight up, then its a slope
                return true;
            else
                return false;
            
                    
        }
        return false;
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);
        if (CanWallRun())
        {
            if (wallLeft)
                StartWallRun();
            else if (wallRight)
                StartWallRun();
            else
                EndWallRun();

        }
        else
            EndWallRun();

    }

    bool CanWallRun() 
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);            
    }


    void StartWallRun() 
    {
        wallRunning = true;
        rb.useGravity = false;

        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        fovCam.fieldOfView = Mathf.Lerp(fovCam.fieldOfView, wallRunFOV, wallRunFOVTime * Time.deltaTime);

        if (wallLeft) 
        {
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);        
        }
        else if (wallRight)
        {
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if (wallLeft) 
            {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * jumpHeight * 50, ForceMode.Force);
            }
            else if (wallRight)
            {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * jumpHeight * 50, ForceMode.Force);
            }
        }
    
    }

    void EndWallRun() 
    {
        wallRunning = false;
        rb.useGravity = true;
        fovCam.fieldOfView = Mathf.Lerp(fovCam.fieldOfView, fov, wallRunFOVTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0f, camTiltTime * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Net") 
        {
            transform.position = new Vector3(0, 4, 0);        
        }
    }
}
