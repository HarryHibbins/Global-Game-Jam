using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    private void Start()
    {
        //Instantiate on server 
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 3, 0), Quaternion.identity);
    }
}
