using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieWar.Gameplay;

namespace ZombieWar.Gameplay.Combat
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private Transform healthBarAnchor;

        private float maxHealth = 100f;
        private float currentHealth;

        public float CurrentHealth => currentHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        private void Start()
        {
            currentHealth = maxHealth;

            HealthBarManager.Instance.RequestHealthBar(this, healthBarAnchor);
        }

        public void TakeDamage(float damageAmount)
        {
            if (currentHealth <= 0f) return;

            currentHealth = Mathf.Max(0f, currentHealth - damageAmount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0f)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(float healAmount)
        {
            throw new NotImplementedException();
        }
    }
}
