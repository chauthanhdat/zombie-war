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
        [SerializeField] private float spawnInterval = 10f;

        private int enemyCount = 0;

        private void Start()
        {
            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            while (enemyCount < 10)
            {
                SpawnEnemy();
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
    }
}
