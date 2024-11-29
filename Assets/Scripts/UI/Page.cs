using UserInterface.Animations;
using UnityEngine;

namespace UserInterface.Menu
{
    [RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
    public class Page : MonoBehaviour
    {
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] CanvasGroup _canvasGroup;

        [Header("Animation settings")]

        [SerializeField] bool _blocksGameInput = false;
        [SerializeField] float _animationSpeed = 1f;
        public bool ExitOnNewPagePush = false;

        [SerializeField] AnimationMode _entryMode = AnimationMode.Slide;
        [SerializeField] AnimationDirection _entryDirection = AnimationDirection.Left;

        [SerializeField] AnimationMode _exitMode = AnimationMode.Slide;
        [SerializeField] AnimationDirection _exitDirection = AnimationDirection.Left;


        Coroutine _animationCoroutine;

        public bool BlocksGameInput { get => _blocksGameInput; }

        public void Enter()
        {
            switch (_entryMode)
            {
                case AnimationMode.Slide:
                    SlideIn();
                    break;
                case AnimationMode.Zoom:
                    ZoomIn();
                    break;
                case AnimationMode.Fade:
                    FadeIn();
                    break;
            }
        }

        public void Exit()
        {
            switch (_exitMode)
            {
                case AnimationMode.Slide:
                    SlideOut();
                    break;
                case AnimationMode.Zoom:
                    ZoomOut();
                    break;
                case AnimationMode.Fade:
                    FadeOut();
                    break;
            }
        }

        void SlideIn()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(AnimationHelper.SlideIn(_rectTransform, _entryDirection, _animationSpeed, null));
        }

        void SlideOut()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(AnimationHelper.SlideOut(_rectTransform, _exitDirection, _animationSpeed, null));
        }

        void ZoomIn()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(AnimationHelper.ZoomIn(_rectTransform, _animationSpeed, null));
        }

        void ZoomOut()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(AnimationHelper.ZoomIn(_rectTransform, _animationSpeed, null));
        }

        void FadeIn()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(AnimationHelper.FadeIn(_canvasGroup, _animationSpeed, null));
        }

        void FadeOut()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(AnimationHelper.FadeIn(_canvasGroup, _animationSpeed, null));
        }
    }
}

