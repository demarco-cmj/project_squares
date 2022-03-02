using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager inst;

    Spawnpoint[] spawnpoints;

    void Awake()
    {
        inst = this;
        spawnpoints = GetComponentsInChildren<Spawnpoint>();
    }

    public Transform GetSpawnpoint()    //TODO: Create dynamic spawns based on distance from all other players
    {
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform; //Gets location of random sp
    }
}
