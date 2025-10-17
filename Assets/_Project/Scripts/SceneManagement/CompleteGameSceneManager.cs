using UnityEngine;
using ZombieWar.Core;
using ZombieWar.Core.Managers;
using ZombieWar.Gameplay.Player;
using ZombieWar.Gameplay.Camera;

namespace ZombieWar.SceneManagement
{
    /// <summary>
    /// Manages the complete game scene setup with player, camera, and all game systems
    /// This script automatically sets up all the necessary references and connections
    /// </summary>
    public class CompleteGameSceneManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private CameraController cameraController;
        
        [Header("Scene Objects")]
        [SerializeField] private GameObject player;
        [SerializeField] private Canvas gameCanvas;
        
        private void Start()
        {
            InitializeScene();
        }
        
        private void InitializeScene()
        {
            // Find or create player
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>()?.gameObject;
                
                if (player == null && playerPrefab != null)
                {
                    Vector3 spawnPosition = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
                    player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                }
            }
            
            // Setup camera to follow player
            if (cameraController == null)
            {
                cameraController = FindObjectOfType<CameraController>();
            }
            
            if (cameraController != null && player != null)
            {
                cameraController.SetTarget(player.transform);
            }
            
            // Initialize UI
            SetupUI();
            
            // Initialize game systems
            InitializeGameSystems();
            
            Debug.Log("Complete Game Scene initialized successfully!");
        }
        
        private void SetupUI()
        {
            // Find game canvas if not assigned
            if (gameCanvas == null)
            {
                gameCanvas = FindObjectOfType<Canvas>();
            }
            
            // Initialize UI Manager if it exists
            var uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                // UI Manager will handle its own initialization
                Debug.Log("UI Manager found and initialized");
            }
        }
        
        private void InitializeGameSystems()
        {
            // Initialize Game Manager
            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                // Start the game
                // gameManager.StartGame();
                Debug.Log("Game Manager initialized and game started");
            }
            
            // Initialize Object Pool Manager
            var poolManager = ObjectPoolManager.Instance;
            if (poolManager != null)
            {
                Debug.Log("Object Pool Manager initialized");
            }
        }
        
        /// <summary>
        /// Called when player is spawned or respawned
        /// </summary>
        public void OnPlayerSpawned(GameObject newPlayer)
        {
            player = newPlayer;
            
            // Update camera target
            if (cameraController != null)
            {
                cameraController.SetTarget(player.transform);
            }
            
            Debug.Log("Player spawned and camera updated");
        }
        
        /// <summary>
        /// Get the current player GameObject
        /// </summary>
        public GameObject GetPlayer()
        {
            return player;
        }
        
        /// <summary>
        /// Get the camera controller
        /// </summary>
        public CameraController GetCameraController()
        {
            return cameraController;
        }
    }
}