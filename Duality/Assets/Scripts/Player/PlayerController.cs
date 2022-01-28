using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{

    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private float mouseSensitivty, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] private Image healthBarImage;
    private float verticalLookRotation;
    private bool isGrounded;
    private Vector3 smoothMoveVelocity, moveAmount;
    
    private Rigidbody rb;
    
    private Transform groundCheck;
    private float groundDistance = 0.4f;
    public LayerMask groundMask;

    private PhotonView PV;

    private const float maxHealth = 100f;
    private float currentHealth = maxHealth;

    private PlayerManager playerManager;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        groundCheck = GetComponentInChildren<GroundCheckTag>().transform;
        PV = GetComponent<PhotonView>();

        //Gets the player manager for the local player
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();

        if (PV.IsMine)
        {
            transform.Find("CameraHolder/RecoilCam/WeaponHolder").GetComponentInChildren<WeaponScript>().AssignRecoilCam();
        }
    }

    void Start()
    {

        if (!PV.IsMine)
        {
            //Destroy the camera and rigidbody of the other player 
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            //Destroy(transform.Find("CameraHolder/RecoilCam/WeaponCam/WeaponHolder").gameObject);
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

        if (transform.position.y < -20f)
        {
            playerManager.Die();
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    //Runs on the shooters computer
    public void TakeDamage(float damage)
    {
        //Calls the RPC called RPC_TakeDamage
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    //Runs on everyone elses computer, but on the person hit will receive the damage because of the if !PV.Mine
    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine)
        {
            return;
        }
        Debug.Log("Took Damage: "+ damage);

        currentHealth -= damage;
        //Gives a value between 0 and 1 for health 
        healthBarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            playerManager.Die();
        }
    }


}
