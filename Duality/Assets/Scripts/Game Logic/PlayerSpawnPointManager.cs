using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnPointManager : MonoBehaviour
{
    public static PlayerSpawnPointManager Instance;
    public List<Transform> spawnPoints;

    private void Start()
    {
        Instance = this;
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints.Add(transform.GetChild(i));
        }
    }

    public Transform getRandomSpawnPoint()
    {
        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)].transform;
    }
}
