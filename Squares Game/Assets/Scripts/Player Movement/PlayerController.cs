using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{   
    //Fields
    [SerializeField] float mouseSens, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] GameObject camHolder;
    [SerializeField] Item[] items;



    //In-Hand Items
    int itemIndex;
    int prevItemIndex = -1;

    //Movement
    float verticalLookRotation;
    bool isGrounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    //Client Sync
    Rigidbody rb;
    PhotonView PV;

    //Health
    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    PlayerManager playerManager;
    [SerializeField] Image healthbarImg;
    [SerializeField] GameObject ui;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        if(PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb); //Helps to smooth movement sync, Destroys RB of other players. NOTE: physics object bug in future?
            Destroy(ui);
        }
    }

    void Update()
    {
        //Only Update for your own client's info
        if(!PV.IsMine)
            return;
        
        MouseLook();
        Move();
        Jump();
        UpdateItem();
        CheckInBounds();
        UseItem();
    }


    /************************* MOVEMENT *************************/
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

    void CheckInBounds()
    {
        //Fall out of world
        if(transform.position.y < -10f)
        {
            Die();
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

     /************************* ITEMS *************************/

     void EquipItem(int tempIndex)
     {
         if(tempIndex == prevItemIndex)
            return;

         itemIndex = tempIndex;
         items[itemIndex].itemObj.SetActive(true);

         if(prevItemIndex != -1)
         {
             items[prevItemIndex].itemObj.SetActive(false);
         }

         prevItemIndex = itemIndex;

        //Sync With other players, E7@3:00
        if(PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
     }

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if(!PV.IsMine && targetPlayer == PV.Owner)
		{
			EquipItem((int)changedProps["itemIndex"]);
		}
	}

    void UpdateItem()
    {
         //Gun Item Swapping
        for(int i = 0; i < items.Length; i++)
        {
            if(Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
        
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if(itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }

        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if(itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

    }

    void UseItem()
    {
        //Using items in hand
        if(Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }
    }

    /************************* DAMAGE *************************/

    public void TakeDamage(float damage)
    {
        //Debug.Log("Dealt: " + damage);
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC] //Finds correct target within PUN
    void RPC_TakeDamage(float damage)
    {
        if(!PV.IsMine)
            return;

        //Debug.Log("Took: " + damage);

        currentHealth -= damage;

        healthbarImg.fillAmount = currentHealth / maxHealth; //Modify health bar as a percentage

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        playerManager.Die();
    }
}
