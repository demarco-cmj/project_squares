using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;
    private GameObject universalUI;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        universalUI = GameObject.FindWithTag("KillFeed");
    }
    
    void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        //Debug.Log("Created player controller");

        Transform spawnpoint = SpawnManager.inst.GetSpawnpoint();   //Decide initial spawn point
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {PV.ViewID});
    }

    public void Die(string killer, string body)
    {
        PhotonNetwork.Destroy(controller);
        TraceKill(killer, body);
        CreateController();
    }

    void TraceKill(string killer, string body)
    {   
        Debug.Log("New feed: " + killer +  " killed " + body);
        string killFeed = killer +  " killed " + body;
        PV.RPC("RPC_TraceKill", RpcTarget.All, killFeed);
    }

    [PunRPC]
    void RPC_TraceKill(string killFeedText)
    {
        universalUI.GetComponent<KillFeed>().CreateItem(killFeedText);
        //killFeedUI.CreateItem(killFeedText);
    }
}
