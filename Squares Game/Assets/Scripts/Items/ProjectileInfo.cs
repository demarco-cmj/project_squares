using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;

//[CreateAssetMenu(menuName = "FPS/New Projectile")]
public class ProjectileInfo : MonoBehaviour
{
    public float damage;
    public int bouncesLeft;
    public string owner;
    public GameObject current;
    Rigidbody rb;
    Vector3 location;

    public bool isHot;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")    //everyone is calculating damage, only the shooter activates RPC calls
        {
            float tempHP = -100;
            if(!collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                tempHP = collision.gameObject.GetComponent<PlayerController>().currentHealth -= damage; //TODO: create RPC check in PlayerController.cs TakeDamage() that verifies the player took damage, if not revert this local health update
                if(tempHP <= 0)
                {
                    //collision.gameObject.GetComponent<Collider>().enabled = false;
                    collision.gameObject.layer = LayerMask.NameToLayer("DeadPlayers");
                    collision.gameObject.tag = "DeadPlayer";
                }
            }

            //if this bullet is on shooter's client, deal damage to player
            if(isHot)
            {
                //Debug.LogError("Was hot");
                //Debug.LogError("Player's current HP: " + tempHP);
                collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(tempHP, damage, owner);
            }
            //despawn proj
            Destroy(current);
            //TODO: better performance to build a bullet manager for handling all projectiles?
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
