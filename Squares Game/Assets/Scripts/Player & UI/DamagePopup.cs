using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] TMP_Text damageText;

    // void Start()
    // {
    //     //Debug.Log("Damage plane: ");
    // }

    public void SetDamage(float damage)
    {
        damageText.text = damage.ToString();
    }

}
