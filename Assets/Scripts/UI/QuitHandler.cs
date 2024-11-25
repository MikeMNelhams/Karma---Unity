using UnityEditor;
using UnityEngine;


namespace UserInterface.Menu
{
    public class QuitScript : MonoBehaviour
    {
        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}