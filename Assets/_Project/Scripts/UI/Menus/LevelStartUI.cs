using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ZombieWar
{
    public class LevelStartUI : MonoBehaviour
    {
        [SerializeField] private Transform titleTransform;
        [SerializeField] private TextMeshProUGUI titleText;

        public IEnumerator Show(string title)
        {
            titleText.text = title;
            titleTransform.localScale = Vector3.one;
            gameObject.SetActive(true);

            // TODO: get canvas height
            titleTransform.localPosition = new Vector3(0, 1200, 0);

            var sequence = DOTween.Sequence()
                .Append(titleTransform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack))
                .Append(titleTransform.DOLocalMoveY(-1200, 0.6f).SetEase(Ease.InBack));

            yield return sequence.WaitForCompletion();

            gameObject.SetActive(false);
        }
    }
}
