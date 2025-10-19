using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieWar.Gameplay.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform player;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnInterval = 5f;

        private int enemyCount = 0;

        private void Start()
        {
            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            while (enemyCount < 1)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }
            
            float elapsedTime = 0f;
            int enemiesToSpawn = 1;

            while (elapsedTime < 180f) // 3 minutes
            {
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    if (enemyCount >= 100) yield break;
                    SpawnEnemy();
                }

                enemiesToSpawn++; // Gradually increase the number of enemies to spawn
                elapsedTime += spawnInterval;
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnEnemy()
        {
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject enemy = Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
            enemy.GetComponent<ZombieAI>().SetTarget(player);

            enemyCount++;
        }

        private void OnDrawGizmos()
        {
            if (spawnPoints != null)
            {
                Gizmos.color = Color.red;
                foreach (var spawnPoint in spawnPoints)
                {
                    if (spawnPoint != null)
                    {
                        Gizmos.DrawSphere(spawnPoint.position, 2f);
                    }
                }
            }
        }
    }
}
