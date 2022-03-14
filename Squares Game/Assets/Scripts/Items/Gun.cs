using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public GameObject bulletImpactPrefab;
    
    public abstract override void Use(Vector3 tp);

    public abstract override void Reload();

    public abstract override void UpdateHUD();

    public abstract override void CancelUpdate();
       
}
