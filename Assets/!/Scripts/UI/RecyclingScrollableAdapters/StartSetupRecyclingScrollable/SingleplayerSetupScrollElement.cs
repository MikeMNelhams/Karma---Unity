using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.RecyclingScrollable.SingleplayerSetup
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class SingleplayerSetupScrollElement : ScrollElement
    {
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] Button _button;
        [SerializeField] TextMeshProUGUI _textMeshProUGUI;

        protected override RectTransform RectTransform { get => _rectTransform; }

        public void SetText(string text)
        {
            _textMeshProUGUI.text = text;
        } 

        public void RegisterOnClickListener(OnClickListener listener)
        {
            _button.onClick.AddListener(() => listener());
        }
    }
}