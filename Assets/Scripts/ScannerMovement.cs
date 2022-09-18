using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerMovement : MonoBehaviour
{
    [SerializeField] GameObject[] locations, LogsPrefabs, plankPrefabs;
    [SerializeField] GameObject scorePods;

    private float spawnPositionY = -4.1f, spPosY = -0.25f;
    public bool isActive = false;

    private void Awake()
    {
        SpawnPlanks();
    }

    private void Update()
    {
        if(isActive)
        {
            spawnScores();
        }
    }

    void spawnScores()
    {
        scorePods.SetActive(true);
    }

    void SpawnPlanks()
    {
        int index;
        Vector3 spawnPosition;

        for (int i = 0; i < locations.Length; i++)
        {
            index = FindRandomIndex(0, LogsPrefabs.Length);
            spawnPosition = locations[i].transform.position;
            GameObject child;
            if (locations[i].name == "spLoc")
            {
                spawnPosition.y = spPosY;
                child = Instantiate(plankPrefabs[index], spawnPosition, locations[i].transform.rotation);
            }
            else
            {
                spawnPosition.y = spawnPositionY;
                child = Instantiate(LogsPrefabs[index], spawnPosition, locations[i].transform.rotation);
            }

            child.transform.parent = locations[i].transform;
        }
    }

    int FindRandomIndex(int min, int max)
    {
        return Random.Range(min, max);
    }
}
