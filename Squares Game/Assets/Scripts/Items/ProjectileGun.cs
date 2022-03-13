using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using TMPro;

public class ProjectileGun : Gun
{
    [SerializeField] Camera cam;
    PhotonView PV;

    public GameObject tempProjectile;
    bool reloading;
    public Transform muzzle;
    float timeBetweenShots, lastShot;

    public TMP_Text ammoText;
    int bulletsLeft;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        timeBetweenShots = 60 / ((GunInfo)itemInfo).fireRate; //Measured in rounds/min
        lastShot = -1f;
        bulletsLeft = ((GunInfo)itemInfo).magazineSize;
        SetAmmoText();

        
    }

    public override void Use(Vector3 tp)
    {
        if(Time.time >= lastShot + timeBetweenShots)
        {
            //Debug.Log("Using: " + itemInfo.itemName);

            //shoot according to fire mode
            if (((GunInfo)itemInfo).isAutomatic)
            {
                //Debug.Log("Is automatic");                 
                Shoot(tp);
                
            }
            else if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Isnt automatic");
                Shoot(tp);
            }
        }
    }

    void Shoot(Vector3 tp)
    {
        if(bulletsLeft > ((GunInfo)itemInfo).magazineSize)
        {
            bulletsLeft--;
            lastShot = Time.time;
            SetAmmoText();
            PV.RPC("RPC_Shoot", RpcTarget.All, tp, PV.Owner.NickName);
        }
        else
        {
            Reload();
        }

    }

    public override void Reload()
    {
        if(reloading)
            return;
        
        float startTime = Time.time;
        reloading = true;
        //Play animation here?
        // while(reloading)  //Find better wait method thats accurate? Maybe Invoke(ReloadDone(), reloadTime);?
        // {
        //     if(Time.time - startTime >= ((GunInfo)itemInfo).reloadTime)
        //     {
        //         reloading = false;
        //     }
        // }
        bulletsLeft = ((GunInfo)itemInfo).magazineSize;
        SetAmmoText();

    }

    void SetAmmoText()
    {
        ammoText.text = bulletsLeft.ToString();
    }

    [PunRPC]
    void RPC_Shoot(Vector3 targetPoint, string owner)
    {
        //Debug.Log("SHOOTING PROJECTILE");

        //Create basic V3 trajectory
        Vector3 direction = targetPoint - muzzle.position;

        //Instantiate bullet/projectile then Rotate bullet to shoot direction
        GameObject currentBullet = Instantiate(((GunInfo)itemInfo).projectile, muzzle.position, Quaternion.identity); //store instantiated bullet in currentBullet
        currentBullet.transform.forward = direction.normalized;

        //Set Bullet's local info
        currentBullet.GetComponent<ProjectileInfo>().damage = ((GunInfo)itemInfo).damage;
        currentBullet.GetComponent<ProjectileInfo>().owner = owner;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * ((GunInfo)itemInfo).bulletVelocity, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * ((GunInfo)itemInfo).recoil, ForceMode.Impulse); //adds upward force to bullets

        //Destroy bullet after time
        Destroy(currentBullet, 20f);

    }
}
