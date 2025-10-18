using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ZombieWar
{
    public class LevelEndUI : MonoBehaviour
    {
        [SerializeField] private Transform titleTransform;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private Button continueButton;

        public System.Action OnClickContinue;

        private void OnEnable()
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);

            continueButton.gameObject.SetActive(false);
            DOVirtual.DelayedCall(2f, () =>
            {
                continueButton.gameObject.SetActive(true);
                continueButton.transform.DOScale(Vector3.one * 2, 0.3f).From(Vector3.one * 2.2f).SetEase(Ease.OutBack);
            });
        }

        private void OnDisable()
        {
            continueButton.onClick.RemoveListener(OnContinueButtonClicked);
        }

        public void Show(string title, string subtitle)
        {
            titleText.text = title;
            subtitleText.text = subtitle;

            titleTransform.gameObject.SetActive(true);
        }

        public void OnContinueButtonClicked()
        {
            OnClickContinue?.Invoke();
            gameObject.SetActive(false);

            SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
