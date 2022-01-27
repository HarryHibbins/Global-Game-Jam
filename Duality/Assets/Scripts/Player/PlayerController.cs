using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private float mouseSensitivty, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    private float verticalLookRotation;
    private bool isGrounded;
    private Vector3 smoothMoveVelocity, moveAmount;
    
    private Rigidbody rb;
    
    private Transform groundCheck;
    private float groundDistance = 0.4f;
    public LayerMask groundMask;

    private PhotonView PV;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        groundCheck = GetComponentInChildren<GroundCheckTag>().transform;
        PV = GetComponent<PhotonView>();

        if (!PV.IsMine)
        {
            //Destroy the camera and rigidbody of the other player 
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }

    void Update()
    {

        //Only controller local player
        if (!PV.IsMine)
        {
            return;
        }
        Look();
        Move();
        Jump();

    }

    private void FixedUpdate()
    {
        //Only control local player
        if (!PV.IsMine)
        {
            return;
        }
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }


    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivty);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivty;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void Move()
    {        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        //If holding shift use sprint speed, if not use walk
        moveAmount = Vector3.SmoothDamp(moveAmount,
            moveDirection * (Input.GetButton("Sprint") ? sprintSpeed : walkSpeed),
            ref smoothMoveVelocity, smoothTime);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }



}
