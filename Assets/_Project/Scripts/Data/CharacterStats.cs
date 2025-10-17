using UnityEngine;

namespace ZombieWar.Data
{
    [CreateAssetMenu(fileName = "New Character Stats", menuName = "ZombieWar/Data/Character Stats")]
    public class CharacterStats : ScriptableObject
    {
        [Header("Health")]
        public float maxHealth = 100f;
        
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 180f;
        
        [Header("Combat")]
        public float attackDamage = 25f;
        public float attackRange = 2f;
        public float attackCooldown = 1f;
        
        [Header("Experience")]
        public int maxLevel = 10;
        public AnimationCurve experienceCurve;
        
        public float GetExperienceForLevel(int level)
        {
            if (experienceCurve != null && experienceCurve.length > 0)
            {
                return experienceCurve.Evaluate(level);
            }
            return level * 100f; // Default fallback
        }
    }
}