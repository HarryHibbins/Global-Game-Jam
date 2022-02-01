using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponScript : MonoBehaviour
{
    public int bulletsLeft, bulletsShot;
    private float startFOV;

    [Header("------------References------------")] [Space(4)]
    public WeaponStats stats;
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public Animator animator;
    public GameObject recoil;
    public Recoil recoilScript;
    public Transform hipfirePos;
    public Transform adsPos;
    private GameObject weapon;
    public PlayerController playerController;

    [Header("------------GFX------------")] [Space(4)]
    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI nameText;
    public GameObject HUD;
    public Image scopeSprite;
    public Image crosshair;
    public GameObject explosionGFX;


    //Bools
    private bool shooting, readyToShoot, reloading;
    public bool isFiring, isADS, isADSing;
    
    //Networking
    private PhotonView PV;

    private void Awake()
    {
        bulletsLeft = stats.magazineSize;
        readyToShoot = true;
        //recoilScript = GameObject.FindGameObjectWithTag("RecoilCam").GetComponent<Recoil>();
        recoilScript = recoil.GetComponent<Recoil>();
        PV = GetComponent<PhotonView>();
        playerController = GetComponentInParent<PlayerController>();
        startFOV = playerController.ps.FOV;

        foreach (Transform child in transform.root)
        {
            if (child.CompareTag("HUD"))
            {
                HUD = child.gameObject;
            }
        }

        if (PV.IsMine)
        {
            gameObject.layer = LayerMask.NameToLayer("Weapon");
            SetLayerRecursively(this.gameObject, "Weapon");
        }


        //The ammo Text must be the first child of HUD
        ammoText = HUD.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        //The weapon name Text must be the second child of HUD
        nameText = HUD.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        //Scope image muse be the third child of the HUD
        scopeSprite = HUD.transform.GetChild(2).GetComponent<Image>();

        //Scope image muse be the fourth child of the HUD
        crosshair = HUD.transform.GetChild(3).GetComponent<Image>();

        if (!PV.IsMine)
        {
            //Destroy the HUD of the other player 
            Destroy(HUD);
            //this.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            //this.gameObject.layer = LayerMask.NameToLayer("Weapon");
            AssignRecoilCam();
        }
        AssignModel();
    }

    void SetLayerRecursively(GameObject obj, string layer)
    {
        obj.layer = LayerMask.NameToLayer(layer);
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public void AssignRecoilCam()
    {
        //Assign the recoil camera 
        //fpsCam = transform.root.Find("CameraHolder/RecoilCam/ViewCam").GetComponent<Camera>();
    }

    private void OnEnable()
    {
        if (PV.IsMine)
        {
            reloading = false;
            animator.SetBool("Reloading", false);
        }

        /*if (PV.IsMine)
        {
            this.gameObject.layer = LayerMask.NameToLayer("Weapon");
        }
        else
        {
            this.gameObject.layer = LayerMask.NameToLayer("Default");
        }*/
    }

    public void AssignModel()
    {
        foreach (Transform child in this.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        weapon = Instantiate(stats.gunModel, Vector3.zero, Quaternion.identity, this.gameObject.transform);
        if (PV.IsMine)
        {
            weapon.layer = LayerMask.NameToLayer("Weapon");
            SetLayerRecursively(weapon, "Weapon");
        }
        else
        {
            weapon.layer = LayerMask.NameToLayer("Default");
            SetLayerRecursively(weapon, "Default");
        }
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.Euler(0, 180, 0);
        foreach (Transform child in weapon.transform)
        {
            if (child.tag == "FiringPoint")
            {
                attackPoint = child;
            }
        }
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if (!playerController.isPaused)
            {
                MyInput();
            }
            

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

            ADSLogic();
        }
    }

    private void ADSLogic()
    {
        // Moving Weapon when ADS to look down sights
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

        // Checks for when player is actually looking down sights. This prevents tapping ads for an accurate shot with a sniper for example.
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

        PlayerMovement pm = GetComponentInParent<PlayerMovement>();
        // Manages FOV for ADS/Scoping & Disabling UI elements for scope
        if (isADSing && !stats.usesScope && !pm.sliding && !pm.wallRunning && !pm.sprinting)
        {
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, 45f, stats.adsSpeed * Time.deltaTime);
        }
        else if (!stats.usesScope && !pm.sliding && !pm.wallRunning && !pm.sprinting)
        {
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, playerController.ps.FOV, stats.adsSpeed * Time.deltaTime);
        }
        else 
        {
            fpsCam.fieldOfView = playerController.ps.FOV;
        }

        if (stats.usesScope && isADSing)
        {
            
            scopeSprite.enabled = true;
            weapon.gameObject.SetActive(false);
            crosshair.enabled = false;
            fpsCam.fieldOfView = 12f;
            playerController.currentSensitivity = playerController.ps.scopedSensitivity;
            GetComponentInParent<PlayerMovement>().sensX = playerController.ps.scopedSensitivity;
            GetComponentInParent<PlayerMovement>().sensY = playerController.ps.scopedSensitivity;
            Debug.Log("set sensitivity to " + playerController.ps.scopedSensitivity);
        }
        else
        {
            scopeSprite.enabled = false;
            weapon.gameObject.SetActive(true);
            crosshair.enabled = true;
            playerController.currentSensitivity = playerController.ps.unscopedSensitivity;
            GetComponentInParent<PlayerMovement>().sensX = playerController.ps.unscopedSensitivity;
            GetComponentInParent<PlayerMovement>().sensY = playerController.ps.unscopedSensitivity;
            Debug.Log("set sensitivity to " + playerController.ps.scopedSensitivity);
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

        

        if (stats.isHitscan)
        {
            //Calculate Direction with Spread
            Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

            //RayCast
            if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, stats.range, whatIsEnemy))
            {
                //Debug.Log(rayHit.collider.name);

                //If the hit object has the IDamagable interface, take damage equal to the weapon damage
                rayHit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(stats.damage);
                PV.RPC("RPC_Shoot", RpcTarget.All, rayHit.point, rayHit.normal);


            }
        }
        else
        {
            Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(75);
            }

            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
            Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

            GameObject currentBullet = Instantiate(stats.projectileAmmo, attackPoint.position, Quaternion.identity);
            currentBullet.GetComponent<ProjectileExplosion>().pv = PV;
            currentBullet.transform.forward = directionWithSpread.normalized;
            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * stats.shootForce, ForceMode.Impulse);
            currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * stats.upwardForce, ForceMode.Impulse);
            
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
        //Debug.Log(hitPos);
        
        //returns an array of all colliders in a 0.3 radius
        Collider[] colliders = Physics.OverlapSphere(hitPos, 0.3f);
        if (colliders.Length != 0)
        {
            //Graphics
            GameObject impactGO = Instantiate(bulletHoleGraphic, hitPos + hitNormal * 0.001f,
                Quaternion.LookRotation(hitNormal));
            Destroy(impactGO, 10f);
            //Debug.Log("bullet hole");
            impactGO.transform.SetParent(colliders[0].transform);
        }
        
    }

    [PunRPC]
    void RPC_Explode(Vector3 pos)
    {
        //Instatiate explosion
        Instantiate(explosionGFX, pos, Quaternion.identity);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        if (PV.IsMine)
        {
            animator.applyRootMotion = false;
            reloading = true;
            StartCoroutine(ReloadAnim());
            Invoke("ReloadFinished", stats.reloadTime);   
        }
    }

    private void ReloadFinished()
    {
        animator.applyRootMotion = true;
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
