using UserInterface.Animations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UserInterface.Menu
{
    [RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
    public class Page : MonoBehaviour
    {
        [SerializeField] GraphicRaycaster _graphicRaycaster;
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] CanvasGroup _canvasGroup;

        [Header("Animation settings")]

        [SerializeField] bool _blocksGameInput = false;
        [SerializeField] float _animationSpeed = 1f;
        [SerializeField] bool _disableRaycastsOnPush = false;
        [SerializeField] bool _visuallyExitsOnPush = false;

        [SerializeField] AnimationMode _entryMode = AnimationMode.Slide;
        [SerializeField] AnimationDirection _entryDirection = AnimationDirection.Left;

        [SerializeField] AnimationMode _exitMode = AnimationMode.Slide;
        [SerializeField] AnimationDirection _exitDirection = AnimationDirection.Left;

        Coroutine _animationCoroutine;

        public bool BlocksGameInput { get => _blocksGameInput; }
        public bool DisablesRaycastsOnPush { get => _disableRaycastsOnPush; }
        public bool VisuallyExitsOnPush { get => _visuallyExitsOnPush; }

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
            _graphicRaycaster.enabled = true;
        }

        public void Exit()
        {
            DisableGraphicsRaycaster();

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

        public void DisableGraphicsRaycaster()
        {
            _graphicRaycaster.enabled = false;
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

            UnityEvent onEnd = new();
            onEnd.AddListener(DisableSelf);

            _animationCoroutine = StartCoroutine(AnimationHelper.SlideOut(_rectTransform, _exitDirection, _animationSpeed, onEnd));
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

            UnityEvent onEnd = new();
            onEnd.AddListener(DisableSelf);

            _animationCoroutine = StartCoroutine(AnimationHelper.ZoomOut(_rectTransform, _animationSpeed, onEnd));
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

            UnityEvent onEnd = new ();
            onEnd.AddListener(DisableSelf);

            _animationCoroutine = StartCoroutine(AnimationHelper.FadeOut(_canvasGroup, _animationSpeed, onEnd));
        }

        void DisableSelf()
        {
            if (gameObject.activeSelf) { gameObject.SetActive(false); }
        }
    }
}

