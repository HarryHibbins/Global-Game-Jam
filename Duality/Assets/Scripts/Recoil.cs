using UnityEngine;

public class Recoil : MonoBehaviour
{
    //Bools
    private bool isADS = false;

    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private float verticalCompensation = 0;

    //References
    public GameObject weaponHolder;
    private WeaponStats weaponStats;


    void Start()
    {
        weaponStats = weaponHolder.GetComponentInChildren<WeaponStats>();
    }

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, weaponStats.returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, weaponStats.snapiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        if (isADS)
        {
            targetRotation += new Vector3(weaponStats.aimRecoilX, Random.Range(-weaponStats.aimRecoilY, weaponStats.aimRecoilY), Random.Range(-weaponStats.aimRecoilZ, weaponStats.aimRecoilZ));
        }
        else 
        {
            targetRotation += new Vector3(weaponStats.recoilX, Random.Range(-weaponStats.recoilY, weaponStats.recoilY), Random.Range(-weaponStats.recoilZ, weaponStats.recoilZ));
        }
        
    }
}
