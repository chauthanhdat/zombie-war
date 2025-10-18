using UnityEngine;
using ZombieWar.Data;
using ZombieWar.Core.Events;

namespace ZombieWar.Gameplay.Combat
{
    public class CharacterHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private Animator animator;

        [SerializeField] private float maxHealth = 100f;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip hurtSound;
        [SerializeField] private AudioClip deathSound;

        private float currentHealth;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => currentHealth <= 0f;
        public float HealthPercentage => currentHealth / MaxHealth;

        public event System.Action<float, float> OnHealthChanged;
        public event System.Action OnDeath;
        
        private void Awake()
        {
            InitializeHealth();
        }
        
        private void InitializeHealth()
        {
            currentHealth = MaxHealth;
        }
        
        public void TakeDamage(float damageAmount)
        {
            if (IsDead) return;
            
            currentHealth = Mathf.Max(0f, currentHealth - damageAmount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (IsDead)
            {
                Die();
                AudioManager.Instance.PlaySFX(deathSound);
                animator.SetTrigger("Die");
                // animator.Play("Die", 1, 0f);
            }
            else
            {
                AudioManager.Instance.PlaySFX(hurtSound);
                animator.Play("Hurt", 1, 0f);
            }
        }
        
        public void Heal(float amount)
        {
            if (IsDead) return;
            
            currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        private void Die()
        {
            OnDeath?.Invoke();
            GameEvent.OnDeath?.Invoke();
        }
    }
}