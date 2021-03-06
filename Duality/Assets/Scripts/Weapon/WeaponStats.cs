using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "My Assets/Weapons")]
public class WeaponStats : ScriptableObject
{
    public string weaponName; [Space(4)]

    [Header("------------Weapon Stats------------")] [Space(4)]
    public int damage;
    public float timeBetweenShooting, spread, adsSpread, range, reloadTime, timeBetweenShots, adsSpeed;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold, allowADS, autoReload, usesShells, usesScope, isHitscan, hasWeapon;

    [Tooltip("X = Up, Y = Left/Right")]
    [Header("------------Recoil Stats------------")]
    [Space(4)]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    [Space(2)]
    public float aimRecoilX;
    public float aimRecoilY;
    public float aimRecoilZ;
    [Space(2)]
    public float snapiness;
    public float returnSpeed;

    [Header("------------Weapon Model------------")]
    public GameObject gunModel;
    public Sprite icon;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Header("------------Projectile Ammo------------")]
    public GameObject projectileAmmo;
    public float shootForce;
    public float upwardForce;
}