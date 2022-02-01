using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] GameObject healthpackModel;
   [SerializeField] private GameObject healthpackHolder;
    private GameObject healthpack;
    [SerializeField] private float respawnTime;
    [SerializeField] private float rotationSpeed;


    private void Awake()
    {
        //healthpackHolder.transform.GetChild(0);
        Destroy(healthpackHolder.transform.GetChild(0).gameObject);
        healthpack = Instantiate(healthpackModel, healthpackHolder.transform.position, healthpackHolder.transform.rotation, healthpackHolder.transform);
      
    }

    private void Update()
    {
        healthpackHolder.transform.Rotate(new Vector3(0,45,0) * rotationSpeed * Time.deltaTime);
    }

    IEnumerator respawnHealthpack()
    {
        yield return new WaitForSeconds(respawnTime);
        healthpack = Instantiate(healthpackModel, healthpackHolder.transform.position, healthpackHolder.transform.rotation, healthpackHolder.transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(healthpack);
        
        if (other.GetComponent<PlayerController>().PV.IsMine)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.HealthPickup();
            StartCoroutine(respawnHealthpack());
        }
    }
}
