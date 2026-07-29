using System.Collections;
using UnityEngine;
using TMPro;

public class WordNode : MonoBehaviour
{
    public float fallSpeed = 2f;
    private string originalWord;
    private int characterIndex = 0;
    private TMP_Text tmpText;

    private bool isShaking = false;
    private Vector3 originalLocalPos;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    public void SetWord(string word)
    {
        originalWord = word.ToUpper();
        characterIndex = 0;
        UpdateTextDisplay(false);
    }

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -5f)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }

    public char GetNextLetter()
    {
        if (characterIndex >= originalWord.Length) return '\0';
        return originalWord[characterIndex];
    }

    public void TypeLetter()
    {
        characterIndex++;
        UpdateTextDisplay(false);
    }

    public void TriggerErrorEffect()
    {
        if (!isShaking)
        {
            StartCoroutine(ErrorShakeRoutine());
        }
    }

    private IEnumerator ErrorShakeRoutine()
    {
        isShaking = true;
        Vector3 startPos = transform.position;
        float duration = 0.15f; 
        float elapsed = 0f;

        UpdateTextDisplay(showError: true);

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-0.15f, 0.15f);
            float offsetY = Random.Range(-0.15f, 0.15f);
            transform.position = startPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        UpdateTextDisplay(showError: false);
        isShaking = false;
    }

    public bool IsWordComplete()
    {
        return characterIndex >= originalWord.Length;
    }

    private void UpdateTextDisplay(bool showError)
    {
        if (tmpText == null) return;

        string typedPart = $"<color=#00FF00>{originalWord.Substring(0, characterIndex)}</color>";
        
        if (characterIndex < originalWord.Length)
        {
            char current = originalWord[characterIndex];
            string rest = originalWord.Substring(characterIndex + 1);

            if (showError)
            {
                tmpText.text = $"{typedPart}<color=#FF0000>{current}</color>{rest}";
            }
            else
            {
                tmpText.text = $"{typedPart}{current}{rest}";
            }
        }
        else
        {
            tmpText.text = typedPart;
        }
    }

    public string GetOriginalWord() => originalWord;
}