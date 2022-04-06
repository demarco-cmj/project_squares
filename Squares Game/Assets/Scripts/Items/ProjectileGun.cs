using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon;
using TMPro;

public class ProjectileGun : Gun
{
    [SerializeField] Camera cam;
    PhotonView PV;

    public bool reloading;
    public Transform muzzle;
    float timeBetweenShots, lastShot;

    public TMP_Text ammoText;
    int bulletsLeft, bulletsMax;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        timeBetweenShots = 60 / ((GunInfo)itemInfo).fireRate; //Measured in rounds/min
        lastShot = -1f;
        bulletsMax = bulletsLeft = ((GunInfo)itemInfo).magazineSize;
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
        if(!reloading && bulletsLeft > 0)
        {
            bulletsLeft--;
            lastShot = Time.time;
            SetAmmoText();
            if(bulletsLeft == 0)
                {
                    Reload();
                }
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
        //Debug.Log("Reloading now");
        reloading = true;
        Invoke("CompleteReload", ((GunInfo)itemInfo).reloadTime);
        //Play animation here?
    }

    void CompleteReload()
    {
        bulletsLeft = ((GunInfo)itemInfo).magazineSize;
        SetAmmoText();
        reloading = false;
        //Debug.Log("Reloading done");
    }

    void SetAmmoText()
    {
        ammoText.text = bulletsLeft.ToString();
    }

    public override void UpdateHUD()
    {
        if(itemObj.activeInHierarchy)
        {
            SetAmmoText();
        }
    }

    public override void CancelUpdate()
    {
        //Debug.Log("Reloading canceled");
        CancelInvoke("CompleteReload");
        reloading = false;
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
        if(String.Equals(PV.Owner.NickName, owner))
        {
            Debug.Log("Setting isHot to Ture");
            currentBullet.GetComponent<ProjectileInfo>().isHot = true;
        }

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * ((GunInfo)itemInfo).bulletVelocity, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * ((GunInfo)itemInfo).recoil, ForceMode.Impulse); //adds upward force to bullets

        //Destroy bullet after time
        Destroy(currentBullet, 20f);

    }
}
