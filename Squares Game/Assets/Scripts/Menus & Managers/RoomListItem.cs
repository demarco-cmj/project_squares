using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text displayText;

    public RoomInfo info;

    public void SetUp(RoomInfo tempInfo)
    {
        info = tempInfo;
        displayText.text = tempInfo.Name;
    }

    public void OnClick()
    {
        Launcher.inst.JoinRoom(info);
    }
}
