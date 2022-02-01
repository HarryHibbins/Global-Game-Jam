using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class pickUpWeapon : MonoBehaviour
{
    private GameObject modelHolder;
    private GameObject model;
    private GameObject spawnedWeapon;
    private PhotonView PV;

    
    [SerializeField] private float rotationSpeed;
    
    [Header("------------Weapons------------")] [Space(4)]
    [SerializeField]private int weaponNumber;
    public List<WeaponStats> weaponList;



    void Start()
    {
        modelHolder = transform.GetChild(0).gameObject;
        model = modelHolder.transform.GetChild(0).gameObject;
        
        
        RespawnWeapon();
    }

    void Update()
    {
        modelHolder.transform.Rotate(new Vector3(0,45,0) * rotationSpeed * Time.deltaTime);
    }

    private void RespawnWeapon()
    {

        foreach (Transform child in model.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        weaponNumber = Random.Range(0, weaponList.Count);
        spawnedWeapon = Instantiate(weaponList[weaponNumber].gunModel, model.transform.position, model.transform.rotation, model.transform);

    }

    private void PickUpWeapon(WeaponSwitcher player)
    {
        foreach (var weapon in player.weaponList)
        {
            if (weapon.weaponName == weaponList[weaponNumber].weaponName)
            {
                weapon.hasWeapon = true;
                Debug.Log("Picked up: " + weapon.weaponName + weapon.hasWeapon);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>().PV.IsMine)
        {
            PickUpWeapon(other.GetComponent<WeaponSwitcher>());
            RespawnWeapon();
        }
    }
}
