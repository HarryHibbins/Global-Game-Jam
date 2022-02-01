using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class pickUpWeapon : MonoBehaviour
{
    private GameObject modelHolder;
    private GameObject model;
    private GameObject spawnedWeapon;

    
    [SerializeField] private float rotationSpeed;
    
    [Header("------------Weapons------------")] [Space(4)]
    [SerializeField]private int weaponNumber;
    public List<WeaponStats> weaponList;



    void Start()
    {
        modelHolder = transform.GetChild(0).gameObject;
        model = modelHolder.transform.GetChild(0).gameObject;

        transform.name = transform.parent.name;
        
        
        foreach (Transform child in model.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        weaponNumber = Random.Range(0, weaponList.Count);
        

        spawnedWeapon = Instantiate(weaponList[weaponNumber].gunModel, model.transform.position, model.transform.rotation, model.transform);
        spawnedWeapon.transform.parent = model.transform;
    }
    

    void Update()
    {
        modelHolder.transform.Rotate(new Vector3(0,45,0) * rotationSpeed * Time.deltaTime);
    }

    public IEnumerator RespawnWeapon()
    {

        foreach (Transform child in model.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
   
        weaponNumber = Random.Range(0, weaponList.Count);
        
        yield return new WaitForSeconds(3f);


        spawnedWeapon = Instantiate(weaponList[weaponNumber].gunModel, model.transform.position, model.transform.rotation, model.transform);
        spawnedWeapon.transform.parent = model.transform;
    }

    public void PickUpWeapon(WeaponSwitcher player)
    {
        foreach (var weapon in player.weaponList)
        {
            if (weapon.weaponName == weaponList[weaponNumber].weaponName)
            {
                weapon.hasWeapon = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>().PV.IsMine)
        {
            String tempName = null;
            GameObject SpawnPoints = GameObject.FindGameObjectWithTag("WeaponSpawnPoints");
            foreach (Transform spawnPoint in SpawnPoints.transform)
            {
                if (spawnPoint.GetChild(0).name == transform.name)
                {
                    tempName = spawnPoint.GetChild(0).name;
                }
            }

            PickUpWeapon(other.GetComponent<WeaponSwitcher>());
            StartCoroutine(RespawnWeapon());
      
            other.GetComponent<PlayerController>().PV.RPC("RPC_PickupWeapon", RpcTarget.Others, tempName);

        }
    }


    
  
    
}
