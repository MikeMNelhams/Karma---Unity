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
    [SerializeField] EventSystem _eventSystem;

    public Camera MenuCamera { get { return _menuCamera; } }

    [Header("UIPages")]

    [SerializeField] Page _startPage;
    [SerializeField] GameObject _firstFocusItem;

    [SerializeField] Page _settingsPage;

    readonly Stack<Page> _pageStack = new ();
    public delegate void OnBlockGameInputListener();

    OnBlockGameInputListener OnBlockGameInput;

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
            _startPage.enabled = true;
            PushPage(_startPage);
        }
    }

    public void OnCancel()
    {
        if (!_rootObject.activeSelf || !_rootObject.activeInHierarchy)
        {
            return;
        } 

        if (_pageStack.Count > 1)
        {
            print("here");
            PopPage();
            return;
        }
        
        if (_pageStack.Count == 1 && _pageStack.Peek() == _startPage) { return; }
        if (_pageStack.Count == 1 && _pageStack.Peek() == _settingsPage) { print("popping settings"); PopPage(); return; }

        PushPage(_settingsPage);
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
        if (!page.gameObject.activeSelf) { page.gameObject.SetActive(true); }

        page.Enter();

        if (_pageStack.Count > 0)
        {
            Page currentPage = _pageStack.Peek();

            if (currentPage.VisuallyExitsOnPush)
            {
                currentPage.Exit();
            }
            else if (currentPage.DisablesRaycastsOnPush)
            {
                currentPage.DisableGraphicsRaycaster();
            }
        }

        _pageStack.Push(page);
        if (page.BlocksGameInput) { OnBlockGameInput?.Invoke(); }
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
            return;
        }

        Page newPage = _pageStack.Peek();

        if (newPage.VisuallyExitsOnPush)
        {
            if (!newPage.gameObject.activeSelf) { newPage.gameObject.SetActive(true); }
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

    public void ReturnToStartPage()
    {
        PopAllPages();
        PushPage(_startPage);
    }

    public bool IsGameInputBlocked()
    {
        return _pageStack.Count > 0 && _pageStack.Peek().BlocksGameInput;
    }

    public int ActivePageCount
    {
        get
        {
            return _pageStack.Count;
        }
    }

    public void SetMenuCamera(Camera camera)
    {
        _menuCamera = camera;
    }

    public void RegisterOnBlockGameInputListener(OnBlockGameInputListener listener)
    {
        OnBlockGameInput += listener;
    }

    public void UnregisterOnBlockGameInputListener(OnBlockGameInputListener listener)
    {
        OnBlockGameInput -= listener;
    }
}
