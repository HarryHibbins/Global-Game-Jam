using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    private Transform playerBody;
    private float xRotation = 0f;
    
    [SerializeField] PlayerInstance playerInstance;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInstance = transform.root.GetComponent<PlayerInstance>();
        playerBody = transform.root;

    }

    void Update()
    {
        if (playerInstance.view.IsMine)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerBody.Rotate(Vector3.up * mouseX);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

    }
}
