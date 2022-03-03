using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileGun : Gun
{
    [SerializeField] Camera cam;
    PhotonView PV;

    GunInfo gunInfo;
    bool readyToShoot, shooting, reloading;
    public Transform muzzle;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        readyToShoot = true;
    }

    public override void Use()
    {
        //Debug.Log("Using: " + itemInfo.itemName);
        Shoot();
    }

    void Shoot()
    {
        readyToShoot = false;

        //Check if allowed to hold down button and take corresponding input
        // if (GunInfo.IsAutomatic)
        // {
        //     shooting = Input.GetKey(KeyCode.Mouse0);
        // }
        // else 
        // {
        //     shooting = Input.GetKeyDown(KeyCode.Mouse0);
        // }


        //Check center of screen w/ ray for hit
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
            //Debug.Log("Hit: " + hit.collider.gameObject.name);                                                        //OLD METHOD TO SHOOT AND DAMAGE
            //hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            //PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
        else
        {
            targetPoint = ray.GetPoint(75); //Just a point far away from the player
        }

        //Create basic V3 trajectory
        Vector3 direction = targetPoint - muzzle.position;

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(gunInfo.projectile, muzzle.position, Quaternion.identity); //store instantiated bullet in currentBullet
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = direction.normalized;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * gunInfo.bulletVelocity, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * gunInfo.recoil, ForceMode.Impulse);


    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        //Debug.Log(hitPosition);
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
        
    }
}
