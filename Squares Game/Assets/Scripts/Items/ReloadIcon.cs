using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadIcon : MonoBehaviour
{

    public Image LoadingBar;
    float currentValue;
    bool isReloading = false;

    // Update is called once per frame
    // void Update()
    // {
    //     if (isReloading) {
    //         ReloadAnimation();
    //     }
    // }

    // void ReloadAnimation() {
    //     if (currentValue < 100) {
	// 		currentValue += PlayerController.items[PlayerController.itemIndex].GunInfo.reloadTime * Time.deltaTime;
	// 	} else {
	// 		LoadingBar.SetActive (false);
	// 	}
 
	// 	LoadingBar.fillAmount = currentValue / 100;
    // }
}
