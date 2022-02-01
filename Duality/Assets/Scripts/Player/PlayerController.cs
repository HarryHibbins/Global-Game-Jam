using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private float sprintSpeed, walkSpeed, jumpForce, smoothTime;
    public float currentSensitivity/*, unScopedSensitivity, scopedSensitivity*/;
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

    public PlayerManager playerManager;
    public GameObject weapon;
    public GameLogic gameLogic;

    public GameObject scoreboard;
    public GameObject pauseMenu;
    public bool isPaused = false;

    public PlayerSettings ps;

    private void Awake()
    {
        currentSensitivity = ps.unscopedSensitivity;
        //Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        groundCheck = GetComponentInChildren<GroundCheckTag>().transform;
        PV = GetComponent<PhotonView>();

        //Gets the player manager for the local player
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();

        gameLogic = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogic>();
        scoreboard = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogic>().sb;
        pauseMenu = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogic>().pm;


        if (PV.IsMine)
        {
            //transform.Find("CameraHolder/RecoilCam/WeaponHolder").GetComponentInChildren<WeaponScript>().AssignRecoilCam();
            weapon.GetComponent<WeaponScript>().AssignRecoilCam();
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
        pauseMenu.SetActive(false);
    }

    void Update()
    {

        //Only controller local player
        if (!PV.IsMine)
        {
            return;
        }

        if (!isPaused)
        {
            //Look();
            //Move();
            //Jump();
        }
        

        if (Input.GetKey(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        }
        else
        { 
            scoreboard.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.Escape) && !isPaused)
        {
            pauseMenu.SetActive(true);
            isPaused = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (Input.GetKeyUp(KeyCode.Escape) && isPaused)
        {
            pauseMenu.SetActive(false);
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void FixedUpdate()
    {
        //Only control local player
        if (!PV.IsMine)
        {
            return;
        }
        //rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }


    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * currentSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * currentSensitivity;
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

        //rb.AddForce(moveAmount);

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
    public void TakeDamage(float damage, string weaponname)
    {
        //Calls the RPC called RPC_TakeDamage
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, weaponname);
    }

    //Runs on everyone elses computer, but on the person hit will receive the damage because of the if !PV.Mine
    [PunRPC]
    void RPC_TakeDamage(float damage, string weaponname, PhotonMessageInfo info)
    {
        if (!PV.IsMine)
        {
            return;
        }
        Debug.Log("Took Damage: "+ damage);

        currentHealth -= damage;
        //Gives a value between 0 and 1 for health 
        healthBarImage.fillAmount = currentHealth / maxHealth;

        /*Debug.Log("Player who shot you: " + info.Sender.NickName);
        Debug.Log(info.Sender);
        Debug.Log(info.photonView.ViewID);*/

        if (currentHealth <= 0)
        {
            gameLogic.UpdatePlayer(info.Sender, 1, 0);
            gameLogic.UpdatePlayer(PV.Owner, 0, 1);

            gameLogic.AddKillfeed(info.Sender, PV.Owner, weaponname);

            //Debug.Log(info.Sender);
            playerManager.Die();
        }
    }
}
