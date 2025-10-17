using UnityEngine;
using ZombieWar.Core.Events;

namespace ZombieWar.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Game Settings")]
        public GameState currentState = GameState.Menu;
        
        // [Header("Events")]
        // public GameEvent OnGameStateChanged;
        // public GameEvent OnGamePaused;
        // public GameEvent OnGameResumed;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeGame()
        {
            // Initialize core systems
            Debug.Log("Game Manager Initialized");
        }
        
        public void ChangeGameState(GameState newState)
        {
            if (currentState == newState) return;
            
            GameState previousState = currentState;
            currentState = newState;
            
            Debug.Log($"Game state changed from {previousState} to {newState}");
            // OnGameStateChanged?.Invoke();
        }
        
        public void PauseGame()
        {
            Time.timeScale = 0f;
            // OnGamePaused?.Invoke();
        }
        
        public void ResumeGame()
        {
            Time.timeScale = 1f;
            // OnGameResumed?.Invoke();
        }
    }
    
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
}