using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.RecyclingScrollable
{
    [System.Serializable]
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class ScrollElement : MonoBehaviour
    {
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] Image _image;
        [SerializeField] Button _button;
        [SerializeField] TextMeshProUGUI _textMeshProUGUI;

        public delegate void OnClickListener();

        public float Height { get => _rectTransform.rect.height; }
        public float Width { get => _rectTransform.rect.width; }

        public void SetImageSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public void SetSize(float width, float height)
        {
            SetWidth(width);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        public void SetWidth(float width)
        {
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        public void SetLocalXPosition(float x)
        {
            _rectTransform.localPosition = new Vector3(x, _rectTransform.localPosition.y);
        }

        public void SetLocalYPosition(float y)
        {
            _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x, y);
        }

        public void SetText(string text)
        {
            _textMeshProUGUI.text = text;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void RegisterOnClickListener(OnClickListener listener)
        {
            _button.onClick.AddListener(() => listener());
        }
    }
}