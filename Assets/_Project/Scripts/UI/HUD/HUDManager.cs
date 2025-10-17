using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ZombieWar.Gameplay.Combat;
using ZombieWar.Core.Events;
using DG.Tweening;

namespace ZombieWar.UI.HUD
{
    public class HUDManager : MonoBehaviour
    {
        [Header("Health UI")]
        public Slider healthBar;
        public TextMeshProUGUI healthText;
        public Image healthBarFill;
        
        [Header("Score UI")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI killCountText;
        
        [Header("Wave UI")]
        public TextMeshProUGUI waveText;
        public TextMeshProUGUI enemiesRemainingText;
        
        [Header("Minimap")]
        public RawImage minimapImage;
        public Transform minimapPlayerIcon;
        
        [Header("Crosshair")]
        public GameObject crosshair;
        
        [Header("Damage Indicators")]
        public GameObject damageIndicatorPrefab;
        public Transform damageIndicatorParent;
        
        [Header("Events")]
        // public GameEvent OnHealthUpdated;
        // public GameEvent OnScoreUpdated;
        
        private Health playerHealth;
        private int currentScore = 0;
        private int killCount = 0;
        
        private void Start()
        {
            InitializeHUD();
        }
        
        public void InitializeHUD()
        {
            // Find player health component
            FindPlayerHealth();
            
            // Initialize UI elements
            UpdateHealthDisplay();
            UpdateScore(0);
            UpdateKillCount(0);
            UpdateWaveInfo(1, 0);
        }
        
        private void FindPlayerHealth()
        {
            if (playerHealth == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerHealth = player.GetComponent<Health>();
                }
                else
                {
                    Debug.LogWarning("HUDManager: No player found with 'Player' tag. Health display may not work correctly.");
                }
            }
        }
        
        public void SetPlayerHealth(Health health)
        {
            playerHealth = health;
            UpdateHealthDisplay();
        }
        
        public void UpdateHealthDisplay()
        {
            if (playerHealth == null || healthBar == null) return;
            
            float healthPercentage = playerHealth.HealthPercentage;
            
            // Animate health bar smoothly with DOTween
            healthBar.DOValue(healthPercentage, 0.5f).SetEase(Ease.OutQuart);
            
            // Update health text
            if (healthText != null)
            {
                healthText.text = $"{playerHealth.CurrentHealth:F0}/{playerHealth.MaxHealth:F0}";
            }
            
            // OnHealthUpdated?.Invoke();
        }
        
        public void UpdateScore(int newScore)
        {
            int previousScore = currentScore;
            currentScore = newScore;
            
            if (scoreText != null)
            {
                // Animate score counter with DOTween
                DOTween.To(() => previousScore, x => {
                    scoreText.text = $"Score: {x:N0}";
                }, currentScore, 1f).SetEase(Ease.OutQuart);
            }
            
            // OnScoreUpdated?.Invoke();
        }
        
        public void AddScore(int points)
        {
            UpdateScore(currentScore + points);
        }
        
        public void UpdateKillCount(int newKillCount)
        {
            killCount = newKillCount;
            if (killCountText != null)
            {
                killCountText.text = $"Kills: {killCount}";
            }
        }
        
        public void AddKill()
        {
            UpdateKillCount(killCount + 1);
        }
        
        public void UpdateWaveInfo(int waveNumber, int enemiesRemaining)
        {
            if (waveText != null)
            {
                waveText.text = $"Wave {waveNumber}";
            }
            
            if (enemiesRemainingText != null)
            {
                enemiesRemainingText.text = $"Enemies: {enemiesRemaining}";
            }
        }
        
        public void ShowDamageIndicator(Vector3 worldPosition, float damage)
        {
            if (damageIndicatorPrefab == null || damageIndicatorParent == null) return;
            
            // Convert world position to screen position
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            
            // Create damage indicator
            GameObject indicator = Instantiate(damageIndicatorPrefab, damageIndicatorParent);
            indicator.transform.position = screenPos;
            
            // Setup damage text
            TextMeshProUGUI damageText = indicator.GetComponent<TextMeshProUGUI>();
            if (damageText != null)
            {
                damageText.text = damage.ToString("F0");
            }
            
            // Animate and destroy
            AnimateDamageIndicator(indicator);
        }
        
        private void AnimateDamageIndicator(GameObject indicator)
        {
            // Setup initial values
            Vector3 startPos = indicator.transform.position;
            Vector3 endPos = startPos + Vector3.up * 100f;
            
            CanvasGroup canvasGroup = indicator.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = indicator.AddComponent<CanvasGroup>();
            }
            
            // Animate position and alpha simultaneously with DOTween
            indicator.transform.DOMoveY(endPos.y, 1f)
                .SetEase(Ease.OutQuart);
                
            canvasGroup.DOFade(0f, 1f)
                .SetEase(Ease.OutQuart)
                .OnComplete(() => Destroy(indicator));
        }
        
        public void SetCrosshairVisible(bool visible)
        {
            if (crosshair != null)
            {
                crosshair.SetActive(visible);
                
                if (visible)
                {
                    // Add a subtle pulse animation to the crosshair
                    crosshair.transform.DOScale(1.1f, 1f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
                }
                else
                {
                    // Stop the pulse animation
                    crosshair.transform.DOKill();
                    crosshair.transform.localScale = Vector3.one;
                }
            }
        }
        
        public void ShowReloadIndicator()
        {
            // Implement reload UI feedback
            Debug.Log("Reloading...");
        }
        
        // Call this when player takes damage
        public void OnPlayerDamaged()
        {
            UpdateHealthDisplay();
            
            // Screen flash effect with DOTween
            CreateScreenFlash();
        }
        
        private void CreateScreenFlash()
        {
            // Create temporary overlay for damage flash
            GameObject flashOverlay = new GameObject("DamageFlash");
            flashOverlay.transform.SetParent(transform);
            
            Image flashImage = flashOverlay.AddComponent<Image>();
            flashImage.color = new Color(1f, 0f, 0f, 0.3f);
            flashImage.raycastTarget = false;
            
            RectTransform rect = flashOverlay.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            // Fade out with DOTween
            flashImage.DOFade(0f, 0.5f)
                .OnComplete(() => Destroy(flashOverlay));
        }
    }
}