using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class KillfeedItem : MonoBehaviour
{
    public TMP_Text killerText;
    public TMP_Text victimText;
    public Image gunIcon;

    public Player player;

    public WeaponStats AR;
    public WeaponStats Burst;
    public WeaponStats SMG;
    public WeaponStats Shotgun;
    public WeaponStats DMR;
    public WeaponStats Sniper;
    public WeaponStats RocketLauncher;
    public WeaponStats Pistol;


    public void GetSprite(string name)
    {
        if (name == AR.weaponName)
        {
            gunIcon.sprite = AR.icon;
        }
        else if (name == Burst.weaponName)
        {
            gunIcon.sprite = Burst.icon;
        }
        else if (name == SMG.weaponName)
        {
            gunIcon.sprite = SMG.icon;
        }
        else if (name == Shotgun.weaponName)
        {
            gunIcon.sprite = Shotgun.icon;
        }
        else if (name == DMR.weaponName)
        {
            gunIcon.sprite = DMR.icon;
        }
        else if (name == Sniper.weaponName)
        {
            gunIcon.sprite = Sniper.icon;
        }
        else if (name == RocketLauncher.weaponName)
        {
            gunIcon.sprite = RocketLauncher.icon;
        }
        else if (name == Pistol.weaponName)
        {
            gunIcon.sprite = Pistol.icon;
        }
    }
}
