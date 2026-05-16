using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private int enemyAmount = 1;

    private void Awake()
    {
        for (int i = 0; i < enemyAmount; i++)
        {
            if (spawnPoints.Count == 0)
            {
                Debug.Log("No Valid spawn points remaining");
                break;
            }

            int selection = Random.Range(0, spawnPoints.Count);
            Instantiate(enemyPrefab, spawnPoints[selection].position, transform.rotation);
            spawnPoints.RemoveAt(selection);
        }
    }
}
