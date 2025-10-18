using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using ZombieWar.Gameplay.Combat;
using ZombieWar.UI.HUD;

namespace ZombieWar
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private HUDManager hUDManager;

        private CharacterHealth playerHealth;

        private int currentLevel;

        private void Start()
        {
            Initialize();
            LoadLevelConfig();
            StartCoroutine(StartLevel());
        }

        public void Initialize()
        {
            playerHealth = FindObjectOfType<CharacterHealth>();
            playerHealth.OnDeath += HandlePlayerDeath;
        }

        public void LoadLevelConfig()
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        }

        public void SetupLevel()
        {
            
        }

        public IEnumerator StartLevel()
        {
            yield return hUDManager.ShowLevelStartUI("Level " + currentLevel);
        }

        public void HandlePlayerDeath()
        {
            hUDManager.ShowLevelEndUI("MISSION FAILED", "TRY AGAIN!");
        }
        
        public void HandleLevelComplete()
        {
            hUDManager.ShowLevelEndUI("MISSION COMPLETE", "WELL DONE!");
        }
    }
}
