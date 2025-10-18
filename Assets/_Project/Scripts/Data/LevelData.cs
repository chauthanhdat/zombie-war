using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieWar.Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ZombieWar/Data/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Information")]
        public string levelName = "Level 1";
        
        [Header("Level Settings")]
        public float timeBetweenWaves = 10f; // Rest time between waves
        public bool showWaveCountdown = true;
        
        [Header("Waves")]
        public List<WaveData> waves = new List<WaveData>();
        
        [Header("Level Completion")]
        public bool mustCompleteAllWaves = true;
        public float levelTimeLimit = -1f; // -1 for no time limit
        
        /// <summary>
        /// Get total number of waves in this level
        /// </summary>
        public int GetWaveCount()
        {
            return waves.Count;
        }
        
        /// <summary>
        /// Get estimated total level duration
        /// </summary>
        public float GetEstimatedDuration()
        {
            float totalTime = 0f;
            
            for (int i = 0; i < waves.Count; i++)
            {
                if (waves[i] != null)
                {
                    totalTime += waves[i].GetEstimatedDuration();
                    
                    // Add time between waves (except for last wave)
                    if (i < waves.Count - 1)
                        totalTime += timeBetweenWaves;
                }
            }
            
            return totalTime;
        }
        
        /// <summary>
        /// Get total number of enemies across all waves
        /// </summary>
        public int GetTotalEnemyCount()
        {
            int total = 0;
            foreach (var wave in waves)
            {
                if (wave != null)
                    total += wave.GetTotalEnemyCount();
            }
            return total;
        }
        
        /// <summary>
        /// Validate level configuration
        /// </summary>
        public bool IsValidLevel()
        {
            if (waves.Count == 0)
                return false;
                
            foreach (var wave in waves)
            {
                if (wave == null || !wave.IsValidWave())
                    return false;
            }
            
            return true;
        }
    }
}
