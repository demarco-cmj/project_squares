using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher inst;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;


    void Awake()
    {
        inst = this;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //Connects to server using settings in package file generated with ID
        //Debug.Log("Connecting to Master");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        //Debug.Log("Connected to Master");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.inst.OpenMenu("title");
        //Debug.Log("Joined Lobby");
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }
    
    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.inst.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.inst.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient); 
    }

    public override void OnMasterClientSwitched(Player newHost) //Photon has automatic host migration
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed:" + message;
        MenuManager.inst.OpenMenu("error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.inst.OpenMenu("loading");
        Debug.Log("Left Room");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.inst.OpenMenu("loading");
        //Debug.Log("Joined Room");
    }

    public override void OnLeftRoom()
    {
        MenuManager.inst.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for(int i = 0; i  < roomList.Count; i++)
        {
            if(roomList[i].RemovedFromList)
				continue;
			Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void StartGame()
    {
        PlayerPropertiesManager.InitalizeAllPlayersProperties();
        PhotonNetwork.LoadLevel(2); //load the numbered scene for everyone
    }
}
