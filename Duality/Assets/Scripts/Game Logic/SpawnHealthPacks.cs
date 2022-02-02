using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnHealthPacks : MonoBehaviour
{

    [SerializeField]private GameObject HealthSpawner;
    private void Start()
    {
        foreach (Transform HealthPack in GameObject.FindGameObjectWithTag("HealthSpawnPoints").transform)
        {
            Instantiate(HealthSpawner, HealthPack.transform.position, HealthPack.transform.rotation, HealthPack.transform);
        }
     
    }

    
}
