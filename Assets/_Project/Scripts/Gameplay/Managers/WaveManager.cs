using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZombieWar.Data;
using ZombieWar.Gameplay.Combat;

namespace ZombieWar.Gameplay.Managers
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public Transform[] spawnPoints;
        public float spawnRadius = 2f;
        
        [Header("Current Wave")]
        public WaveData currentWave;
        
        [Header("Debug")]
        public bool showDebugInfo = true;
        
        // Events
        [System.Serializable]
        public class WaveEvents
        {
            public UnityEvent<int> OnWaveStart;
            public UnityEvent<int> OnWaveComplete;
            public UnityEvent<GameObject> OnEnemySpawned;
            public UnityEvent<GameObject> OnEnemyKilled;
            public UnityEvent<string> OnWaveEvent;
        }
        
        public WaveEvents waveEvents;
        
        // Private variables
        private List<GameObject> activeEnemies = new List<GameObject>();
        private List<Coroutine> activeSpawnCoroutines = new List<Coroutine>();
        private bool isWaveActive = false;
        private float waveStartTime;
        private int currentWaveNumber = 0;
        
        // Properties
        public bool IsWaveActive => isWaveActive;
        public int ActiveEnemyCount => activeEnemies.Count;
        public float WaveElapsedTime => isWaveActive ? Time.time - waveStartTime : 0f;
        public int CurrentWaveNumber => currentWaveNumber;
        
        private void Start()
        {
            // Auto-find spawn points if none assigned
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                FindSpawnPoints();
            }
        }
        
        public void StartWave(WaveData wave)
        {
            if (isWaveActive)
            {
                Debug.LogWarning("Cannot start wave - another wave is already active!");
                return;
            }
            
            if (wave == null || !wave.IsValidWave())
            {
                Debug.LogError("Cannot start wave - invalid wave data!");
                return;
            }
            
            currentWave = wave;
            currentWaveNumber++;
            isWaveActive = true;
            waveStartTime = Time.time;
            
            Debug.Log($"Starting Wave {currentWaveNumber}: {wave.waveName}");
            waveEvents.OnWaveStart?.Invoke(currentWaveNumber);
            
            StartCoroutine(ProcessWave());
        }
        
        /// <summary>
        /// Stop current wave and clean up
        /// </summary>
        public void StopWave()
        {
            if (!isWaveActive)
                return;
            
            // Stop all spawn coroutines
            foreach (var coroutine in activeSpawnCoroutines)
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
            }
            activeSpawnCoroutines.Clear();
            
            isWaveActive = false;
            
            Debug.Log($"Wave {currentWaveNumber} stopped");
        }
        
        /// <summary>
        /// Main wave processing coroutine
        /// </summary>
        private IEnumerator ProcessWave()
        {
            // Wait for wave start delay
            if (currentWave.waveStartDelay > 0)
            {
                yield return new WaitForSeconds(currentWave.waveStartDelay);
            }
            
            // Start all spawn groups
            foreach (var group in currentWave.spawnGroups)
            {
                var coroutine = StartCoroutine(ProcessSpawnGroup(group));
                activeSpawnCoroutines.Add(coroutine);
            }
            
            // Process wave events
            if (currentWave.waveEvents.Count > 0)
            {
                var eventCoroutine = StartCoroutine(ProcessWaveEvents());
                activeSpawnCoroutines.Add(eventCoroutine);
            }
            
            // Handle infinite spawning
            if (currentWave.infiniteWave)
            {
                var infiniteCoroutine = StartCoroutine(ProcessInfiniteSpawning());
                activeSpawnCoroutines.Add(infiniteCoroutine);
            }
            
            // Wait for wave completion
            yield return StartCoroutine(WaitForWaveCompletion());
            
            // Wave completed
            CompleteWave();
        }
        
        /// <summary>
        /// Process a spawn group
        /// </summary>
        private IEnumerator ProcessSpawnGroup(SpawnGroup group)
        {
            // Wait for group start delay
            if (group.groupStartDelay > 0)
            {
                yield return new WaitForSeconds(group.groupStartDelay);
            }
            
            Debug.Log($"Starting spawn group: {group.groupName}");
            
            // Spawn all enemies in the group
            List<Coroutine> enemySpawnCoroutines = new List<Coroutine>();
            
            foreach (var enemyInfo in group.enemies)
            {
                var coroutine = StartCoroutine(SpawnEnemyType(enemyInfo));
                enemySpawnCoroutines.Add(coroutine);
            }
            
            // Wait for group completion if required
            if (group.waitForGroupCompletion)
            {
                foreach (var coroutine in enemySpawnCoroutines)
                {
                    yield return coroutine;
                }
                
                // Group cooldown
                if (group.groupCooldown > 0)
                {
                    yield return new WaitForSeconds(group.groupCooldown);
                }
            }
            
            Debug.Log($"Completed spawn group: {group.groupName}");
        }
        
        /// <summary>
        /// Spawn a specific enemy type
        /// </summary>
        private IEnumerator SpawnEnemyType(EnemySpawnInfo enemyInfo)
        {
            // Wait for spawn delay
            if (enemyInfo.spawnDelay > 0)
            {
                yield return new WaitForSeconds(enemyInfo.spawnDelay);
            }
            
            // Spawn enemies
            for (int i = 0; i < enemyInfo.spawnCount; i++)
            {
                SpawnEnemy(enemyInfo);
                
                // Wait between spawns (except for last enemy)
                if (i < enemyInfo.spawnCount - 1 && enemyInfo.spawnInterval > 0)
                {
                    yield return new WaitForSeconds(enemyInfo.spawnInterval);
                }
            }
        }
        
        private void SpawnEnemy(EnemySpawnInfo enemyInfo)
        {
            Vector3 spawnPosition = GetSpawnPosition(enemyInfo);
            GameObject enemy = Instantiate(enemyInfo.enemyPrefab, spawnPosition, Quaternion.identity);
            
            // Apply behavior overrides
            ApplyEnemyOverrides(enemy, enemyInfo);
            
            // Track enemy
            activeEnemies.Add(enemy);
            
            // Setup enemy death callback
            var enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // enemyHealth.OnDeath.AddListener(() => OnEnemyDied(enemy));
            }
            
            waveEvents.OnEnemySpawned?.Invoke(enemy);
            
            if (showDebugInfo)
            {
                Debug.Log($"Spawned {enemy.name} at {spawnPosition}");
            }
        }
        
        private Vector3 GetSpawnPosition(EnemySpawnInfo enemyInfo)
        {
            if (!enemyInfo.useRandomSpawnPoints && enemyInfo.specificSpawnPoints.Count > 0)
            {
                // Use specific spawn points
                var spawnPoint = enemyInfo.specificSpawnPoints[Random.Range(0, enemyInfo.specificSpawnPoints.Count)];
                if (spawnPoint != null)
                    return spawnPoint.position;
            }
            
            // Use random spawn point
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                if (spawnPoint != null)
                {
                    Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                    randomOffset.y = 0; // Keep on same Y level
                    return spawnPoint.position + randomOffset;
                }
            }
            
            // Fallback to world origin
            Debug.LogWarning("No spawn points available, spawning at origin");
            return Vector3.zero;
        }
        
        /// <summary>
        /// Apply behavior overrides to spawned enemy
        /// </summary>
        private void ApplyEnemyOverrides(GameObject enemy, EnemySpawnInfo enemyInfo)
        {
            // Health override
            if (enemyInfo.overrideHealth)
            {
                var health = enemy.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    // health.SetMaxHealth(health.MaxHealth * enemyInfo.healthMultiplier);
                }
            }
            
            // Speed override
            if (enemyInfo.overrideSpeed)
            {
                var movement = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (movement != null)
                {
                    movement.speed *= enemyInfo.speedMultiplier;
                }
            }
            
            // Damage override
            if (enemyInfo.overrideDamage)
            {
                // This would depend on your enemy damage system
                // Example: Apply damage multiplier to weapon or attack component
            }
        }
        
        /// <summary>
        /// Handle enemy death
        /// </summary>
        private void OnEnemyDied(GameObject enemy)
        {
            activeEnemies.Remove(enemy);
            waveEvents.OnEnemyKilled?.Invoke(enemy);
            
            if (showDebugInfo)
            {
                Debug.Log($"Enemy died: {enemy.name}. Remaining: {activeEnemies.Count}");
            }
        }
        
        /// <summary>
        /// Process wave events
        /// </summary>
        private IEnumerator ProcessWaveEvents()
        {
            foreach (var waveEvent in currentWave.waveEvents)
            {
                yield return new WaitForSeconds(waveEvent.triggerTime);
                
                // Check conditions
                if (waveEvent.requireAllEnemiesDead && activeEnemies.Count > 0)
                    continue;
                    
                if (waveEvent.triggerOnEnemyCount >= 0 && activeEnemies.Count != waveEvent.triggerOnEnemyCount)
                    continue;
                
                // Trigger event
                ProcessWaveEvent(waveEvent);
            }
        }
        
        /// <summary>
        /// Process a specific wave event
        /// </summary>
        private void ProcessWaveEvent(WaveEvent waveEvent)
        {
            Debug.Log($"Triggering wave event: {waveEvent.eventName}");
            
            switch (waveEvent.eventType)
            {
                case WaveEventType.SpawnBoss:
                    if (waveEvent.spawnPrefab != null)
                    {
                        Vector3 spawnPos = waveEvent.spawnLocation != null ? 
                            waveEvent.spawnLocation.position : GetRandomSpawnPosition();
                        Instantiate(waveEvent.spawnPrefab, spawnPos, Quaternion.identity);
                    }
                    break;
                    
                case WaveEventType.PlaySound:
                    if (waveEvent.eventSound != null)
                    {
                        AudioSource.PlayClipAtPoint(waveEvent.eventSound, Camera.main.transform.position);
                    }
                    break;
                    
                case WaveEventType.ShowMessage:
                    // This would integrate with your UI system
                    Debug.Log($"Wave Message: {waveEvent.eventMessage}");
                    break;
            }
            
            waveEvents.OnWaveEvent?.Invoke(waveEvent.eventName);
        }
        
        /// <summary>
        /// Handle infinite spawning
        /// </summary>
        private IEnumerator ProcessInfiniteSpawning()
        {
            while (isWaveActive && currentWave.infiniteWave)
            {
                yield return new WaitForSeconds(currentWave.infiniteSpawnInterval);
                
                // Spawn random enemy from first group
                if (currentWave.spawnGroups.Count > 0 && currentWave.spawnGroups[0].enemies.Count > 0)
                {
                    var randomEnemy = currentWave.spawnGroups[0].enemies[Random.Range(0, currentWave.spawnGroups[0].enemies.Count)];
                    SpawnEnemy(randomEnemy);
                }
            }
        }
        
        /// <summary>
        /// Wait for wave completion conditions
        /// </summary>
        private IEnumerator WaitForWaveCompletion()
        {
            float waveTimeout = currentWave.waveDuration > 0 ? currentWave.waveDuration : float.MaxValue;
            
            while (isWaveActive)
            {
                // Check time-based victory
                if (currentWave.timeBasedVictory && WaveElapsedTime >= waveTimeout)
                {
                    break;
                }
                
                // Check kill all enemies victory
                if (currentWave.mustKillAllEnemies && activeEnemies.Count == 0 && !AreSpawnCoroutinesRunning())
                {
                    break;
                }
                
                // Check timeout
                if (WaveElapsedTime >= waveTimeout)
                {
                    break;
                }
                
                yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
            }
        }
        
        /// <summary>
        /// Check if any spawn coroutines are still running
        /// </summary>
        private bool AreSpawnCoroutinesRunning()
        {
            return activeSpawnCoroutines.Count > 0;
        }
        
        /// <summary>
        /// Complete the current wave
        /// </summary>
        private void CompleteWave()
        {
            isWaveActive = false;
            activeSpawnCoroutines.Clear();
            
            Debug.Log($"Wave {currentWaveNumber} completed!");
            waveEvents.OnWaveComplete?.Invoke(currentWaveNumber);
        }
        
        /// <summary>
        /// Get random spawn position
        /// </summary>
        private Vector3 GetRandomSpawnPosition()
        {
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                if (spawnPoint != null)
                    return spawnPoint.position;
            }
            return Vector3.zero;
        }
        
        /// <summary>
        /// Auto-find spawn points in scene
        /// </summary>
        private void FindSpawnPoints()
        {
            GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
            spawnPoints = new Transform[spawnPointObjects.Length];
            
            for (int i = 0; i < spawnPointObjects.Length; i++)
            {
                spawnPoints[i] = spawnPointObjects[i].transform;
            }
            
            if (spawnPoints.Length == 0)
            {
                Debug.LogWarning("No spawn points found! Create GameObjects with 'SpawnPoint' tag or assign spawn points manually.");
            }
            else
            {
                Debug.Log($"Found {spawnPoints.Length} spawn points automatically.");
            }
        }
        
        /// <summary>
        /// Get wave progress (0-1)
        /// </summary>
        public float GetWaveProgress()
        {
            if (!isWaveActive || currentWave == null)
                return 0f;
                
            if (currentWave.waveDuration <= 0)
                return 0f; // Infinite wave
                
            return Mathf.Clamp01(WaveElapsedTime / currentWave.waveDuration);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (spawnPoints != null)
            {
                Gizmos.color = Color.red;
                foreach (var spawnPoint in spawnPoints)
                {
                    if (spawnPoint != null)
                    {
                        Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
                        Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 2f);
                    }
                }
            }
        }
    }
}