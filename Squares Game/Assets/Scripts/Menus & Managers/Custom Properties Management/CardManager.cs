using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = System.Random;
using TMPro;

public class CardManager : MonoBehaviourPunCallbacks
{
    private int[] playableLevels = new int[2];
    public int currentPlayerID;
    public int currentPlayerIndex;

    public int[] displayedCards;

    [SerializeField] Card[] allCards;

    [SerializeField] GameObject[] cardsUI;

    [SerializeField] TMP_Text playerTurnText;

    void Start() {
        Cursor.lockState = CursorLockMode.Confined;
        GenerateCards();
        PlayersTurn();
    }

    
    public static void LoadCardPicking() {
        PhotonNetwork.LoadLevel(3);
    }

    public void LoadNextLevel() {
        // Random r = new Random();        //TODO:
        // PhotonNetwork.LoadLevel(playableLevels[r.Next(0, playableLevels.Length)]);  //loads random level out of array

        PhotonNetwork.LoadLevel(2);
    }

    public void PlayersTurn() {

        playerTurnText.text = PhotonNetwork.PlayerList[0].NickName + "'s Turn!";
        // foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
        //     if (PlayerPropertiesManager.GetTargetPlayerPropertyAC(player.ActorNumber, PlayerPropertiesManager.livesRemaining) > 0) {
        //         playersLeft++;

        //         if (playersLeft >= 2) {
        //             return false;
        //         }
        //     }
        //     //Debug.Log("ID: " + player.ActorNumber + "Lives: " + PlayerPropertiesManager.GetTargetPlayerPropertyAC(player.ActorNumber, PlayerPropertiesManager.livesRemaining));
        // }
    }

    void GenerateCards() {
        Random r = new Random();
        int[] cardIndexes = new int[]{r.Next(0, allCards.Length), r.Next(0, allCards.Length), r.Next(0, allCards.Length)}; //TODO: add more than three allCards per screne
        displayedCards = cardIndexes;

        // Debug.Log(cardIndexes[0] + " " + cardIndexes[1] + " " + cardIndexes[2]);

        //TODO: instantiate the allCards according to this number
        for (int i = 0; i < cardIndexes.Length; i++) {
            //cardsUI[i].GetComponentInChildren<TMP_Text>().text = allCards[cardIndexes[i]].nameText;
            cardsUI[i].GetComponentInChildren<CardLoader>().LoadCard(allCards[cardIndexes[i]].nameText, allCards[cardIndexes[i]].statText);
        }

    }

    public void PickCard(int index) {
        int chosenCardIndex = displayedCards[index];

        for (int i = 0; i < allCards[chosenCardIndex].stats.Length; i++) {
            Debug.Log("applying stat: " + PhotonNetwork.PlayerList[0].NickName + " " + allCards[chosenCardIndex].stats[i].property + " " + allCards[chosenCardIndex].stats[i].value + " " + allCards[chosenCardIndex].stats[i].multiply + " " + allCards[chosenCardIndex].stats[i].add);
            PlayerPropertiesManager.ChangeTargetPlayerPropertyAC(PhotonNetwork.PlayerList[0].ActorNumber, allCards[chosenCardIndex].stats[i].property, allCards[chosenCardIndex].stats[i].value, allCards[chosenCardIndex].stats[i].multiply, allCards[chosenCardIndex].stats[i].add);
        }

    }
}
