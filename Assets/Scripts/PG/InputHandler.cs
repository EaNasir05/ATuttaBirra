using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class InputHandler : MonoBehaviour
{
    private PlayerInput _playerInput;
    public Action<Vector2> OnMoveLInput;
    public Action<Vector2> OnMoveRInput;
    public Action<bool> OnHoldGlassTInput;
    public Action<bool> OnHoldGlassSInput;
    public Action<bool> OnGrabLeverTInput;
    public Action<bool> OnGrabLeverSInput;
    public Action OnTestInput;
    public Action OnPauseInput;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    public void OnMoveL(InputValue input)
    {
        var moveVector = input.Get<Vector2>();
        OnMoveLInput?.Invoke(moveVector);
    }

    public void OnMoveR(InputValue input)
    {
        var moveVector = input.Get<Vector2>();
        OnMoveRInput?.Invoke(moveVector);
    }

    public void OnHoldGlassT(InputValue input)
    {
        OnHoldGlassTInput?.Invoke(input.isPressed);
    }

    public void OnHoldGlassS(InputValue input)
    {
        OnHoldGlassSInput?.Invoke(input.isPressed);
    }

    public void OnGrabLeverT(InputValue input)
    {
        OnGrabLeverTInput?.Invoke(input.isPressed);
    }

    public void OnGrabLeverS(InputValue input)
    {
        OnGrabLeverSInput?.Invoke(input.isPressed);
    }

    public void OnTest(InputValue input)
    {
        OnTestInput?.Invoke();
    }

    public void OnPause(InputValue input)
    {
        OnPauseInput?.Invoke();
    }
}
