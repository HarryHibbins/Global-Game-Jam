using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class WeaponStats : MonoBehaviour
{
    [Header("------------Weapon Stats------------")] [Space(4)]
    public int damage;
    public float timeBetweenShooting, range, reloadTime, timeBetweenShots, adsSpeed;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold, allowADS, autoReload, usesShells;
    private int bulletsLeft, bulletsShot;

    [Header("------------Recoil Stats------------")] [Space(4)]
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

    [Header("------------References------------")] [Space(4)]
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public Animator animator;
    public Recoil recoilScript;
    public Transform hipfirePos;
    public Transform adsPos;

    [Header("------------GFX------------")] [Space(4)]
    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public TextMeshProUGUI text;
    private GameObject HUD;


    //Bools
    private bool shooting, readyToShoot, reloading;
    public bool isFiring, isADS;
    
    //Networking
    //private PlayerInstance playerInstance;
    private PhotonView PV; 


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        recoilScript = GameObject.FindGameObjectWithTag("RecoilCam").GetComponent<Recoil>();
        PV = transform.root.GetComponent<PhotonView>();

        
        foreach (Transform child in transform.root)
        {
            if (child.CompareTag("HUD"))
            {
                HUD = child.gameObject;
            }
        }

        //The ammo text must be the first child of HUD
        text = HUD.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        if (!PV.IsMine)
        {
            //Destroy the camera and rigidbody of the other player 
            Destroy(HUD);
           
        }
        

    }

    private void OnEnable()
    {
        reloading = false;
        animator.SetBool("Reloading", false);
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            MyInput();

            //Set Text
            if (usesShells)
            {
                text.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
            }
            else
            {
                text.SetText(bulletsLeft + " / " + magazineSize);
            }

            if (shooting && !reloading && bulletsLeft > 0)
            {
                isFiring = true;
            }
            else
            {
                isFiring = false;
            }

            if (isADS)
            {
                this.gameObject.transform.localPosition = Vector3.Lerp(this.transform.localPosition, adsPos.localPosition, adsSpeed * Time.deltaTime);
                this.gameObject.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, adsPos.localRotation, adsSpeed * Time.deltaTime);
            }
            else
            {
                this.gameObject.transform.localPosition = Vector3.Lerp(this.transform.localPosition, hipfirePos.localPosition, adsSpeed * Time.deltaTime);
                this.gameObject.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, hipfirePos.localRotation, adsSpeed * Time.deltaTime);
            }
        }

    }

    private void MyInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        } 
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (allowADS)
        {
            if (reloading)
            {
                isADS = false;
            }
            else
            {
                isADS = Input.GetKey(KeyCode.Mouse1);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        if (autoReload && bulletsLeft == 0 && !reloading)
        {
            Reload();
        }

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward;

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);
      
            //If the hit object has the IDamagable interface, take damage equal to the weapon damage
            rayHit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);

            GameObject impactGO = Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            Destroy(impactGO, 2f);
        }

        recoilScript.RecoilFire();

        //Graphics
        GameObject flashGO = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        Destroy(flashGO, 0.15f);

        bulletsLeft--;
        bulletsShot--;

        if (!IsInvoking("ResetShot") && !readyToShoot)
        {
            Invoke("ResetShot", timeBetweenShooting);
        }

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        StartCoroutine(ReloadAnim());
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        
        bulletsLeft = magazineSize;
        reloading = false;
    }

    IEnumerator ReloadAnim()
    {
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime - 0.25f);
        animator.SetBool("Reloading", false);
    }
}
