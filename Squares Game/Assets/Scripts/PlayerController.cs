using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float mouseSens, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] GameObject camHolder;


    float verticalLookRotation;
    bool isGrounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;
    PhotonView PV;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb); //Helps to smooth movement sync, Destroys RB of other players. NOTE: physics object bug in future?
        }
    }

    void Update()
    {
        if(!PV.IsMine)
            return;
        
        MouseLook();
        Move();
        Jump();
    }

    void MouseLook()
    {
        //Mouse Look transforms
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSens);
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSens;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        camHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime); //use sprint speed if holding shift, walk if not
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }
    
    public void SetIsGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    void FixedUpdate()
    {
        if(!PV.IsMine)
            return;
        
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

}
