using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnHealthPacks : MonoBehaviour
{

    void Start()
    {
        //Instantiate on server 
        
        //Transform spawnPoint = HealthSpawnPointManager.Instance.getRandomSpawnPoint();
        foreach (var healthpack in HealthSpawnPointManager.Instance.spawnPoints)
        {
            //PhotonNetwork.Instantiate("HealthPack", healthpack.transform.position, healthpack.transform.rotation,0, new object[] {PV.ViewID});
          //  Debug.Log("Spawn health");
        }
        
        
       
    }

    
}
