using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillFeed : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] GameObject killItemPrefab;

    public void CreateItem(string text)
    {
        KillFeedItem item = Instantiate(killItemPrefab, container).GetComponent<KillFeedItem>();
        item.Initialize(text);
        Destroy(item, 5f);
    }
}
