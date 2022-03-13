using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemObj;

    public abstract void Use(Vector3 tp);

    public abstract void Reload();
}
