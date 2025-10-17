using UnityEngine;

namespace ZombieWar.Utilities
{
    public static class GameUtilities
    {
        /// <summary>
        /// Finds the closest GameObject from an array of GameObjects
        /// </summary>
        public static GameObject FindClosest(Vector3 position, GameObject[] objects)
        {
            GameObject closest = null;
            float closestDistance = Mathf.Infinity;
            
            foreach (GameObject obj in objects)
            {
                if (obj == null) continue;
                
                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = obj;
                }
            }
            
            return closest;
        }
        
        /// <summary>
        /// Check if a position is within a certain range of another position
        /// </summary>
        public static bool IsInRange(Vector3 position1, Vector3 position2, float range)
        {
            return Vector3.Distance(position1, position2) <= range;
        }
        
        /// <summary>
        /// Get a random point within a circle
        /// </summary>
        public static Vector3 GetRandomPointInCircle(Vector3 center, float radius)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            return center + new Vector3(randomCircle.x, 0f, randomCircle.y);
        }
        
        /// <summary>
        /// Smoothly damp an angle
        /// </summary>
        public static float DampAngle(float current, float target, ref float velocity, float smoothTime)
        {
            return Mathf.SmoothDampAngle(current, target, ref velocity, smoothTime);
        }
        
        /// <summary>
        /// Check if a layer is in a LayerMask
        /// </summary>
        public static bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return (layerMask.value & (1 << layer)) != 0;
        }
    }
}