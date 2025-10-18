using System.Collections.Generic;
using UnityEngine;

namespace ZombieWar.Gameplay
{
    public class HealthBarManager : MonoBehaviour
    {
        public static HealthBarManager Instance { get; private set; }

        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private GameObject bossHealthBarPrefab;
        [SerializeField] private int poolSize = 10;
        [SerializeField] private Transform healthBarParent;

        private Queue<EnemyHealthBarUI> healthBarPool = new();
        private Dictionary<IDamageable, EnemyHealthBarUI> activeHealthBars = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializePool();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                AddNewHealthBarToPool();
            }
        }

        private void AddNewHealthBarToPool()
        {
            GameObject healthBarObj = Instantiate(healthBarPrefab, healthBarParent);
            EnemyHealthBarUI healthBarUI = healthBarObj.GetComponent<EnemyHealthBarUI>();
            healthBarUI.gameObject.SetActive(false);
            healthBarPool.Enqueue(healthBarUI);
        }

        public void RequestHealthBar(IDamageable enemyHealth, Transform enemyHealthBarAnchor)
        {
            if (activeHealthBars.ContainsKey(enemyHealth)) return;

            if (healthBarPool.Count == 0)
            {
                AddNewHealthBarToPool();
            }

            EnemyHealthBarUI healthBar = healthBarPool.Dequeue();
            healthBar.gameObject.SetActive(true);
            healthBar.Setup(enemyHealth, enemyHealthBarAnchor, ReturnNormalHealthBarToPool);
            activeHealthBars.Add(enemyHealth, healthBar);
        }
        
        public void ReturnNormalHealthBarToPool(IDamageable enemyHealth, EnemyHealthBarUI healthBarUI)
        {
            if (activeHealthBars.ContainsKey(enemyHealth))
            {
                activeHealthBars.Remove(enemyHealth);
            }

            healthBarUI.gameObject.SetActive(false);
            healthBarPool.Enqueue(healthBarUI);
        }
    }
}
