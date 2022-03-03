using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Projectile")]
public class ProjectileInfo : ItemInfo
{
    float damage, velocity;
    int bouncesLeft;
}
