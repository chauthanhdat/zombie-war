using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieWar
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        void TakeDamage(float damageAmount);
        void Heal(float healAmount);
        event System.Action<float, float> OnHealthChanged; // currentHealth, maxHealth
        event System.Action OnDeath;
    }
}
