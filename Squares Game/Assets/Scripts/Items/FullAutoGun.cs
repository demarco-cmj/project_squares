using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class FullAutoGun : MonoBehaviour
{
    // [SerializeField] Camera cam;
    // PhotonView PV;

    // public GameObject bullet;
    // public float shootForce, upwardForce;

    // public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    // public int magazineSize, bulletsPerTap;
    // public bool allowButtonHold;

    // //public Rigidbody playerRb;
    // //public float recoilForce;

    // bool shooting, readyToShoot, reloading;

    // public Camera fpsCam;
    // public Transform attackPoint;


    // void Awake()
    // {
    //     PV = GetComponent<PhotonView>();
    // }

    // public override void Use()
    // {
    //     //Debug.Log("Using: " + itemInfo.itemName);
    //     Shoot();
    // }

    // void userInput()
    // {
    //      //Check if allowed to hold down button and take corresponding input
    //     if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
    //     else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        
    //     //Reloading 
    //     if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
    //     //Reload automatically when trying to shoot without ammo
    //     if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();
    // }

    // void Shoot()
    // {
       


    //     Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
    //     ray.origin = cam.transform.position;
    //     if(Physics.Raycast(ray, out RaycastHit hit))
    //     {
    //         //Debug.Log("Hit: " + hit.collider.gameObject.name);
    //         hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
    //         PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
    //     }
    // }

    // [PunRPC]
    // void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    // {
    //     //Debug.Log(hitPosition);
    //     Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
    //     if(colliders.Length != 0)
    //     {
    //         GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
    //         Destroy(bulletImpactObj, 10f);
    //         bulletImpactObj.transform.SetParent(colliders[0].transform);
    //     }
        
    // }

}
