using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class KeyboardInput : MonoBehaviour
{
    [SerializeField] private SequenceManager sequenceManager;
    void Update()
    {
        if (Keyboard.current == null)
            return;

        foreach (KeyControl key in Keyboard.current.allKeys)
        {
            if (key.wasPressedThisFrame)
            {
                sequenceManager.CheckInput(key.keyCode);
            }
        }
    }
}