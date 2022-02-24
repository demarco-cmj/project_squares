using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPV;
    [SerializeField] TMP_Text tempText;

    void Start()
    {   
        if(playerPV.IsMine) //Hide own username
        {
            gameObject.SetActive(false);
        }

        tempText.text = playerPV.Owner.NickName;
    }
}
