using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1.0f;
        }

        public IEnumerator FadeIn(float time)
        {
            while (canvasGroup.alpha > 0.0f) {
                float alphaIncrease = Time.deltaTime / time;
                canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha - alphaIncrease);

                yield return null;
            }
        }

        public IEnumerator FadeOut(float time)
        {
            while (canvasGroup.alpha < 1.0f) {
                float alphaIncrease = Time.deltaTime / time;
                canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha + alphaIncrease);

                yield return null;
            }
        }
    }
}
