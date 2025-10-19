using UnityEngine;

namespace ZombieWar.Gameplay.Player
{
    public class TargetingSystem : MonoBehaviour
    {
        [Header("Targeting Settings")]
        public float detectionRadius = 50f;
        public LayerMask enemyLayers;

        [Header("Indicatior")]
        public Transform indicator;
        public float indicatorRotateSpeed = 5f;
        
        private Transform currentTarget;

        public Transform CurrentTarget => currentTarget;

        private void Update()
        {
            FindNearestTarget();

            if (currentTarget != null)
            {
                RotateIndicatorTowardsTarget(currentTarget);
            }
            else
            {
                indicator.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = currentTarget == null ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

        private void FindNearestTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayers);
            float closestDistance = float.MaxValue;
            Transform nearestTarget = null;

            foreach (var hit in hits)
            {
                var damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable == null || damageable.CurrentHealth <= 0) continue;

                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestTarget = hit.transform;
                }
            }

            currentTarget = nearestTarget;
        }

        private void RotateIndicatorTowardsTarget(Transform target)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            if (direction == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            indicator.rotation = targetRotation;
        }
    }
}
