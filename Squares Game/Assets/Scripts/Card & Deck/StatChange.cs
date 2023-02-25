using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/New Stat")]
public class StatChange : ScriptableObject
{
    public string property;
    public int value;
    public bool multiply;
    public bool add;

}
