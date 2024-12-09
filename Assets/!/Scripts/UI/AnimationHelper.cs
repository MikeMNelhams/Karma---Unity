using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UserInterface.Animations
{
    public class AnimationHelper
    {
        public static IEnumerator ZoomIn(RectTransform Transform, float Speed, UnityEvent OnEnd)
        {
            float time = 0;
            while (time < 1)
            {
                Transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
                yield return null;
                time += Time.deltaTime * Speed;
            }

            Transform.localScale = Vector3.one;

            OnEnd?.Invoke();
        }

        public static IEnumerator ZoomOut(RectTransform Transform, float Speed, UnityEvent OnEnd)
        {
            float time = 0;
            while (time < 1)
            {
                Transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time);
                yield return null;
                time += Time.deltaTime * Speed;
            }

            Transform.localScale = Vector3.zero;
            OnEnd?.Invoke();
        }

        public static IEnumerator FadeIn(CanvasGroup canvasGroup, float Speed, UnityEvent OnEnd)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            float time = 0;
            while (time < 1)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, time);
                yield return null;
                time += Time.deltaTime * Speed;
            }

            canvasGroup.alpha = 1;
            OnEnd?.Invoke();
        }

        public static IEnumerator FadeOut(CanvasGroup CanvasGroup, float Speed, UnityEvent OnEnd)
        {
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;

            float time = 0;
            while (time < 1)
            {
                CanvasGroup.alpha = Mathf.Lerp(1, 0, time);
                yield return null;
                time += Time.deltaTime * Speed;
            }

            CanvasGroup.alpha = 0;
            OnEnd?.Invoke();
        }

        public static IEnumerator SlideIn(RectTransform Transform, AnimationDirection direction, float Speed, UnityEvent OnEnd)
        {
            Vector2 startPosition = direction switch
            {
                AnimationDirection.Up => new Vector2(0, -Screen.height),
                AnimationDirection.Right => new Vector2(-Screen.width, 0),
                AnimationDirection.Down => new Vector2(0, Screen.height),
                AnimationDirection.Left => new Vector2(Screen.width, 0),
                _ => new Vector2(0, -Screen.height)
            };

            float time = 0;
            while (time < 1)
            {
                Transform.anchoredPosition = Vector2.Lerp(startPosition, Vector2.zero, time);
                yield return null;
                time += Time.deltaTime * Speed;
            }

            Transform.anchoredPosition = Vector2.zero;
            OnEnd?.Invoke();
        }

        public static IEnumerator SlideOut(RectTransform Transform, AnimationDirection direction, float Speed, UnityEvent OnEnd)
        {
            Vector2 endPosition = 2 * direction switch
            {
                AnimationDirection.Up => new Vector2(0, Screen.height),
                AnimationDirection.Right => new Vector2(Screen.width, 0),
                AnimationDirection.Down => new Vector2(0, -Screen.height),
                AnimationDirection.Left => new Vector2(-Screen.width, 0),
                _ => new Vector2(0, Screen.height)
            };
            UnityEngine.Debug.Log("exit direction: " + direction);
            float time = 0;
            while (time < 1)
            {
                Transform.anchoredPosition = Vector2.Lerp(Vector2.zero, endPosition, time);
                yield return null;
                time += Time.deltaTime * Speed;
            }

            Transform.anchoredPosition = endPosition;
            OnEnd?.Invoke();
        }
    }
}