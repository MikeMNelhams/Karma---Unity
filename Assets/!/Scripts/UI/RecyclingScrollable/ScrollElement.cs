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

        public void SetImageSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public void SetSize(float width, float height)
        {
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }
}