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
    //PhotonView PV;

    // void Awake()
    // {
    //     PV = GetComponent<PhotonView>();
    // }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Hit player");
            //deal damage to player, despawn proj
            if(isHot)
            {
                //Debug.LogError("Was hot");
                collision.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, owner);
            }
            Destroy(current);
            //PV.RPC("RPC_DeleteBullet", RpcTarget.All);                                      //TODO: better performance to build a bullet manager for handling all projectiles?
            //PhotonNetwork.Destroy(current);
        }
        else
        {
            //TODO: reduce bounce count or despawn
        }


    }

    // [PunRPC]
    // void RPC_DeleteBullet()
    // {
    //         Destroy(current);
    // }

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
