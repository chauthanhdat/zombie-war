using System.Collections.Generic;
using UnityEngine;
using ZombieWar.Core.Events;
using ZombieWar.UI;

namespace ZombieWar.Core.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        [Header("UI Panels")]
        public GameObject mainMenuPanel;
        public GameObject gameplayHUD;
        public GameObject pauseMenuPanel;
        public GameObject gameOverPanel;
        public GameObject settingsPanel;
        public GameObject inventoryPanel;
        
        [Header("Popup System")]
        public Transform popupContainer;
        public GameObject genericPopupPrefab;
        
        [Header("Events")]
        // public GameEvent OnUIStateChanged;
        
        private Stack<GameObject> activePopups = new Stack<GameObject>();
        private Dictionary<string, GameObject> registeredPanels = new Dictionary<string, GameObject>();
        private GameObject currentActivePanel;
        
        public enum UIState
        {
            MainMenu,
            Gameplay,
            Paused,
            GameOver,
            Settings,
            Inventory
        }
        
        private UIState currentUIState = UIState.MainMenu;
        public UIState CurrentUIState => currentUIState;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeUI();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeUI()
        {
            // Register all UI panels
            RegisterPanel("MainMenu", mainMenuPanel);
            RegisterPanel("GameplayHUD", gameplayHUD);
            RegisterPanel("PauseMenu", pauseMenuPanel);
            RegisterPanel("GameOver", gameOverPanel);
            RegisterPanel("Settings", settingsPanel);
            RegisterPanel("Inventory", inventoryPanel);
            
            // Start with main menu
            ShowPanel("MainMenu");
        }
        
        private void RegisterPanel(string panelName, GameObject panel)
        {
            if (panel != null)
            {
                registeredPanels[panelName] = panel;
                panel.SetActive(false);
            }
        }
        
        public void ShowPanel(string panelName)
        {
            // Hide current active panel
            if (currentActivePanel != null)
            {
                currentActivePanel.SetActive(false);
            }
            
            // Show new panel
            if (registeredPanels.ContainsKey(panelName))
            {
                currentActivePanel = registeredPanels[panelName];
                currentActivePanel.SetActive(true);
                
                // Update UI state based on panel
                UpdateUIState(panelName);
            }
            else
            {
                Debug.LogWarning($"Panel '{panelName}' not found!");
            }
        }
        
        private void UpdateUIState(string panelName)
        {
            UIState newState = panelName switch
            {
                "MainMenu" => UIState.MainMenu,
                "GameplayHUD" => UIState.Gameplay,
                "PauseMenu" => UIState.Paused,
                "GameOver" => UIState.GameOver,
                "Settings" => UIState.Settings,
                "Inventory" => UIState.Inventory,
                _ => currentUIState
            };
            
            if (newState != currentUIState)
            {
                currentUIState = newState;
                // OnUIStateChanged?.Invoke();
            }
        }
        
        // Popup System
        public void ShowPopup(string title, string message, System.Action onConfirm = null, System.Action onCancel = null)
        {
            if (genericPopupPrefab == null || popupContainer == null)
            {
                Debug.LogWarning("Popup system not properly configured!");
                return;
            }
            
            GameObject popup = Instantiate(genericPopupPrefab, popupContainer);
            
            // Configure popup
            PopupController popupController = popup.GetComponent<PopupController>();
            if (popupController != null)
            {
                popupController.Setup(title, message, onConfirm, onCancel);
            }
            
            activePopups.Push(popup);
            popup.SetActive(true);
        }
        
        public void CloseTopPopup()
        {
            if (activePopups.Count > 0)
            {
                GameObject topPopup = activePopups.Pop();
                if (topPopup != null)
                {
                    Destroy(topPopup);
                }
            }
        }
        
        public void CloseAllPopups()
        {
            while (activePopups.Count > 0)
            {
                CloseTopPopup();
            }
        }
        
        // Quick access methods for common UI transitions
        public void ShowMainMenu()
        {
            ShowPanel("MainMenu");
            Time.timeScale = 1f;
        }
        
        public void ShowGameplayHUD()
        {
            ShowPanel("GameplayHUD");
            Time.timeScale = 1f;
        }
        
        public void ShowPauseMenu()
        {
            ShowPanel("PauseMenu");
            Time.timeScale = 0f;
        }
        
        public void ShowGameOverScreen()
        {
            ShowPanel("GameOver");
            Time.timeScale = 0f;
        }
        
        public void ShowSettings()
        {
            ShowPanel("Settings");
        }
        
        public void ShowInventory()
        {
            ShowPanel("Inventory");
        }
        
        // Utility methods
        public bool IsPopupActive()
        {
            return activePopups.Count > 0;
        }
        
        public bool IsPanelActive(string panelName)
        {
            return registeredPanels.ContainsKey(panelName) && 
                   registeredPanels[panelName] == currentActivePanel &&
                   currentActivePanel.activeInHierarchy;
        }
        
        // Handle back button/escape key
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleBackAction();
            }
        }
        
        private void HandleBackAction()
        {
            // Close popup if any are open
            if (IsPopupActive())
            {
                CloseTopPopup();
                return;
            }
            
            // Handle based on current UI state
            switch (currentUIState)
            {
                case UIState.Gameplay:
                    ShowPauseMenu();
                    break;
                case UIState.Paused:
                    ShowGameplayHUD();
                    break;
                case UIState.Settings:
                case UIState.Inventory:
                    ShowGameplayHUD();
                    break;
            }
        }
    }
}