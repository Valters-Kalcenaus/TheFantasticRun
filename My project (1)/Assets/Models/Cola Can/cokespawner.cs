using UnityEngine;

public class cokespawner : MonoBehaviour
{
    public GameObject collectiblePrefab; 
    public Transform spawnPointsParent; 

    void Start()
    {
        SpawnCollectible();
    }

    void SpawnCollectible()
    {
        Transform[] spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();

        int spawnIndex = Random.Range(1, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        Instantiate(collectiblePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}

