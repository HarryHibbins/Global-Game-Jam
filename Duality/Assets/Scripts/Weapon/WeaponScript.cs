using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class WeaponScript : MonoBehaviour
{
    private int bulletsLeft, bulletsShot;

    [Header("------------References------------")] [Space(4)]
    public WeaponStats stats;
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
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI nameText;
    private GameObject HUD;


    //Bools
    private bool shooting, readyToShoot, reloading;
    public bool isFiring, isADS, isADSing;
    
    //Networking
    private PhotonView PV;

    private void Awake()
    {
        bulletsLeft = stats.magazineSize;
        readyToShoot = true;
        recoilScript = GameObject.FindGameObjectWithTag("RecoilCam").GetComponent<Recoil>();
        PV = GetComponent<PhotonView>();

        
        foreach (Transform child in transform.root)
        {
            if (child.CompareTag("HUD"))
            {
                HUD = child.gameObject;
            }
        }

        //The ammo Text must be the first child of HUD
        ammoText = HUD.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        //The weapon name Text must be the second child of HUD
        nameText = HUD.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        if (!PV.IsMine)
        {
            //Destroy the HUD of the other player 
            Destroy(HUD);
        }
        else
        {
            AssignRecoilCam();
        }
      

    }

    public void AssignRecoilCam()
    {
        //Assign the recoil camera 
        fpsCam = transform.root.Find("CameraHolder/RecoilCam/ViewCam").GetComponent<Camera>();
    }

    private void OnEnable()
    {
        if (PV.IsMine)
        {
            reloading = false;
            animator.SetBool("Reloading", false);
        }
     
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            MyInput();

            //Set Text
            if (stats.usesShells)
            {
                ammoText.SetText(bulletsLeft / stats.bulletsPerTap + " / " + stats.magazineSize / stats.bulletsPerTap);
            }
            else
            {
                ammoText.SetText(bulletsLeft + " / " + stats.magazineSize);
            }
            nameText.SetText(stats.weaponName);

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
                this.gameObject.transform.localPosition = Vector3.Lerp(this.transform.localPosition, adsPos.localPosition, stats.adsSpeed * Time.deltaTime);
                this.gameObject.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, adsPos.localRotation, stats.adsSpeed * Time.deltaTime);
            }
            else
            {
                this.gameObject.transform.localPosition = Vector3.Lerp(this.transform.localPosition, hipfirePos.localPosition, stats.adsSpeed * Time.deltaTime);
                this.gameObject.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, hipfirePos.localRotation, stats.adsSpeed * Time.deltaTime);
            }

            float xPosThis = Mathf.Round(this.gameObject.transform.localPosition.x * 10f) * 0.1f;
            float xPosThat = Mathf.Round(adsPos.transform.localPosition.x * 10f) * 0.1f; 

            if (Mathf.Approximately(xPosThis, xPosThat))
            {
                isADSing = true;
            }
            else
            {
                isADSing = false;
            }
        }

    }

    private void MyInput()
    {
        if (stats.allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        } 
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (stats.allowADS)
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

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < stats.magazineSize && !reloading)
        {
            Reload();
        }

        if (stats.autoReload && bulletsLeft == 0 && !reloading)
        {
            Reload();
        }

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = stats.bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        float x;
        float y;

        //Spread
        if (isADSing)
        {
            if (stats.adsSpread != 0)
            {
                x = Random.Range(-stats.adsSpread, stats.adsSpread);
                y = Random.Range(-stats.adsSpread, stats.adsSpread);
            }
            else
            {
                x = 0;
                y = 0;
            }
        }
        else
        {
            if (stats.spread != 0)
            {
                x = Random.Range(-stats.spread, stats.spread);
                y = Random.Range(-stats.spread, stats.spread);
            }
            else
            {
                x = 0;
                y = 0;
            }
        }

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, stats.range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);
      
            //If the hit object has the IDamagable interface, take damage equal to the weapon damage
            rayHit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(stats.damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, rayHit.point, rayHit.normal);

            
        }

        recoilScript.RecoilFire();

        

        bulletsLeft--;
        bulletsShot--;

        if (!IsInvoking("ResetShot") && !readyToShoot)
        {
            Invoke("ResetShot", stats.timeBetweenShooting);
        }

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", stats.timeBetweenShots);
        }
    }

    //Muzzle flash and bullet holes sent to all clients
    [PunRPC]
    void RPC_Shoot(Vector3 hitPos, Vector3 hitNormal)
    {
        GameObject flashGO = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        Destroy(flashGO, 0.15f);
        Debug.Log(hitPos);
        
        //returns an array of all colliders in a 0.3 radius
        Collider[] colliders = Physics.OverlapSphere(hitPos, 0.3f);
        if (colliders.Length != 0)
        {
            //Graphics
            GameObject impactGO = Instantiate(bulletHoleGraphic, hitPos + hitNormal * 0.001f,
                Quaternion.LookRotation(hitNormal));
            Destroy(impactGO, 10f);
            Debug.Log("bullet hole");
            impactGO.transform.SetParent(colliders[0].transform);
        }
        
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        if (PV.IsMine)
        {
            reloading = true;
            StartCoroutine(ReloadAnim());
            Invoke("ReloadFinished", stats.reloadTime);   
        }
    }

    private void ReloadFinished()
    {
        
        bulletsLeft = stats.magazineSize;
        reloading = false;
    }

    IEnumerator ReloadAnim()
    {
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(stats.reloadTime - 0.25f);
        animator.SetBool("Reloading", false);
    }
}
