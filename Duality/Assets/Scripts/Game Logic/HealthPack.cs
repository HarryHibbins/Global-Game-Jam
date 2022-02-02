using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] GameObject modelPrefab;
    [SerializeField] GameObject modelHolder;
    [SerializeField] private GameObject healthPackHolder;
    [SerializeField] GameObject spawnedHealthPack;
    [SerializeField] private float respawnTime;
    [SerializeField] private float rotationSpeed;

    


    private void Awake()
    {
        
        
        
        healthPackHolder = transform.GetChild(0).gameObject;
        modelHolder = healthPackHolder.transform.GetChild(0).gameObject;

        transform.name = transform.parent.name;
        
        
        foreach (Transform child in modelHolder.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        

        spawnedHealthPack = Instantiate(modelPrefab, modelHolder.transform.position, modelHolder.transform.rotation, modelHolder.transform);
        spawnedHealthPack.transform.parent = modelHolder.transform;
    
    }



    private void Update()
    {
        modelHolder.transform.Rotate(new Vector3(0,45,0) * rotationSpeed * Time.deltaTime);
    }

    public IEnumerator respawnHealthpack()
    {
        foreach (Transform child in modelHolder.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
   
        
        yield return new WaitForSeconds(respawnTime);

     
        spawnedHealthPack = Instantiate(modelPrefab, modelHolder.transform.position, modelHolder.transform.rotation, modelHolder.transform);
        spawnedHealthPack.transform.parent = modelHolder.transform;

    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(spawnedHealthPack);
        
        if (other.GetComponent<PlayerController>().PV.IsMine)
        {
            String tempName = null;
            GameObject SpawnPoints = GameObject.FindGameObjectWithTag("HealthSpawnPoints");
            foreach (Transform spawnPoint in SpawnPoints.transform)
            {
                if (spawnPoint.GetChild(0).name == transform.name)
                {
                    tempName = spawnPoint.GetChild(0).name;
                }
            }
            
            PlayerController player = other.GetComponent<PlayerController>();
            player.HealthPickup();
            StartCoroutine(respawnHealthpack());
            
            other.GetComponent<PlayerController>().PV.RPC("RPC_PickupHealth", RpcTarget.Others, tempName);

        }
    }
}
