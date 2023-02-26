using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = System.Random;

public class CardManager : MonoBehaviourPunCallbacks
{
    private int[] playableLevels = new int[2];
    public int currentPlayerID;

    [SerializeField] Card[] cards;

    [SerializeField] GameObject[] cardsUI;

    void Start() {
        Cursor.lockState = CursorLockMode.Confined;
        GenerateCards();
    }

    
    public static void LoadCardPicking() {
        PhotonNetwork.LoadLevel(3);
    }

    public void LoadNextLevel() {
        // Random r = new Random();        //TODO:
        // PhotonNetwork.LoadLevel(playableLevels[r.Next(0, playableLevels.Length)]);  //loads random level out of array

        PhotonNetwork.LoadLevel(2);
    }

    void GenerateCards() {
        Random r = new Random();
        int[] cardIndexes = new int[]{r.Next(0, cards.Length), r.Next(0, cards.Length), r.Next(0, cards.Length)}; //TODO: add more than three cards per screne

        Debug.Log(cardIndexes[0] + " " + cardIndexes[1] + " " + cardIndexes[2]);

        //TODO: instantiate the cards according to this number
        for (int i = 0; i < cardIndexes.Length; i++) {
            //cardsUI[i].GetComponentInChildren<TMP_Text>().text = cards[cardIndexes[i]].nameText;
            cardsUI[i].GetComponentInChildren<CardLoader>().LoadCard(cards[cardIndexes[i]].nameText, cards[cardIndexes[i]].statText);
        }

    }

    public void PickCard() {

        //PlayerPropertiesManager.ChangeTargetPlayerProperty(currentPlayerID, );
    }
}
