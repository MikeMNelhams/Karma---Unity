using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PresetDisplayInfo
{
    [SerializeField] string _titleText;
    [SerializeField] string _descriptionText;
    [SerializeField] Sprite _thumbnail;

    public string TitleText { get => _titleText; }
    public string DescriptionText { get => _descriptionText; }
    public Sprite Thumbnail { get => _thumbnail; }

    public PresetDisplayInfo(string titleText)
    {
        _titleText = titleText;
    }

    public PresetDisplayInfo(string titleText, string descriptionText, Sprite thumbnail)
    {
        _titleText = titleText;
        _descriptionText = descriptionText;
        _thumbnail = thumbnail;
    }

    public override string ToString()
    {
        return _titleText;
    }
}
