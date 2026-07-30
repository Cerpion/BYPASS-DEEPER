using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingManager : MonoBehaviour
{
    private WordNode activeWordNode = null;

    [Header("Efectos Visuales")]
    public GameObject floatingTextPrefab;
    public GameObject hackParticlesPrefab;
    [SerializeField] private ShipController shipController;
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
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

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
                if (node != null && !node.IsWordComplete() && node.GetNextLetter() == letter)
                {
                    activeWordNode = node;
                    if (shipController != null)
                    {
                    shipController.SetTarget(activeWordNode.transform);
                    }
                    activeWordNode.TypeLetter();
                    foundMatch = true;

                    if (activeWordNode.IsWordComplete())
                    {
                        OnWordCompleted();
                    }
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
        Debug.Log("¡Palabra Hackeada!");

        if (activeWordNode != null)
        {
            Vector3 spawnPos = activeWordNode.transform.position + new Vector3(0, 0, -1f);

            if (floatingTextPrefab != null)
            {
                GameObject floatObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
                FloatingText floatScript = floatObj.GetComponent<FloatingText>();
                if (floatScript != null)
                {
                    floatScript.SetText("+10");
                }
            }

            if (hackParticlesPrefab != null)
            {
                GameObject fx = Instantiate(hackParticlesPrefab, spawnPos, Quaternion.identity);
                Destroy(fx, 1f);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(10);
            }

            if (shipController != null)
            {
                shipController.Shoot();
            }

            // 4. Destruir objeto y liberar referencia
           // GameObject wordToDestroy = activeWordNode.gameObject;
            //activeWordNode = null;
           // Destroy(wordToDestroy);
        }
    }
}