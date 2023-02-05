using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using TMPro;
using UnityEngine.UI;

public class ProjectileGun : Gun
{
    [SerializeField] Camera cam;
    PhotonView PV;

    public bool reloading;
    public Transform muzzle;
    float timeBetweenShots, lastShot;

    public TMP_Text ammoText;
    int bulletsLeft, bulletsMax;

    //Reload Icon
    public Image reloadIcon;
    float currentFillValue;
    Vector3 rotationEuler = Vector3.zero;
    
    //Animation
    public Animator weaponAnimation;
    RecoilCam recoilCam;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        weaponAnimation = GetComponentInParent<Animator>();
        recoilCam = GetComponentInParent<RecoilCam>();
        timeBetweenShots = 60 / ((GunInfo)itemInfo).fireRate; //Measured in rounds/min
        lastShot = -1f;
        bulletsMax = bulletsLeft = ((GunInfo)itemInfo).magazineSize;
        SetAmmoText();        
    }

    void Update () {
        if(reloading) {
            StartReloadIconAnimation();
        }
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
                Shoot(tp, true);
                
            }
            else if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Isnt automatic");
                Shoot(tp, false);
            }
        }
    }

    void Shoot(Vector3 tp, bool isAuto)
    {
        if(!reloading && bulletsLeft > 0) //shoot
        {
            if(isAuto) {
                weaponAnimation.SetBool("isShooting", true);
                recoilCam.CamRecoil();
                // weaponAnimation.SetBool("isWalking", false);
                // weaponAnimation.SetBool("isIdle", false);
            } else {
                weaponAnimation.Play("Shoot_1");
                recoilCam.CamRecoil();
            }
 

            bulletsLeft--;
            lastShot = Time.time;
            SetAmmoText();
            if(bulletsLeft == 0)
                {
                    weaponAnimation.SetBool("isShooting", false);
                    Reload();
                }
            PV.RPC("RPC_Shoot", RpcTarget.All, tp, PV.Owner.NickName);
        }
        else if (reloading && bulletsLeft > 0) { //cancel reload to shoot
            CancelUpdate();
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
        EndReloadIconAnimation();
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
        EndReloadIconAnimation();
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
        if(PV.IsMine) //only the player shooting will have an active bullet
        {
            currentBullet.GetComponent<ProjectileInfo>().isHot = true;
        }

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * ((GunInfo)itemInfo).bulletVelocity, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * ((GunInfo)itemInfo).recoil, ForceMode.Impulse); //adds upward force to bullets

        //Destroy bullet after time
        Destroy(currentBullet, 20f);

    }

    /************* RELOAD ICON ***************/

    void StartReloadIconAnimation() {
        reloadIcon.gameObject.SetActive(true);
        // if (currentFillValue < 100) {
		// 	currentFillValue +=  40f * Time.deltaTime; //TODO: GET GUN reload speed here
		// } else {
		// 	LoadingBar.gameObject.SetActive(false);
		// }
 
		// LoadingBar.fillAmount = currentFillValue / 100;

        rotationEuler -= Vector3.forward*180*Time.deltaTime; //increment 30 degrees every second
        reloadIcon.gameObject.transform.rotation = Quaternion.Euler(rotationEuler);
    }

    void EndReloadIconAnimation() {
        rotationEuler = Vector3.zero;
        if (reloadIcon.gameObject != null) {
            reloadIcon.gameObject.SetActive(false);
        }
    }

    /*************** RECOIL **************/

    void CalculateRecoil() {
        //currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * ((GunInfo)itemInfo).recoil, ForceMode.Impulse); //adds upward force to bullets
        float changeX, changeY;

        changeX = Random.Range(-1.0f, 1.0f) / 2 * ((GunInfo)itemInfo).horizonalRecoil;
        changeY = Random.Range(-1.0f, 1.0f) / 2 * ((GunInfo)itemInfo).verticalRecoil; //TODO add respect for the amount of time pressed
    }
}
