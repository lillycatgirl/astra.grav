using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TransitionManager : MonoBehaviour
    {
        public Image transition;
        private bool _transitioning;

        public static TransitionManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Application.targetFrameRate = 60;
            } else {
                Destroy(gameObject);
            }
            transition.raycastTarget = false;
            transition.color = new Color(1, 1, 1, 0);
            _transitioning = false;
        }

        public void TransitionInOut(int pwr = 8)
        {
            if(!_transitioning)
                StartCoroutine(FadeIn(pwr));
        }
        public void TransitionOut(int pwr = 8)
        {
            if(!_transitioning)
                StartCoroutine(FadeOut(pwr));
        }
        public void TransitionIn(int pwr = 8)
        {
            if(!_transitioning)
                StartCoroutine(FadeIn(pwr, false));
        }

        private IEnumerator FadeIn(int pwr, bool fadeOut = true)
        {
            _transitioning = true;
            transition.raycastTarget = true;
            while (transition.color.a < 0.99f)
            {
                float alpha = transition.color.a;
                float a = Mathf.Lerp(alpha, 1, 1f - Mathf.Exp(-pwr * Time.deltaTime));
                transition.color = new Color(1, 1, 1, a);
                yield return new WaitForEndOfFrame();
            }
            transition.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.6f);
            if (fadeOut)
                yield return StartCoroutine(FadeOut(pwr));
            else
                _transitioning = false;
        }
        private IEnumerator FadeOut(int pwr)
        {
            transition.color = new Color(1, 1, 1, 1);
            transition.raycastTarget = true;
            while (transition.color.a > 0.01f)
            {
                float alpha = transition.color.a;
                float a = Mathf.Lerp(alpha, 0, 1f - Mathf.Exp(-pwr * Time.deltaTime));
                transition.color = new Color(1, 1, 1, a);
                yield return new WaitForEndOfFrame();
            }
            transition.raycastTarget = false;
            transition.color = new Color(1, 1, 1, 0);
            _transitioning = false;
        }
    }
}