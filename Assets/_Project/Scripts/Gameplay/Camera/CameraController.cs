using UnityEngine;

namespace ZombieWar.Gameplay.Camera
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;
        
        [Header("Camera Settings")]
        public Vector3 offset = new Vector3(0f, 5f, -5f);
        public float smoothSpeed = 0.125f;
        public bool lookAtTarget = true;
        
        private Vector3 currentOffset;
        private float currentZoom;
        
        private void Start()
        {
            currentOffset = offset;
            currentZoom = offset.magnitude;
        }
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            UpdateCameraPosition();
            UpdateCameraRotation();
        }
        
        private void UpdateCameraPosition()
        {
            Vector3 desiredPosition;
            desiredPosition = target.position + currentOffset;
            
            // Smooth camera movement
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        
        private void UpdateCameraRotation()
        {
            if (lookAtTarget)
            {
                transform.LookAt(target);
            }
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
            currentOffset = newOffset;
        }
        
        // Useful for cutscenes or special camera movements
        public void SetTemporaryTarget(Transform temporaryTarget, float duration)
        {
            StartCoroutine(TemporaryTargetCoroutine(temporaryTarget, duration));
        }
        
        private System.Collections.IEnumerator TemporaryTargetCoroutine(Transform temporaryTarget, float duration)
        {
            Transform originalTarget = target;
            target = temporaryTarget;
            
            yield return new WaitForSeconds(duration);
            
            target = originalTarget;
        }
    }
}