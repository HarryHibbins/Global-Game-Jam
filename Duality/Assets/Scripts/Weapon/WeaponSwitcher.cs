using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public List<WeaponStats> weaponList;
    public WeaponScript weaponScript;

    public int currentWeapon = 0;

    private void Update()
    {
        Debug.Log(Input.mouseScrollDelta);

        if (Input.mouseScrollDelta.y > 0 && currentWeapon < weaponList.Count -1)
        {
            currentWeapon++;
            
        }
        if (Input.mouseScrollDelta.y < 0 && currentWeapon > 0)
        {
            currentWeapon--;
            
        }
        weaponScript.stats = weaponList[currentWeapon];
    }
}
