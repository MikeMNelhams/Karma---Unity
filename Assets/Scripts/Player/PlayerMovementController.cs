using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] Transform _playerHead; 
    [SerializeField] float _lookSensitivity = 1f;

    bool _isRotating = false;

    float mouseY;
    float mouseX;
    Vector3 rotate;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetRotating(bool isRotating)
    {
        _isRotating = isRotating;
        if (_isRotating) 
        { 
            Cursor.visible = false;
            RotateHead();  // Otherwise it will incorrect by 1 frame.
        }
        else { Cursor.visible = true; }
    }

    public void ToggleRotation()
    {
        SetRotating(!_isRotating);
    }

    void Update()
    {
        if (_isRotating)
        {
            RotateHead();
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
