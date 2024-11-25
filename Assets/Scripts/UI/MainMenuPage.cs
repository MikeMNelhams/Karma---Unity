using UnityEngine;
using UnityEngine.UI;
using UserInterface.Menu;

[RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
public class MainMenuPage : Page
{
    [SerializeField] Button _singleplayerDebugButton;

    void Awake()
    {
#if UNITY_EDITOR
        _singleplayerDebugButton.enabled = true;
# else
        _singleplayerDebugButton.enabled = false;
#endif
    }
}
