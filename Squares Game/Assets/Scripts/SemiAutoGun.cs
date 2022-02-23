using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiAutoGun : Gun
{
    [SerializeField] Camera cam;

    public override void Use()
    {
        //Debug.Log("Using: " + itemInfo.itemName);
        //throw new System.NotImplementedException();
        Shoot();
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.Log("Hit: " + hit.collider.gameObject.name);
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
        }
    }
}
