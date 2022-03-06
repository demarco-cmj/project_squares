using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "FPS/New Projectile")]
public class ProjectileInfo : MonoBehaviour
{
    public float damage;
    public int bouncesLeft;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //deal damage to player, despawn proj
            collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
            //Destroy();
        }
        else
        {
            //reduce bounce count or despawn
        }
    }
}
