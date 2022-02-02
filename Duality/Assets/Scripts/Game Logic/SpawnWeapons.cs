using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnWeapons : MonoBehaviour
{
    [SerializeField]private GameObject WeaponSpawner;
    private void Start()
    {
        foreach (Transform weapon in GameObject.FindGameObjectWithTag("WeaponSpawnPoints").transform)
        {
           Instantiate(WeaponSpawner, weapon.transform.position, weapon.transform.rotation, weapon.transform);
        }
     
    }
}
