using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RayGun : Gun
{
    [SerializeField] Camera cam;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use(Vector3 tp)
    {
        //Debug.Log("Using: " + itemInfo.itemName);
        Shoot();
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.Log("Hit: " + hit.collider.gameObject.name);
            float tempHP = hit.collider.gameObject.GetComponent<PlayerController>().currentHealth; //here
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(tempHP, ((GunInfo)itemInfo).damage, PV.Owner.NickName);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    public override void Reload()
    {

    }

    public override void UpdateHUD()
    {

    }

    public override void CancelUpdate()
    {
        CancelInvoke();
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
