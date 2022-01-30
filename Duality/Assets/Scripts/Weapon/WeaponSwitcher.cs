using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponSwitcher : MonoBehaviour
{
    public List<WeaponStats> weaponList;
    public WeaponScript weaponScript;

    public int currentWeapon = 0;

    public PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if (Input.mouseScrollDelta.y > 0 && currentWeapon < weaponList.Count - 1)
            {
                currentWeapon++;
                UpdateGun();
            }
            if (Input.mouseScrollDelta.y < 0 && currentWeapon > 0)
            {
                currentWeapon--;
                UpdateGun();
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
