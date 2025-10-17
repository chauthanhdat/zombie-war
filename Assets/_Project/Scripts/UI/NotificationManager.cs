using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace ZombieWar.UI
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }
        
        [Header("Notification Settings")]
        public GameObject notificationPrefab;
        public Transform notificationContainer;
        public float notificationDuration = 3f;
        public int maxNotifications = 5;
        
        [Header("Notification Types")]
        public Color infoColor = Color.white;
        public Color successColor = Color.green;
        public Color warningColor = Color.yellow;
        public Color errorColor = Color.red;
        
        private Queue<GameObject> activeNotifications = new Queue<GameObject>();
        
        public enum NotificationType
        {
            Info,
            Success,
            Warning,
            Error
        }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void ShowNotification(string message, NotificationType type = NotificationType.Info)
        {
            if (notificationPrefab == null || notificationContainer == null)
            {
                Debug.LogWarning("Notification system not properly configured!");
                return;
            }
            
            // Remove oldest notification if at max capacity
            if (activeNotifications.Count >= maxNotifications)
            {
                RemoveOldestNotification();
            }
            
            // Create new notification
            GameObject notification = Instantiate(notificationPrefab, notificationContainer);
            ConfigureNotification(notification, message, type);
            
            activeNotifications.Enqueue(notification);
            
            // Auto-remove after duration
            StartCoroutine(RemoveNotificationAfterDelay(notification, notificationDuration));
        }
        
        private void ConfigureNotification(GameObject notification, string message, NotificationType type)
        {
            // Set message text
            TextMeshProUGUI messageText = notification.GetComponentInChildren<TextMeshProUGUI>();
            if (messageText != null)
            {
                messageText.text = message;
            }
            
            // Set background color based on type
            Image background = notification.GetComponent<Image>();
            if (background != null)
            {
                Color backgroundColor = type switch
                {
                    NotificationType.Success => successColor,
                    NotificationType.Warning => warningColor,
                    NotificationType.Error => errorColor,
                    _ => infoColor
                };
                
                backgroundColor.a = 0.8f; // Semi-transparent
                background.color = backgroundColor;
            }
            
            // Add close button functionality
            Button closeButton = notification.GetComponentInChildren<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(() => RemoveNotification(notification));
            }
            
            // Animate in
            AnimateNotificationIn(notification);
        }
        
        private void AnimateNotificationIn(GameObject notification)
        {
            CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = notification.AddComponent<CanvasGroup>();
            }
            
            RectTransform rect = notification.GetComponent<RectTransform>();
            Vector3 startPos = rect.anchoredPosition;
            Vector3 targetPos = startPos;
            startPos.x += 300f;
            rect.anchoredPosition = startPos;
            canvasGroup.alpha = 0f;
            
            // Animate position and alpha with DOTween
            rect.DOAnchorPosX(targetPos.x, 0.3f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
                
            canvasGroup.DOFade(1f, 0.3f)
                .SetUpdate(true);
        }
        
        private void AnimateNotificationOut(GameObject notification, System.Action onComplete = null)
        {
            CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
            RectTransform rect = notification.GetComponent<RectTransform>();
            
            if (canvasGroup != null && rect != null)
            {
                Vector3 endPos = rect.anchoredPosition;
                endPos.x += 300f;
                
                // Create a sequence for smooth animation with DOTween
                Sequence sequence = DOTween.Sequence();
                
                sequence.Append(rect.DOAnchorPosX(endPos.x, 0.3f)
                    .SetEase(Ease.InBack))
                    .Join(canvasGroup.DOFade(0f, 0.3f))
                    .SetUpdate(true)
                    .OnComplete(() => {
                        onComplete?.Invoke();
                        if (notification != null)
                            Destroy(notification);
                    });
            }
            else
            {
                onComplete?.Invoke();
                if (notification != null)
                    Destroy(notification);
            }
        }
        
        private System.Collections.IEnumerator RemoveNotificationAfterDelay(GameObject notification, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (notification != null)
            {
                RemoveNotification(notification);
            }
        }
        
        private void RemoveNotification(GameObject notification)
        {
            if (activeNotifications.Contains(notification))
            {
                // Create a new queue without this notification
                Queue<GameObject> newQueue = new Queue<GameObject>();
                while (activeNotifications.Count > 0)
                {
                    GameObject item = activeNotifications.Dequeue();
                    if (item != notification)
                    {
                        newQueue.Enqueue(item);
                    }
                }
                activeNotifications = newQueue;
            }
            
            AnimateNotificationOut(notification);
        }
        
        private void RemoveOldestNotification()
        {
            if (activeNotifications.Count > 0)
            {
                GameObject oldest = activeNotifications.Dequeue();
                AnimateNotificationOut(oldest);
            }
        }
        
        public void ClearAllNotifications()
        {
            // Kill all DOTween animations
            DOTween.Kill(this);
            
            while (activeNotifications.Count > 0)
            {
                GameObject notification = activeNotifications.Dequeue();
                if (notification != null)
                {
                    // Kill animations on the notification object
                    notification.transform.DOKill();
                    CanvasGroup cg = notification.GetComponent<CanvasGroup>();
                    if (cg != null) cg.DOKill();
                    
                    Destroy(notification);
                }
            }
        }
        
        // Convenience methods for different notification types
        public void ShowInfo(string message)
        {
            ShowNotification(message, NotificationType.Info);
        }
        
        public void ShowSuccess(string message)
        {
            ShowNotification(message, NotificationType.Success);
        }
        
        public void ShowWarning(string message)
        {
            ShowNotification(message, NotificationType.Warning);
        }
        
        public void ShowError(string message)
        {
            ShowNotification(message, NotificationType.Error);
        }
        
        // Game-specific notifications
        public void ShowWaveComplete(int waveNumber)
        {
            ShowSuccess($"Wave {waveNumber} Complete!");
        }
        
        public void ShowNewWave(int waveNumber)
        {
            ShowInfo($"Wave {waveNumber} Starting...");
        }
        
        public void ShowLowHealth()
        {
            ShowWarning("Health Critical!");
        }
        
        public void ShowLowAmmo()
        {
            ShowWarning("Low Ammo!");
        }
        
        public void ShowWeaponPickup(string weaponName)
        {
            ShowSuccess($"Picked up {weaponName}!");
        }
        
        public void ShowObjectiveComplete(string objective)
        {
            ShowSuccess($"Objective Complete: {objective}");
        }
    }
}