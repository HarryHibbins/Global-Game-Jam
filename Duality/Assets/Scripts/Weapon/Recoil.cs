using Photon.Pun;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    //Bools
    private bool isADS = false;

    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //References
    public GameObject weaponHolder;
    private WeaponScript weaponScript;

    //Networking
    private PhotonView PV; 

    void Start()
    {
        weaponScript = weaponHolder.GetComponentInChildren<WeaponScript>();
        PV = transform.root.GetComponent<PhotonView>();
        

    }

    void Update()
    {
        if (PV.IsMine)
        {
            isADS = weaponScript.isADSing;

            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, weaponScript.stats.returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, weaponScript.stats.snapiness * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Euler(currentRotation);
        }
   
    }

    public void RecoilFire()
    {
        if (PV.IsMine)
        {
            if (isADS)
            {
                targetRotation += new Vector3(weaponScript.stats.aimRecoilX,
                    Random.Range(-weaponScript.stats.aimRecoilY, weaponScript.stats.aimRecoilY),
                    Random.Range(-weaponScript.stats.aimRecoilZ, weaponScript.stats.aimRecoilZ));
            }
            else
            {
                targetRotation += new Vector3(weaponScript.stats.recoilX,
                    Random.Range(-weaponScript.stats.recoilY, weaponScript.stats.recoilY),
                    Random.Range(-weaponScript.stats.recoilZ, weaponScript.stats.recoilZ));
            }
        }

    }
}
