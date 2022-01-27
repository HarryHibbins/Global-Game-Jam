using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    private PhotonView PV;

    private GameObject controller;

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


    private void createController()
    {
        controller = PhotonNetwork.Instantiate("PlayerController", new Vector3(0f,1.5f,0f), Quaternion.identity, 0, new object[] {PV.ViewID});
    }

    public void Die()
    {
        //Destroy the player
        PhotonNetwork.Destroy(controller);
        //Respawn
        createController();
        
    }
}
