using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    // References
    private PhotonView PV;
    private Player player;
    private GameObject controller;

    //Player Stats
    public string NickName;
    public string ID;
    public int kills;
    public int deaths;

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

        NickName = PV.Owner.NickName;
        //ID = PV.Owner.UserId;
    }

    private void createController()
    {
        Transform spawnPoint = SpawnPointManager.Instance.getRandomSpawnPoint();
        controller = PhotonNetwork.Instantiate("PlayerController", spawnPoint.position, spawnPoint.rotation, 0, new object[] {PV.ViewID});
    }

    public void Die()
    {
        //Destroy the player
        PhotonNetwork.Destroy(controller);
        deaths++;
        //Respawn
        createController();
        
    }
}
