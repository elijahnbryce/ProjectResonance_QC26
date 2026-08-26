using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int threshold;
    [SerializeField] GameObject enemy;
    [SerializeField] float spawnDelay;
    [SerializeField] int extraEnemies;
    private bool enemiesSpawning = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    int checkNumEnemies()
    {
        return transform.childCount;
    }

    IEnumerator spawnEnemies(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemy, transform);
            yield return new WaitForSeconds(spawnDelay);
        }
        enemiesSpawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        int numEnemies = checkNumEnemies();
        if (numEnemies < threshold && !enemiesSpawning)
        {
            enemiesSpawning = true;
            StartCoroutine(spawnEnemies(threshold - numEnemies + extraEnemies));
        }
    }
}
