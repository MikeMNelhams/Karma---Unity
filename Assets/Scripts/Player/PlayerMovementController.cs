using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] Transform _playerHead; 
    [SerializeField] GameObject _playerHand;

    [SerializeField] float _lookSensitivity = 1f;

    public delegate void PlayerRotationEventListener();
    public event PlayerRotationEventListener PlayerRotationEvent;

    public bool IsRotating { get; protected set; } = false;

    float mouseY;
    float mouseX;
    Vector3 rotate;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        _playerHand.SetActive(false);
    }

    public void SetRotating(bool isRotating)
    {
        IsRotating = isRotating;
        if (IsRotating) 
        { 
            Cursor.visible = false;
            RotateHead();  // Otherwise it will incorrect by 1 frame.
        }
        else { Cursor.visible = true; }
    }

    public void ToggleRotation()
    {
        SetRotating(!IsRotating);
    }

    void Update()
    {
        if (IsRotating)
        {
            RotateHead();
            PlayerRotationEvent?.Invoke();
        }
    }

    void RotateHead()
    {
        // Assumes is rotating
        mouseY = Input.GetAxis("Mouse X");
        mouseX = Input.GetAxis("Mouse Y");
        rotate = new Vector3(-mouseX, mouseY * _lookSensitivity, 0);
        _playerHead.eulerAngles = _playerHead.eulerAngles + rotate;
    }
}
