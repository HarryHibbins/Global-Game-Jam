using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Random = UnityEngine.Random;


public class WeaponSwitcher : MonoBehaviour
{
    public List<WeaponStats> weaponList;
    public WeaponScript weaponScript;
    public GameLogic gameLogic;

    public int currentWeapon = 0;

    public PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        gameLogic = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogic>();

        if (PV.IsMine)
        {
            foreach (var weapon in weaponList)
            {
                if (weapon.hasWeapon)
                {
                    weapon.hasWeapon = false;
                    
                }
            }


            StartCoroutine(assignModel());

        }
       
    }

    IEnumerator assignModel()
    {
        //spawn with pistol and random other gun between AS, Shotgun, and SMG
        weaponList[0].hasWeapon = true;
        int randomNum = Random.Range(1, 4);
        weaponList[randomNum].hasWeapon = true;
        
        yield return new WaitForSeconds(.1f);

        if (gameLogic.game_mode == 0)
        {
            foreach (WeaponStats ws in weaponList)
            {
                ws.hasWeapon = true;
            }
        }
        currentWeapon = randomNum;
        UpdateGun();
            
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if (Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(KeyCode.Alpha2) && currentWeapon < weaponList.Count - 1)
            {
                nextWeapon();

            }
            if (Input.mouseScrollDelta.y < 0  || Input.GetKeyDown(KeyCode.Alpha1) && currentWeapon >= 0)
            {
                previousWeapon();
            }
        }

    }

    private void nextWeapon()
    {
        if (weaponList[currentWeapon+1].hasWeapon)
        {
            currentWeapon++;
            UpdateGun();
            return;
        }
 
        while (!weaponList[currentWeapon+1].hasWeapon && currentWeapon < weaponList.Count-1)
        {
            currentWeapon++;

            if (weaponList[currentWeapon+1].hasWeapon)
            {
                currentWeapon++;
                UpdateGun();
                return;

            }
            
            if (!weaponList[weaponList.Count-1].hasWeapon && currentWeapon == weaponList.Count-2)
            {
                Debug.Log("Reset");
                currentWeapon = 0;
                UpdateGun();
                return;

            }

        }

     

       
 
    }
    private void previousWeapon()
    {
        if (currentWeapon == 0)
        {
            for (int i = weaponList.Count-1; i > 0; i--)
            {
                
                if (weaponList[i].hasWeapon)
                {
                    currentWeapon = i;
                    UpdateGun();
                    return;
                }
            }
        }
        if (weaponList[currentWeapon-1].hasWeapon)
        {
            currentWeapon--;
            UpdateGun();
            return;
        }
        while (!weaponList[currentWeapon-1].hasWeapon  && currentWeapon > 0)
        {
            currentWeapon--;
            
            if (weaponList[currentWeapon-1].hasWeapon)
            {
                currentWeapon--;
                UpdateGun();
                return;
            }
        }
    }
    void UpdateGun()
    {
        weaponScript.stats = weaponList[currentWeapon];
        weaponScript.bulletsLeft = weaponScript.stats.magazineSize;
        weaponScript.bulletsShot = 0;
        weaponScript.AssignModel();
    }
}
