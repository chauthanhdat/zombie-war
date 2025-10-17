using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZombieWar
{
    public class TouchFeedbackButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private AudioClip clickSound;

        private Vector3 originalScale;

        private void Start()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.DOScale(originalScale * 0.9f, 0.1f);

            AudioManager.Instance.PlaySFX(clickSound);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.DOScale(originalScale, 0.1f);
        }
    }
}
