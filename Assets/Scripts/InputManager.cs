#define USE_NEW_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputSystem_Actions playerInputActions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        playerInputActions = new PlayerInputSystem_Actions();
        playerInputActions.Player.Enable();

    }

    public Vector2 GetMouseScreenPosition()
    {
        return Mouse.current.position.ReadValue();
    }

    public bool IsMouseLeftButtonDown()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Click.WasPressedThisFrame();
#else
        return Mouse.current.leftButton.wasPressedThisFrame;
#endif
    }

    public Vector2 GetCameraMoveVector()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        Vector2 inputDir = Vector2.zero;
        var keyboard = Keyboard.current;
        if (keyboard.wKey.isPressed) inputDir.y += 1f;
        if (keyboard.sKey.isPressed) inputDir.y -= 1f;
        if (keyboard.aKey.isPressed) inputDir.x -= 1f;
        if (keyboard.dKey.isPressed) inputDir.x += 1f;
        return inputDir;
#endif
    }

    public float GetCameraRotate()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
        var keyboard = Keyboard.current;

        float rotateDir = 0f;
        if (keyboard.qKey.isPressed) rotateDir -= 1f;
        if (keyboard.eKey.isPressed) rotateDir += 1f;

        return rotateDir;
#endif
    }

    public float GetCameraZoomAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        var mouse = Mouse.current;

        float scrollY = mouse.scroll.ReadValue().y;
        float zoomAmount = 0f;
        if (scrollY > 0f)
        {
            zoomAmount = 1;
        }
        else if (scrollY < 0f)
        {
            zoomAmount = -1;
        }

        return zoomAmount;
#endif
    }
}
