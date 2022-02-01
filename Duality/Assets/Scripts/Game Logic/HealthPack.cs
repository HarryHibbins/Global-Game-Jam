using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] GameObject healthpackModel;
    [SerializeField] private GameObject healthpackHolder;
    [SerializeField] GameObject healthpack;
    [SerializeField] private float respawnTime;
    [SerializeField] private float rotationSpeed;

    


    private void Awake()
    {
        
        /*PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
        {
            Destroy(gameObject);
            
   
        }*/
        

        //PhotonNetwork.Destroy(healthpackHolder.transform.GetChild(0).gameObject);
        //Destroy(healthpackHolder.transform.GetChild(0).gameObject);
        //healthpack = PhotonNetwork.Instantiate("HealthPackModel", healthpackHolder.transform.position, healthpackHolder.transform.rotation);
        //healthpack.transform.parent = 
        //healthpack = Instantiate(healthpackModel, healthpackHolder.transform.position, healthpackHolder.transform.rotation);
        healthpackModel = transform.GetChild(0).GetChild(0).gameObject;
        healthpack = healthpackModel.transform.GetChild(0).gameObject;
    }



    private void Update()
    {
        healthpackHolder.transform.Rotate(new Vector3(0,45,0) * rotationSpeed * Time.deltaTime);
    }

    IEnumerator respawnHealthpack()
    {
        yield return new WaitForSeconds(respawnTime);
       healthpack = PhotonNetwork.Instantiate("HealthPackModel", healthpackHolder.transform.position, healthpackHolder.transform.rotation);
      // healthpack.transform.parent = healthpackModel.transform.GetChild(0);
       //healthpack = Instantiate(healthpackModel, healthpackHolder.transform.position, healthpackHolder.transform.rotation);

    }
    private void OnTriggerEnter(Collider other)
    {
        PhotonNetwork.Destroy(healthpack);
        //Destroy(healthpack);
        
        if (other.GetComponent<PlayerController>().PV.IsMine)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.HealthPickup();
            StartCoroutine(respawnHealthpack());
        }
    }
}
