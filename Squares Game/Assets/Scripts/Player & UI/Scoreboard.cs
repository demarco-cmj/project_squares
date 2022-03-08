using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardUI;
    [SerializeField] GameObject scoreboardItemPrefab;
    private bool active = false;

    Dictionary<Player, ScoreboardItem> scoreboardList = new Dictionary<Player, ScoreboardItem>();

    void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            active = !active;
            scoreboardUI.SetActive(active);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        scoreboardList[player] = item;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }

    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardList[player].gameObject);
        scoreboardList.Remove(player);
    }
}
