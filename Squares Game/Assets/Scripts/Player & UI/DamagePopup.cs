using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] TMP_Text damageText;

    public void SetDamage(float damage)
    {
        damageText.text = damage.ToString();
    }

}
