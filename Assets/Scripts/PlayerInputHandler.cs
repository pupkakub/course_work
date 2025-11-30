using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public bool interactPressed;

    void Update()
    {
        interactPressed = Keyboard.current.eKey.wasPressedThisFrame;
    }
}
