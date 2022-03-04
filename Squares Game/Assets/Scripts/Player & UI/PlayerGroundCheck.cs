using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;
    
    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    
    //Triggers
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == playerController.gameObject)
            return;

        playerController.SetIsGrounded(true);
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == playerController.gameObject)
            return;

        playerController.SetIsGrounded(false);
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject == playerController.gameObject)
            return;

        playerController.SetIsGrounded(true);
    }

    //Colissions
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject == playerController.gameObject)
            return;

        playerController.SetIsGrounded(true);
    }

    void OnCollisionExit(Collision other)
    {
        if(other.gameObject == playerController.gameObject)
            return;

        playerController.SetIsGrounded(false);
    }

        void OnCollisionStay(Collision other)
    {
        if(other.gameObject == playerController.gameObject)
            return;

        playerController.SetIsGrounded(true);
    }


}
