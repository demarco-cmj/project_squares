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

    //Health-Lives
    public static string livesRemaining = "LivesRemaining";

    //Guns
    public static string damageMod = "DamageMod", horizontalRecoilMod = "HorizontalRecoilMod", verticalRecoilMod = "VerticalRecoilMod", fireRateMod = "FireRateMod", 
                            bulletVelocityMod = "BulletVelocityMod", cooldownSpeedMod = "CooldownSpeedMod", reloadTimeMod = "ReloadTimeMod";

    //Bullets-Ammo
    public static string bulletSize = "BulletSize";

    //Movement
    public static string baseMoveSpeed = "BaseMoveSpeed";

    //Equipment
    public static string grappleRange = "GrappleRange";


    //private ExitGames.Client.Photon.Hashtable myCustomProps = new ExitGames.Client.Photon.Hashtable();

    // void Start() {
    //    //create hash with all props, mostly empty, for each player? OR call initialize func when player joins in lobby/everyone when starting game
    // }


    public static void InitalizeAllPlayersProperties() {
        Hashtable tempProperties = new Hashtable();

        //Add all props as zero

        //Health-Lives
        tempProperties.Add(livesRemaining, 2f);
        //Guns
        tempProperties.Add(damageMod, 1f);
        tempProperties.Add(horizontalRecoilMod, 1f);
        tempProperties.Add(verticalRecoilMod, 1f);
        tempProperties.Add(fireRateMod, 1f);
        tempProperties.Add(bulletVelocityMod, 1f);
        tempProperties.Add(cooldownSpeedMod, 1f);
        tempProperties.Add(reloadTimeMod, 1f);
        //Bullets-Ammo
        tempProperties.Add(bulletSize, 1f);
        //Movement
        tempProperties.Add(baseMoveSpeed, 1f);
        //Equipment
        tempProperties.Add(grappleRange, 1f);

        foreach (Player player in PhotonNetwork.PlayerList) {
            player.SetCustomProperties(tempProperties);

        }
    }

    //TODO: if player joins mid way?
    // public static void InitalizeTargetPlayerProperty(int id, string property, float valueMultiply) {
    //     Hashtable tempProperties = new Hashtable();

    //     //Add all props as zero
    //     tempProperties.Add(damageMod, 0);
    //     tempProperties.Add(horizontalRecoilMod, 0);
    //     tempProperties.Add(verticalRecoilMod, 0);
    //     tempProperties.Add(fireRateMod, 0);


    //     PhotonView.Find(id).Owner.SetCustomProperties(tempProperties);


    //     //PhotonNetwork.PlayerList

    // }

    public static void ChangeTargetPlayerProperty(int id, string property, float value, bool multiply, bool add) {
        Hashtable tempProperties = new Hashtable();
        tempProperties = PhotonView.Find(id).Owner.CustomProperties;

        if (multiply) {
            tempProperties[property] = (float)tempProperties[property] * value;
        } else if (add) {
            tempProperties[property] = (float)tempProperties[property] + value;
        }

        PhotonView.Find(id).Owner.CustomProperties = tempProperties;
    }

    public static void ChangeTargetPlayerPropertyAC(int actorNumber, string property, float value, bool multiply, bool add) {
        Hashtable tempProperties = new Hashtable();
        tempProperties = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).CustomProperties;

        if (multiply) {
            tempProperties[property] = (float)tempProperties[property] * value;
        } else if (add) {
            tempProperties[property] = (float)tempProperties[property] + value;
        }

        PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).CustomProperties = tempProperties;
    }

    public static float GetTargetPlayerProperty(int id, string property) {
        return (float)PhotonView.Find(id).Owner.CustomProperties[property];
    }

    public static float GetTargetPlayerPropertyAC(int actorNumber, string property) {
        return (float)PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).CustomProperties[property];
    }


}