using UnityEngine;
using UserInterface.Menu;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MenuUIManager : MonoBehaviour
{
    static MenuUIManager _instance;
    public static MenuUIManager Instance { get { return _instance; } }

    [SerializeField] GameObject _rootObject;
    [SerializeField] Camera _menuCamera;

    public Camera MenuCamera { get { return _menuCamera; } }

    [Header("UIPages")]

    [SerializeField] Page _startPage;
    [SerializeField] GameObject _firstFocusItem;

    readonly Stack<Page> _pageStack = new ();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        if (_firstFocusItem != null)
        {
            EventSystem.current.SetSelectedGameObject(_firstFocusItem);
        }

        if (_startPage != null)
        {
            PushPage(_startPage);
        }
    }

    void OnCancel()
    {
        if (_rootObject.activeSelf && _rootObject.activeInHierarchy && _pageStack.Count != 0)
        {
            PopPage();
        }
    }

    public bool IsPageInStack(Page page)
    {
        return _pageStack.Contains(page);
    }

    public bool IsPageOnTopOfStack(Page page)
    {
        return _pageStack.Count > 0 && page == _pageStack.Peek();
    }

    public void PushPage(Page page)
    {
        if (!_menuCamera.enabled) { _menuCamera.enabled = true; }

        page.Enter();

        if (_pageStack.Count > 0)
        {
            Page currentPage = _pageStack.Peek();

            if (currentPage.ExitOnNewPagePush)
            {
                currentPage.Exit();
            }
        }

        _pageStack.Push(page);
    }

    public void PopPage()
    {
        if (_pageStack.Count < 1)
        {
            Debug.Log("Trying to pop a UI Page, but none remain in the stack!");
            return;
        }
        
        Page page = _pageStack.Pop();
        page.Exit();

        if (_pageStack.Count == 0)
        {
            _menuCamera.enabled = false;
            return;
        }

        Page newPage = _pageStack.Peek();

        if (newPage.ExitOnNewPagePush)
        {
            newPage.Enter();
        }
    }

    public void PopAllPages()
    {
        for (int i = 0; i < _pageStack.Count; i++)
        {
            PopPage();
        }
    }

    public void SetMenuCamera(Camera camera)
    {
        _menuCamera = camera;
    }
}
