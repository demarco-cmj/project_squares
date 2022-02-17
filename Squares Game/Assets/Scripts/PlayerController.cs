using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   

    //Basic Movement
    public CharacterController controller;
    public float speed = 12f;
    public float jumpHeight = 3f;

    //Gravity
    Vector3 velocity;
    bool isGrounded;
    public float gravity = -9.81f;

    //Ground Check & Grav Reset
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


    // Update is called once per frame
    void Update()
    {   
        //Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0){
            velocity.y = 0f;
        }


        //PLayer Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * x + transform.forward * z);
        if(move.magnitude > 1){
            move /= move.magnitude;
        }
        controller.Move(move * speed * Time.deltaTime);

        //Jump
        if(Input.GetButtonDown("Jump") && isGrounded){
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
