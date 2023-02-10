//Class is instanciated as a game object clone on game scene load

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;
    private GameObject killFeedObj;
    private GameObject scoreboardObj;

    private int startingLives = 2;
    private Dictionary < int, int > playerLives = new Dictionary<int, int>();

    private ExitGames.Client.Photon.Hashtable myCustomProps = new ExitGames.Client.Photon.Hashtable();

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        killFeedObj = GameObject.FindWithTag("KillFeed");
        scoreboardObj = GameObject.FindWithTag("Scoreboard");
    }
    
    void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
            LivesSettup();
        }
    }

    void LivesSettup() {
        myCustomProps["LivesRemaining"] = startingLives;
        PhotonNetwork.LocalPlayer.SetCustomProperties(myCustomProps);

        // foreach (Player player in PhotonNetwork.PlayerList) {
        //     player.SetCustomProperties(myCustomProperties);
        //     Debug.Log(player.NickName + " lives set to: " + player.CustomProperties["LivesRemaining"]);
        // }
    }

    void CreateController()
    {
        //Debug.Log("Created player controller");

        Transform spawnpoint = SpawnManager.inst.GetSpawnpoint();   //Decide initial spawn point
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {PV.ViewID});
    }

    public void Die(string killer, string body, bool suicide)
    {
        //Send to kill feed
        TraceKill(killer, body, suicide);

        //update custom properties
        myCustomProps["LivesRemaining"] = (int)PhotonNetwork.LocalPlayer.CustomProperties["LivesRemaining"] - 1;
        PhotonNetwork.LocalPlayer.CustomProperties = myCustomProps;

        //Check lives remaining for if dead
        if((int)PhotonNetwork.LocalPlayer.CustomProperties["LivesRemaining"] <= 0 ) {
            Debug.Log("YOU ARE DEAD");
            
        }

        //PV.RPC("RPC_HideBody", RpcTarget.All, controller); //TODO: Not used
        StartCoroutine(DeleteBody(killer, body, suicide));
    }

    void TraceKill(string killer, string body, bool suicide)
    {   
        Debug.Log("New feed: " + killer +  " killed " + body);
        PV.RPC("RPC_TraceKill", RpcTarget.All, killer, body, suicide);
    }

    IEnumerator DeleteBody(string killer, string body, bool suicide) //TODO: Not used
    {
        if(!suicide)
        {
            yield return new WaitForSeconds(3);
        }
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

    // [PunRPC]
    // void RPC_HideBody(GameObject body) //TODO: Not used
    // {
    //     Renderer graphics = body.GetComponent<Renderer>();
    //     graphics.enabled = false;
    //     Debug.Log("Hid body");
    // }

    [PunRPC]
    void RPC_TraceKill(string killer, string body, bool suicide)
    {
        string killFeedText = killer +  " killed " + body;
        killFeedObj.GetComponent<KillFeed>().CreateItem(killFeedText);
        if(!suicide)
        {
            scoreboardObj.GetComponent<Scoreboard>().IncrementKills(killer);
        }
        scoreboardObj.GetComponent<Scoreboard>().IncrementDeaths(body);
    }
}
