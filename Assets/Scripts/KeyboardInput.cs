using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class KeyboardInput : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null)
            return;

        foreach (KeyControl key in Keyboard.current.allKeys)
        {
            if (key.wasPressedThisFrame)
            {
                Debug.Log("Tecla: " + key.displayName + " | Código: " + key.keyCode);
            }
        }
    }
}