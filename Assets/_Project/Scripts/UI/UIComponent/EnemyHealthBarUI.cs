using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieWar
{
    public class EnemyHealthBarUI : MonoBehaviour
    {
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider healthBarDelay;

        private IDamageable targetHealth;
        private Transform targetAnchor;
        private Camera mainCamera;

        private System.Action<IDamageable, EnemyHealthBarUI> returnAction;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public void Setup(IDamageable enemyHealth, Transform anchor, System.Action<IDamageable, EnemyHealthBarUI> returnToPoolAction)
        {
            targetHealth = enemyHealth;
            targetAnchor = anchor;
            returnAction = returnToPoolAction;

            healthBar.value = 1;
            healthBarDelay.value = 1;

            targetHealth.OnHealthChanged += UpdateHealthBar;
            targetHealth.OnDeath += OnTargetDeath;
        }

        private void Update()
        {
            if (targetAnchor != null)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(targetAnchor.position);
                transform.position = screenPos;
            }
            else
            {
                returnAction?.Invoke(targetHealth, this);
            }
        }

        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            healthBar.DOValue(currentHealth / maxHealth, 0.1f).SetEase(Ease.OutQuart);
            healthBarDelay.DOValue(currentHealth / maxHealth, 0.5f).SetEase(Ease.OutQuart);
        }

        private void OnTargetDeath()
        {
            targetHealth.OnHealthChanged -= UpdateHealthBar;
            targetHealth.OnDeath -= OnTargetDeath;

            Invoke(nameof(DelayReturn), 1f);
        }

        private void DelayReturn()
        {
            returnAction?.Invoke(targetHealth, this);
        }
    }
}
