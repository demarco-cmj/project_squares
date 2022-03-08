using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class KillFeedItem : MonoBehaviour
{
    public TMP_Text feedText;

    public void Initialize(string feedMessage)
    {
        feedText.text = feedMessage;
        Destroy(gameObject, 5);
    }
}
