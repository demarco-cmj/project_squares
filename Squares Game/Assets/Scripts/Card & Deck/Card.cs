using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/New Card")]
public class Card : ScriptableObject
{
    // public class Stat {
    //     public string property;
    //     public int value;
    //     bool multiply;
    //     bool add;
    // }

    public string nameText, statText;
    public StatChange[] stats;

}
