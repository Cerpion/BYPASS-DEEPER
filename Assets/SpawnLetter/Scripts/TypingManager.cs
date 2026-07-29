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
                activeWordNode.TriggerErrorEffect();
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
                nodes[0].TriggerErrorEffect();
            }
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

        GameObject wordToDestroy = activeWordNode.gameObject;
        activeWordNode = null; 
        Destroy(wordToDestroy);
    }
}
}