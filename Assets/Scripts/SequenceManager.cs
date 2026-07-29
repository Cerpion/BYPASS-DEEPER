using UnityEngine;
using UnityEngine.InputSystem;

public class SequenceManager : MonoBehaviour
{
    [Header("Secuencia de prueba")]
    [SerializeField] private Key[] sequence =
    {
        Key.A,
        Key.S,
        Key.D,
        Key.W
    };

    private int currentIndex = 0;

    public void CheckInput(Key inputKey)
    {
        if (inputKey == sequence[currentIndex])
        {
            Debug.Log("Correcto: " + inputKey);

            currentIndex++;

            if (currentIndex >= sequence.Length)
            {
                Debug.Log("¡Secuencia completada!");

                currentIndex = 0;
            }
        }
        else
        {
            Debug.Log("Incorrecto: " + inputKey);

            currentIndex = 0;
        }
    }
}