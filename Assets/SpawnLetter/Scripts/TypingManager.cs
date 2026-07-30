using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingManager : MonoBehaviour
{
    private WordNode activeWordNode = null;

    [Header("Efectos Visuales")]
    public GameObject floatingTextPrefab;
    public GameObject hackParticlesPrefab;

    private void OnEnable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput += OnTextInput;
        }
    }

    private void OnDisable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput -= OnTextInput;
        }
    }

    private void OnTextInput(char ch)
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        ProcessInput(char.ToUpper(ch));
    }

    private void ProcessInput(char letter)
    {
        if (activeWordNode != null)
        {
            if (activeWordNode.GetNextLetter() == letter)
            {
                activeWordNode.TypeLetter();

                if (activeWordNode.IsWordComplete())
                {
                    OnWordCompleted();
                }
            }
            else
            {
                RegisterError(activeWordNode);
            }
        }
        else
        {
            WordNode[] nodes = FindObjectsByType<WordNode>(FindObjectsSortMode.None)
                .OrderBy(n => n.transform.position.y)
                .ToArray();

            bool foundMatch = false;

            foreach (WordNode node in nodes)
            {
                if (node != null &&
                    !node.IsWordComplete() &&
                    node.GetNextLetter() == letter)
                {
                    activeWordNode = node;

                    // La nave sigue la palabra seleccionada
                    if (ShipController.Instance != null)
                    {
                        ShipController.Instance.SetTarget(activeWordNode.transform);
                    }

                    activeWordNode.TypeLetter();

                    if (activeWordNode.IsWordComplete())
                    {
                        OnWordCompleted();
                    }

                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch && nodes.Length > 0)
            {
                RegisterError(nodes[0]);
            }
        }
    }

    private void RegisterError(WordNode nodeToGlitched)
    {
        if (nodeToGlitched != null)
        {
            nodeToGlitched.TriggerErrorEffect();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetCombo();
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.1f, 0.15f);
        }
    }

    private void OnWordCompleted()
    {
        if (activeWordNode == null)
            return;

        // Disparar desde la nave
        if (ShipController.Instance != null)
        {
            ShipController.Instance.Shoot();
            ShipController.Instance.ClearTarget();
        }

        Vector3 spawnPos = new Vector3(
            activeWordNode.transform.position.x,
            activeWordNode.transform.position.y,
            -2f);

        if (hackParticlesPrefab != null)
        {
            GameObject fx = Instantiate(
                hackParticlesPrefab,
                spawnPos,
                Quaternion.identity);

            fx.transform.localScale = Vector3.one;
            Destroy(fx, 2f);
        }
        else
        {
            Debug.LogWarning("Falta asignar Hack Particles Prefab.");
        }

        if (floatingTextPrefab != null)
        {
            GameObject floatObj = Instantiate(
                floatingTextPrefab,
                activeWordNode.transform.position,
                Quaternion.identity);

            floatObj.transform.localScale = Vector3.one;

            FloatingText floatScript = floatObj.GetComponent<FloatingText>();

            if (floatScript != null)
            {
                floatScript.SetText("+10");
            }
        }
        else
        {
            Debug.LogWarning("Falta asignar Floating Text Prefab.");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(10);
        }

        GameObject wordToDestroy = activeWordNode.gameObject;

        activeWordNode = null;

        Destroy(wordToDestroy);
    }
}