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
        foreach (var weapon in WeaponSpawnPointManager.Instance.spawnPoints)
        {
           Instantiate(WeaponSpawner, weapon.transform.position, weapon.transform.rotation, weapon.transform);
        }
     
    }
}
