using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "FPS/New Projectile")]
public class ProjectileInfo : MonoBehaviour
{
    public float damage;
    public int bouncesLeft;
    public string owner;
    public GameObject current;
    Rigidbody rb;
    Vector3 location;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //deal damage to player, despawn proj
            collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, owner);
            Destroy(current);
        }
        else
        {
            //TODO: reduce bounce count or despawn
        }


    }

    void Start()
    {
        rb = current.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Check velocity for deletion

        if(rb.velocity.sqrMagnitude < 0.5f)
        {
            //Debug.Log("Delete Vel: " + rb.velocity.sqrMagnitude);
            Destroy(current, 1f);
        }
        
    }
}
