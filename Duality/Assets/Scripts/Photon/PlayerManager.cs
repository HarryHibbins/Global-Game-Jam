using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    private PhotonView PV;
    
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            createController();
        }
    }

    void Update()
    {
        
    }

    private void createController()
    {
        PhotonNetwork.Instantiate("PlayerController", new Vector3(0f,1.5f,0f), Quaternion.identity);
    }
}
