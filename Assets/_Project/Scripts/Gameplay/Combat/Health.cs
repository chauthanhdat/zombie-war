using UnityEngine;
using ZombieWar.Data;
using ZombieWar.Core.Events;

namespace ZombieWar.Gameplay.Combat
{
    public class Health : MonoBehaviour
    {
        [Header("Configuration")]
        public CharacterStats characterStats;
        
        [Header("Events")]
        // public GameEvent OnDeath;
        // public GameEvent OnHealthChanged;
        
        [Header("Runtime Values")]
        [SerializeField] private float currentHealth;
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => characterStats ? characterStats.maxHealth : 100f;
        public bool IsDead => currentHealth <= 0f;
        public float HealthPercentage => currentHealth / MaxHealth;
        
        private void Awake()
        {
            InitializeHealth();
        }
        
        private void InitializeHealth()
        {
            currentHealth = MaxHealth;
        }
        
        public void TakeDamage(float damage)
        {
            if (IsDead) return;
            
            currentHealth = Mathf.Max(0f, currentHealth - damage);
            // OnHealthChanged?.Invoke();
            
            if (IsDead)
            {
                Die();
            }
        }
        
        public void Heal(float amount)
        {
            if (IsDead) return;
            
            currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
            // OnHealthChanged?.Invoke();
        }
        
        public void SetHealth(float amount)
        {
            currentHealth = Mathf.Clamp(amount, 0f, MaxHealth);
            // OnHealthChanged?.Invoke();
            
            if (IsDead)
            {
                Die();
            }
        }
        
        private void Die()
        {
            // OnDeath?.Invoke();
            Debug.Log($"{gameObject.name} has died");
        }
        
        public void ResetHealth()
        {
            InitializeHealth();
            // OnHealthChanged?.Invoke();
        }
    }
}