using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponStats : MonoBehaviour
{
    //Gun Stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold, autoReload, usesShells;
    private int bulletsLeft, bulletsShot;

    //Bools
    private bool shooting, readyToShoot, reloading;

    //References
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public Animator animator;

    //Graphics
    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI text;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void OnEnable()
    {
        reloading = false;
        animator.SetBool("Reloading", false);
    }

    private void Update()
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

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(-x,-y,-z);

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);
            if (rayHit.collider.CompareTag("Enemy"))
            {
                //Damage Enemy
            }

            GameObject impactGO = Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            Destroy(impactGO, 2f);
        }

        //ShakeCamera
        StartCoroutine(camShake.Shake(camShakeDuration, camShakeMagnitude));

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
