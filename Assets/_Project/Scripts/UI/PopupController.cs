using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

namespace ZombieWar.UI
{
    public class PopupController : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI messageText;
        public Button confirmButton;
        public Button cancelButton;
        public Button closeButton;
        
        [Header("Animation")]
        public CanvasGroup canvasGroup;
        public float fadeInDuration = 0.3f;
        
        private Action onConfirmCallback;
        private Action onCancelCallback;
        
        private void Awake()
        {
            // Setup button listeners
            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirmClicked);
            
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelClicked);
            
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);
        }
        
        public void Setup(string title, string message, Action onConfirm = null, Action onCancel = null)
        {
            // Set text content
            if (titleText != null)
                titleText.text = title;
            
            if (messageText != null)
                messageText.text = message;
            
            // Store callbacks
            onConfirmCallback = onConfirm;
            onCancelCallback = onCancel;
            
            // Configure buttons based on callbacks
            ConfigureButtons();
            
            // Animate in
            AnimateIn();
        }
        
        private void ConfigureButtons()
        {
            // Show/hide buttons based on available callbacks
            if (confirmButton != null)
                confirmButton.gameObject.SetActive(onConfirmCallback != null);
            
            if (cancelButton != null)
                cancelButton.gameObject.SetActive(onCancelCallback != null);
        }
        
        private void AnimateIn()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1f, fadeInDuration)
                    .SetEase(Ease.OutQuart)
                    .SetUpdate(true); // Ignore timescale for UI
            }
        }
        
        private void AnimateOut(Action onComplete = null)
        {
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, fadeInDuration * 0.5f)
                    .SetEase(Ease.InQuart)
                    .SetUpdate(true)
                    .OnComplete(() => {
                        onComplete?.Invoke();
                        Destroy(gameObject);
                    });
            }
            else
            {
                onComplete?.Invoke();
                Destroy(gameObject);
            }
        }
        
        private void OnConfirmClicked()
        {
            AnimateOut(() => {
                onConfirmCallback?.Invoke();
            });
        }
        
        private void OnCancelClicked()
        {
            AnimateOut(() => {
                onCancelCallback?.Invoke();
            });
        }
        
        private void OnCloseClicked()
        {
            AnimateOut();
        }
        
        private void OnDestroy()
        {
            // Kill any DOTween animations on this object
            transform.DOKill();
            if (canvasGroup != null) 
                canvasGroup.DOKill();
                
            // Clean up button listeners
            if (confirmButton != null)
                confirmButton.onClick.RemoveAllListeners();
            
            if (cancelButton != null)
                cancelButton.onClick.RemoveAllListeners();
            
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();
        }
    }
}