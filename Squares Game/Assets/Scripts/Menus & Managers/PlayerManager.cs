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

    private ExitGames.Client.Photon.Hashtable myCustomProperties = new ExitGames.Client.Photon.Hashtable();

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        killFeedObj = GameObject.FindWithTag("KillFeed");
        scoreboardObj = GameObject.FindWithTag("Scoreboard");

        if (PhotonNetwork.IsMasterClient) {
            //LivesSettup(); //TODO
        }
    }
    
    void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
        }
    }

    void LivesSettup() {
        myCustomProperties["LivesRemaining"] = startingLives;

        foreach (Player player in PhotonNetwork.PlayerList) {
            player.SetCustomProperties(myCustomProperties);
            Debug.Log(player.NickName + " lives set to: " + player.CustomProperties["LivesRemaining"]);
        }
    }

    void CreateController()
    {
        //Debug.Log("Created player controller");

        Transform spawnpoint = SpawnManager.inst.GetSpawnpoint();   //Decide initial spawn point
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {PV.ViewID});
    }

    public void Die(string killer, string body, bool suicide)
    {
        //PV.RPC("RPC_HideBody", RpcTarget.All, controller); //TODO: Not used
        TraceKill(killer, body, suicide);
        StartCoroutine(DeleteBody(killer, body, suicide));
        // PhotonNetwork.Destroy(controller);
        // TraceKill(killer, body, suicide);
        // CreateController();
    }

    void TraceKill(string killer, string body, bool suicide)
    {   
        //Debug.Log("New feed: " + killer +  " killed " + body);
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
