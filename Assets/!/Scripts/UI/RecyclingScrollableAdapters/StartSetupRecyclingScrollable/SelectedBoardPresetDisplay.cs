using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace CustomUI
{
    public class SelectedBoardPresetDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _titleText;
        [SerializeField] TextMeshProUGUI _descriptionText;
        [SerializeField] Image _presetThumbnail;

        public void SetDisplayedText(PresetDisplayInfo displayInfo)
        {
            if (displayInfo == null) { throw new NullReferenceException(displayInfo.ToString()); }

            _titleText.text = displayInfo.TitleText;
            _presetThumbnail.sprite = displayInfo.Thumbnail;

            if (displayInfo.DescriptionText != null)
            {
                _descriptionText.text = displayInfo.DescriptionText;
            }
        }
    }
}