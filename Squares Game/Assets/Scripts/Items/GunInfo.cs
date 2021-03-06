using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public float damage, recoil, fireRate, cooldownSpeed, reloadTime, bulletVelocity;
    public int magazineSize, bulletsPerTap;
    public bool isAutomatic;
    public GameObject projectile;

}
