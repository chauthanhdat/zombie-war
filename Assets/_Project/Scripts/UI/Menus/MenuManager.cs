using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using ZombieWar.Core.Managers;
using DG.Tweening;

namespace ZombieWar.UI.Menus
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Main Menu")]
        public Button playButton;
        public Button settingsButton;
        public Button creditsButton;
        public Button quitButton;
        
        [Header("Pause Menu")]
        public Button resumeButton;
        public Button mainMenuButton;
        public Button pauseSettingsButton;
        
        [Header("Game Over Menu")]
        public Button restartButton;
        public Button gameOverMainMenuButton;
        public TextMeshProUGUI finalScoreText;
        public TextMeshProUGUI finalKillCountText;
        public TextMeshProUGUI survivalTimeText;
        
        [Header("Settings Menu")]
        public Slider masterVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;
        public Dropdown qualityDropdown;
        public Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;
        public Button applySettingsButton;
        public Button cancelSettingsButton;
        
        [Header("Loading")]
        public GameObject loadingPanel;
        public Slider loadingProgressBar;
        public TextMeshProUGUI loadingText;
        
        private void Start()
        {
            InitializeMenus();
        }
        
        private void InitializeMenus()
        {
            // Main Menu buttons
            if (playButton != null)
            {
                playButton.onClick.AddListener(StartGame);
                AddButtonAnimation(playButton);
            }
            
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(() => UIManager.Instance.ShowSettings());
                AddButtonAnimation(settingsButton);
            }
            
            if (creditsButton != null)
            {
                creditsButton.onClick.AddListener(ShowCredits);
                AddButtonAnimation(creditsButton);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
                AddButtonAnimation(quitButton);
            }
            
            // Pause Menu buttons
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(ResumeGame);
                AddButtonAnimation(resumeButton);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
                AddButtonAnimation(mainMenuButton);
            }
            
            if (pauseSettingsButton != null)
            {
                pauseSettingsButton.onClick.AddListener(() => UIManager.Instance.ShowSettings());
                AddButtonAnimation(pauseSettingsButton);
            }
            
            // Game Over Menu buttons
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
                AddButtonAnimation(restartButton);
            }
            
            if (gameOverMainMenuButton != null)
            {
                gameOverMainMenuButton.onClick.AddListener(ReturnToMainMenu);
                AddButtonAnimation(gameOverMainMenuButton);
            }
            
            // Settings Menu
            InitializeSettings();
        }
        
        private void AddButtonAnimation(Button button)
        {
            // Add hover and click animations with DOTween
            button.transform.localScale = Vector3.one;
            
            // Create event triggers for hover effects
            var eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }
            
            // Hover enter
            var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => {
                button.transform.DOScale(1.05f, 0.2f).SetEase(Ease.OutQuart);
            });
            eventTrigger.triggers.Add(pointerEnter);
            
            // Hover exit
            var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => {
                button.transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuart);
            });
            eventTrigger.triggers.Add(pointerExit);
            
            // Click animation
            var pointerClick = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerClick.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
            pointerClick.callback.AddListener((data) => {
                button.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 1, 0.5f);
            });
            eventTrigger.triggers.Add(pointerClick);
        }
        
        private void InitializeSettings()
        {
            // Volume sliders
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = AudioListener.volume;
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            }
            
            // Quality dropdown
            if (qualityDropdown != null)
            {
                qualityDropdown.value = QualitySettings.GetQualityLevel();
                qualityDropdown.onValueChanged.AddListener(SetQuality);
            }
            
            // Resolution dropdown
            if (resolutionDropdown != null)
            {
                PopulateResolutionDropdown();
            }
            
            // Fullscreen toggle
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
                fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            }
            
            // Settings buttons
            if (applySettingsButton != null)
                applySettingsButton.onClick.AddListener(ApplySettings);
            
            if (cancelSettingsButton != null)
                cancelSettingsButton.onClick.AddListener(CancelSettings);
        }
        
        // Main Menu Actions
        public void StartGame()
        {
            StartCoroutine(LoadGameSceneAsync());
        }
        
        private System.Collections.IEnumerator LoadGameSceneAsync()
        {
            // Show loading screen
            if (loadingPanel != null)
                loadingPanel.SetActive(true);
            
            // Start loading the game scene
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("GameScene");
            loadOperation.allowSceneActivation = false;
            
            // Update loading progress
            while (loadOperation.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
                UpdateLoadingProgress(progress, "Loading game...");
                yield return null;
            }
            
            // Finish loading
            UpdateLoadingProgress(1f, "Ready!");
            yield return new WaitForSeconds(0.5f);
            
            loadOperation.allowSceneActivation = true;
            
            // Switch to gameplay UI
            UIManager.Instance.ShowGameplayHUD();
        }
        
        private void UpdateLoadingProgress(float progress, string text)
        {
            if (loadingProgressBar != null)
            {
                // Animate progress bar smoothly with DOTween
                loadingProgressBar.DOValue(progress, 0.3f).SetEase(Ease.OutQuart);
            }
            
            if (loadingText != null)
            {
                loadingText.text = text;
                
                // Add a subtle fade effect to the loading text
                loadingText.DOFade(0.5f, 0.5f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }
        
        public void ShowCredits()
        {
            UIManager.Instance.ShowPopup("Credits", 
                "Zombie War Game\\n\\nDeveloped by: Your Name\\nMusic by: Artist Name\\nSpecial Thanks: Team Members",
                null, null);
        }
        
        public void QuitGame()
        {
            UIManager.Instance.ShowPopup("Quit Game", 
                "Are you sure you want to quit?",
                () => {
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif
                },
                null);
        }
        
        // Pause Menu Actions
        public void ResumeGame()
        {
            UIManager.Instance.ShowGameplayHUD();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }
        
        public void ReturnToMainMenu()
        {
            UIManager.Instance.ShowPopup("Return to Main Menu",
                "Are you sure? You will lose all progress.",
                () => {
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("MainMenuScene");
                    UIManager.Instance.ShowMainMenu();
                },
                null);
        }
        
        // Game Over Actions
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            UIManager.Instance.ShowGameplayHUD();
        }
        
        public void ShowGameOverScreen(int finalScore, int killCount, float survivalTime)
        {
            // Update game over stats
            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {finalScore:N0}";
            
            if (finalKillCountText != null)
                finalKillCountText.text = $"Zombies Killed: {killCount}";
            
            if (survivalTimeText != null)
            {
                int minutes = Mathf.FloorToInt(survivalTime / 60f);
                int seconds = Mathf.FloorToInt(survivalTime % 60f);
                survivalTimeText.text = $"Survival Time: {minutes:00}:{seconds:00}";
            }
            
            // Show game over screen
            UIManager.Instance.ShowGameOverScreen();
        }
        
        // Settings Actions
        public void SetMasterVolume(float volume)
        {
            AudioListener.volume = volume;
        }
        
        public void SetMusicVolume(float volume)
        {
            // Implement music volume control
            Debug.Log($"Music volume set to: {volume}");
        }
        
        public void SetSFXVolume(float volume)
        {
            // Implement SFX volume control
            Debug.Log($"SFX volume set to: {volume}");
        }
        
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }
        
        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }
        
        private void PopulateResolutionDropdown()
        {
            if (resolutionDropdown == null) return;
            
            resolutionDropdown.ClearOptions();
            
            var options = new System.Collections.Generic.List<string>();
            Resolution[] resolutions = Screen.resolutions;
            int currentResolutionIndex = 0;
            
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = $"{resolutions[i].width} x {resolutions[i].height}";
                options.Add(option);
                
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        
        public void ApplySettings()
        {
            // Save settings to PlayerPrefs or a save system
            PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
            PlayerPrefs.SetInt("QualityLevel", qualityDropdown.value);
            PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
            
            UIManager.Instance.ShowGameplayHUD();
        }
        
        public void CancelSettings()
        {
            // Revert settings without saving
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            
            UIManager.Instance.ShowGameplayHUD();
        }
    }
}