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
    private int numberPlayers;

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
        //Send to kill feed
        TraceKill(killer, body, suicide);

        //update custom properties
        PlayerPropertiesManager.ChangeTargetPlayerProperty(PV.ViewID, PlayerPropertiesManager.livesRemaining, -1, false, true);

        //Check lives remaining for if dead
        if(PlayerPropertiesManager.GetTargetPlayerProperty(PV.ViewID, PlayerPropertiesManager.livesRemaining) <= 0 ) {
            Debug.Log("YOU ARE DEAD");

            if (PV.IsMine && CheckMatchOver()) {
                CardManager.LoadCardPicking();
            }
            
        }

        //PV.RPC("RPC_HideBody", RpcTarget.All, controller); //TODO: Not used
        //StartCoroutine(DeleteBody(killer, body, suicide));
        DeleteBody(killer, body, suicide);
    }

    void TraceKill(string killer, string body, bool suicide)
    {   
        // Debug.Log("New feed: " + killer +  " killed " + body);
        PV.RPC("RPC_TraceKill", RpcTarget.All, killer, body, suicide);
    }

    void DeleteBody(string killer, string body, bool suicide)
    {
        // if(!suicide) // needs to be IEnumerator not void
        // {
        //     yield return new WaitForSeconds(3);
        // }
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

    bool CheckMatchOver() {
        int playersLeft = 0;

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            if (PlayerPropertiesManager.GetTargetPlayerPropertyAC(player.ActorNumber, PlayerPropertiesManager.livesRemaining) > 0) {
                playersLeft++;

                if (playersLeft >= 2) {
                    return false;
                }
            }
            //Debug.Log("ID: " + player.ActorNumber + "Lives: " + PlayerPropertiesManager.GetTargetPlayerPropertyAC(player.ActorNumber, PlayerPropertiesManager.livesRemaining));
        }

        Debug.Log("MATCH OVER");

        return true; //all but one player dead is over
    }
}
