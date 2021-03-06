using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{   
    /************************* LOCAL VARIABLES *************************/
    
    //Fields
    [SerializeField] float mouseSens, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] GameObject camHolder;
    [SerializeField] Camera cam;
    

    //In-Hand Items
    [SerializeField] Item[] items;
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

    //Health & UI
    const float maxHealth = 100f;
    public float currentHealth = maxHealth;
    PlayerManager playerManager;
    [SerializeField] Image healthbarImg;
    [SerializeField] Image healthbarImgWorld;
    [SerializeField] GameObject ui;
    [SerializeField] PhotonView worldUI;
    bool isPaused = false;

    public GameObject damagePopupPrefab, damagePlane;
    [SerializeField] GameObject[] damageSpawns;

    /************************* MODIFIABLE STATS *************************/
    
    //Weapons
    float damageMod, recoilMod, fireRateMod, bulletVelocityMod, cooldownSpeedMod, reloadTimeMod;
    int magazineSizeMod, bulletsPerTapMod, bulletBounces;

    //Player Movement
    float moveSpeedMod, jumpForceMod;

    //Player Other
    float healthMod;

    /************************* SCRIPT CORE FUNCTION *************************/

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
            Cursor.lockState = CursorLockMode.Locked; //Lock cursor to middle of screen on launch
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb); //Helps to smooth movement sync, Destroys RB of other players. NOTE: physics object bug in future?
            Destroy(ui);
            
            if(worldUI.IsMine) //Hide own username
            {
                gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        //Only Update for your own client's info
        if(!PV.IsMine)
            return;

        if(!isPaused)
        {
            MouseLook();
            Move();
            Jump();
            UpdateItem();
            UseItem(); //where gun stats need to be fed
        }

        Pause();
        CheckInBounds();

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
            rb.AddForce(transform.up * (jumpForce * 50)); //multiply jump force by weight of player
        }
    }

    void CheckInBounds()
    {
        //Fall out of world
        if(transform.position.y < -10f)
        {
            Die(PV.Owner.NickName, true);
        }
    }
    
    public void SetIsGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    void FixedUpdate()      //TODO: Smooth player movement between FixedUpdate
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

        items[itemIndex].CancelUpdate(); //Cancel incomplete reload of previous item

        itemIndex = tempIndex;
        items[itemIndex].itemObj.SetActive(true);
        items[itemIndex].UpdateHUD(); //Update player HUD to current gun
        

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
        if(Input.GetMouseButton(0))  //now sends over at rapid fire
        {
            //Debug.Log("Holding M1");
            items[itemIndex].Use(Aim());
        }
        else if(Input.GetKeyDown("r")) //If player wants to reload
        {
            items[itemIndex].Reload();
        }
    }

    Vector3 Aim()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75); //Just a point far away from the player
        }
        return targetPoint;
    }

    /************************* DAMAGE *************************/

    public void TakeDamage(float targetHP, float damage, string killer) //when player is hit, shooter is led here and send damage to correct target
    {
        // foreach(Player player in PhotonNetwork.PlayerList)
        // {
        //     if(player.PhotonView )
        // }

        //Serve damage to correct player
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, killer); 

        float fill = targetHP / maxHealth;        
        PV.RPC("RPC_UpdateHealthBar", RpcTarget.All, fill);
        PV.RPC("RPC_ShowDamage", RpcTarget.All, damage);
    }

    [PunRPC] //Finds correct target within PUN //RPC == Remote Procedure Call
    void RPC_TakeDamage(float damage, string killer)
    {
        if(!PV.IsMine)
            return;

        //Debug.Log("Took: " + damage);

        currentHealth -= damage;
        

        healthbarImg.fillAmount = currentHealth / maxHealth; //Modify health bar as a percentage

        if(currentHealth <= 0)
        {
            Die(killer, false);
        }
    }

    void Die(string killer, bool suicide)
    {
        gameObject.tag = "DeadPlayer";
        playerManager.Die(killer, PV.Owner.NickName, suicide);
    }

    [PunRPC]
    void RPC_ShowDamage(float damage)
    {
        GameObject popupObj = Instantiate(damagePopupPrefab, damageSpawns[Random.Range(0, damageSpawns.Length)].transform);
        popupObj.GetComponent<DamagePopup>().SetDamage(damage);
        Destroy(popupObj, 1f);
    }

    [PunRPC]
    void RPC_UpdateHealthBar(float fill) //needs parameters passed so remote clients dont use their own health vars
    {
        //Debug.LogError("player took damage: " + fill);
        healthbarImgWorld.fillAmount = fill;
    }

    /************************* MENUS *************************/

    void Pause()
    {
        if(Input.GetKeyDown("escape"))
        {
            isPaused = !isPaused;
        }
    }
}
