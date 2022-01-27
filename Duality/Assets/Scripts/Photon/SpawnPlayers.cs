using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class SpawnPlayers : MonoBehaviour
{

    private void Start()
    {
        //Instantiate on server 
        PhotonNetwork.Instantiate("PlayerManager", Vector3.zero, Quaternion.identity);
    }
}
