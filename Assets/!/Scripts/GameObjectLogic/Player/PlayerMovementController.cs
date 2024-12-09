using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] Transform _playerHead; 
    [SerializeField] GameObject _playerHand;
    [SerializeField] Animator _playerHandAnimator;

    [SerializeField] float _lookSensitivity = 1f;

    public GameObject PlayerHead { get => _playerHead.gameObject; }

    public delegate void PlayerRotationEventListener();
    public event PlayerRotationEventListener PlayerRotationEvent;

    public delegate void PlayerPointingEventListener();
    public event PlayerPointingEventListener PlayerPointingEvent;

    public bool IsRotating { get; protected set; } = false;
    public bool IsPointing { get; protected set; } = false;

    float _mouseY;
    float _mouseX;
    Vector3 _rotate;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (IsRotating)
        {
            RotateHead();
            PlayerRotationEvent?.Invoke();
        }

        if (IsPointing)
        {
            MoveHand();
            PlayerPointingEvent?.Invoke();
        }
    }

    public void SetRotating(bool isRotating)
    {
        IsRotating = isRotating;
        if (IsRotating) 
        { 
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            RotateHead();  // Otherwise it will incorrect by 1 frame.
        }
        else 
        { 
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ToggleRotation()
    {
        SetRotating(!IsRotating);
    }

    public void SetPointing(bool isPointing)
    {
        IsPointing = isPointing;
        _playerHand.SetActive(isPointing);
        SetRotating(isPointing);

        if (IsPointing)
        {
            _playerHandAnimator.Play("PalmToPoint");
        }
    }
    
    public void TogglePointing()
    {
        SetPointing(!IsPointing);
    }

    public void RegisterPlayerRotationEventListener(PlayerRotationEventListener eventListener)
    {
        PlayerRotationEvent += eventListener;
    }

    public void UnRegisterPlayerRotationEventListener(PlayerRotationEventListener eventListener)
    {
        PlayerRotationEvent -= eventListener;
    }

    public void RegisterPlayerPointingEventListener(PlayerPointingEventListener eventListener)
    {
        PlayerPointingEvent += eventListener;
    }

    public void UnRegisterPlayerPointingEventListener(PlayerPointingEventListener eventListener)
    {
        PlayerPointingEvent -= eventListener;
    }

    void MoveHand(float distanceFromHead=0.75f)
    {
        // Assumes hand is active
        RotateHead();
        Vector3 forward = _playerHead.transform.forward;
        _playerHand.transform.SetPositionAndRotation(_playerHead.transform.position + forward * distanceFromHead, Quaternion.LookRotation(forward) * Quaternion.Euler(0, -90, -90));
    }

    void RotateHead()
    {
        // Assumes is rotating
        _mouseY = Input.GetAxis("Mouse X");
        _mouseX = Input.GetAxis("Mouse Y");
        _rotate = new Vector3(-_mouseX, _mouseY * _lookSensitivity, 0);
        _playerHead.eulerAngles += _rotate;
    }
}
