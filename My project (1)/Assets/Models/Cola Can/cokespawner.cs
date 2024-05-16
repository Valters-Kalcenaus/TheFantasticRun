using UnityEngine;

public class cokespawner : MonoBehaviour
{
    public GameObject collectiblePrefab; 
    public Transform spawnPointsParent; 
    public int numberOfCokes = 2; 

    void Start()
    {
        SpawnCollectibles();
    }

    void SpawnCollectibles()
    {
        Transform[] spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();

        for (int i = 0; i < numberOfCokes; i++)
        {
            int spawnIndex = Random.Range(1, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            Instantiate(collectiblePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
