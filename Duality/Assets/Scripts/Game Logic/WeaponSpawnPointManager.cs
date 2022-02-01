using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class WeaponSpawnPointManager : MonoBehaviour
{
    public static WeaponSpawnPointManager Instance;
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
