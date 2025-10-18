using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieWar.Data
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        [Header("Enemy Configuration")]
        public GameObject enemyPrefab;
        public int spawnCount = 1;
        
        [Header("Timing")]
        public float spawnDelay = 0f; // Delay before spawning this enemy type
        public float spawnInterval = 1f; // Time between individual spawns of this type
        
        [Header("Spawn Location")]
        public bool useRandomSpawnPoints = true;
        public List<Transform> specificSpawnPoints = new List<Transform>();
        
        [Header("Behavior Overrides")]
        public bool overrideHealth = false;
        [Range(0.1f, 5f)]
        public float healthMultiplier = 1f;
        
        public bool overrideSpeed = false;
        [Range(0.1f, 3f)]
        public float speedMultiplier = 1f;
        
        public bool overrideDamage = false;
        [Range(0.1f, 5f)]
        public float damageMultiplier = 1f;
    }

    [System.Serializable]
    public class SpawnGroup
    {
        [Header("Group Settings")]
        public string groupName = "Spawn Group";
        public float groupStartDelay = 0f; // When this group starts spawning relative to wave start
        
        [Header("Enemy Types")]
        public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
        
        [Header("Group Completion")]
        public bool waitForGroupCompletion = false; // Wait for all enemies in this group to spawn before continuing
        public float groupCooldown = 2f; // Time to wait after group completion
    }

    [CreateAssetMenu(fileName = "WaveData", menuName = "ZombieWar/Data/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Header("Wave Information")]
        public string waveName = "Wave 1";
        public int waveNumber = 1;
        
        [Header("Wave Timing")]
        public float waveStartDelay = 3f; // Delay before wave starts
        public float waveDuration = 60f; // Maximum wave duration (-1 for unlimited)
        
        [Header("Spawn Groups")]
        public List<SpawnGroup> spawnGroups = new List<SpawnGroup>();
        
        [Header("Wave Conditions")]
        public bool infiniteWave = false; // Keep spawning enemies until time runs out
        public float infiniteSpawnInterval = 5f; // Time between infinite spawns
        
        [Header("Victory Conditions")]
        public bool mustKillAllEnemies = true; // Wave ends when all enemies are defeated
        public bool timeBasedVictory = false; // Wave ends when time runs out
        
        [Header("Special Events")]
        public List<WaveEvent> waveEvents = new List<WaveEvent>();
        
        /// <summary>
        /// Get total number of enemies that will spawn in this wave
        /// </summary>
        public int GetTotalEnemyCount()
        {
            int total = 0;
            foreach (var group in spawnGroups)
            {
                foreach (var enemy in group.enemies)
                {
                    total += enemy.spawnCount;
                }
            }
            return total;
        }
        
        /// <summary>
        /// Get estimated wave duration based on spawn timings
        /// </summary>
        public float GetEstimatedDuration()
        {
            if (waveDuration > 0)
                return waveDuration;
                
            float maxTime = 0f;
            foreach (var group in spawnGroups)
            {
                float groupEndTime = group.groupStartDelay;
                foreach (var enemy in group.enemies)
                {
                    float enemyEndTime = enemy.spawnDelay + (enemy.spawnCount * enemy.spawnInterval);
                    groupEndTime = Mathf.Max(groupEndTime, groupEndTime + enemyEndTime);
                }
                maxTime = Mathf.Max(maxTime, groupEndTime);
            }
            return maxTime;
        }
        
        /// <summary>
        /// Validate wave configuration
        /// </summary>
        public bool IsValidWave()
        {
            if (spawnGroups.Count == 0)
                return false;
                
            foreach (var group in spawnGroups)
            {
                if (group.enemies.Count == 0)
                    return false;
                    
                foreach (var enemy in group.enemies)
                {
                    if (enemy.enemyPrefab == null || enemy.spawnCount <= 0)
                        return false;
                }
            }
            
            return true;
        }
    }
    
    [System.Serializable]
    public class WaveEvent
    {
        [Header("Event Timing")]
        public string eventName = "Wave Event";
        public float triggerTime = 0f; // Time relative to wave start
        
        [Header("Event Type")]
        public WaveEventType eventType = WaveEventType.SpawnBoss;
        
        [Header("Event Data")]
        public GameObject spawnPrefab; // For spawn events
        public Transform spawnLocation; // Specific spawn location
        public string eventMessage = ""; // For UI messages
        public AudioClip eventSound; // Sound to play
        
        [Header("Conditions")]
        public bool requireAllEnemiesDead = false; // Only trigger if no enemies alive
        public int triggerOnEnemyCount = -1; // Trigger when enemy count reaches this number (-1 to ignore)
    }
    
    public enum WaveEventType
    {
        SpawnBoss,
        PlaySound,
        ShowMessage,
        ChangeMusic,
        TriggerEffect,
        Custom
    }
}
