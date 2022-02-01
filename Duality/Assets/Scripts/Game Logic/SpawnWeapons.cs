using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnWeapons : MonoBehaviour
{
    [SerializeField]private List<PlayerController> players;
    [SerializeField]private GameObject WeaponSpawner;
    private void Start()
    {
        
        
        /*
        GameObject[] pm = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in pm)
        {
            players.Add(player.GetComponent<PlayerController>());
            Debug.Log(player.GetComponent<PlayerController>().playerManager.NickName);
        }*/
        


       // foreach (var player in players)
        //{
        foreach (var weapon in WeaponSpawnPointManager.Instance.spawnPoints)
        {
           // PhotonNetwork.Instantiate("WeaponSpawn", weapon.transform.position, weapon.transform.rotation);
           Instantiate(WeaponSpawner, weapon.transform.position, weapon.transform.rotation, weapon.transform);
            Debug.Log("Spawn weapon");
        }
        //}
     
    }
}
