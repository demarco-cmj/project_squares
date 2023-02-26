using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class PlayerPropertiesManager
{
    //TODO: use this space to abstract functions and add/change properties for specific players

    /************************* PLAYER MODIFIABLE STATS *************************/
    
    //Weapons
    // float damageMod, horizontalRecoilMod, verticalRecoilMod, fireRateMod, bulletVelocityMod, cooldownSpeedMod, reloadTimeMod;
    // int magazineSizeMod, bulletsPerTapMod, bulletBounces;

    // //Player Movement
    // float moveSpeedMod, jumpForceMod;

    // //Player Other
    // float healthMod;

    /************************* CUSTOM PROPERTIES *************************/

    public static string livesRemaining = "LivesRemaining";

    public static string damageMod = "DamageMod", horizontalRecoilMod = "HorizontalRecoilMod", verticalRecoilMod = "VerticalRecoilMod", fireRateMod = "FireRateMod", 
                            bulletVelocityMod = "BulletVelocityMod", cooldownSpeedMod = "CooldownSpeedMod", reloadTimeMod = "ReloadTimeMod";

    //private ExitGames.Client.Photon.Hashtable myCustomProps = new ExitGames.Client.Photon.Hashtable();

    // void Start() {
    //    //create hash with all props, mostly empty, for each player? OR call initialize func when player joins in lobby/everyone when starting game
    // }


    public static void InitalizeAllPlayersProperties() {
        Hashtable tempProperties = new Hashtable();

        //Add all props as zero
        tempProperties.Add(livesRemaining, 2);
        tempProperties.Add(damageMod, 0);
        tempProperties.Add(horizontalRecoilMod, 0);
        tempProperties.Add(verticalRecoilMod, 0);
        tempProperties.Add(fireRateMod, 2);

        foreach (Player player in PhotonNetwork.PlayerList) {
            player.SetCustomProperties(tempProperties);

        }

        //PhotonNetwork.PlayerList

    }

    public static void InitalizeTargetPlayerProperty(int id, string property, float valueMultiply) {
        Hashtable tempProperties = new Hashtable();

        //Add all props as zero
        tempProperties.Add(damageMod, 0);
        tempProperties.Add(horizontalRecoilMod, 0);
        tempProperties.Add(verticalRecoilMod, 0);
        tempProperties.Add(fireRateMod, 0);


        PhotonView.Find(id).Owner.SetCustomProperties(tempProperties);


        //PhotonNetwork.PlayerList

    }

    public static void ChangeTargetPlayerProperty(int id, string property, int value, bool multiply, bool add) {
        Hashtable tempProperties = new Hashtable();
        tempProperties = PhotonView.Find(id).Owner.CustomProperties;

        if (multiply) {
            tempProperties[property] = (int)tempProperties[property] * value;
        } else if (add) {
            tempProperties[property] = (int)tempProperties[property] + value;
        }

        PhotonView.Find(id).Owner.CustomProperties = tempProperties;
    }

    public static int GetTargetPlayerProperty(int id, string property) {
        return (int)PhotonView.Find(id).Owner.CustomProperties[property];
    }

    public static int GetTargetPlayerPropertyAC(int actorNumber, string property) {
        return (int)PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).CustomProperties[property];
    }
}