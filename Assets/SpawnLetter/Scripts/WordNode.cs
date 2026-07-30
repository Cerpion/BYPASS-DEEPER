using UnityEngine;
using TMPro;

public enum WordType { Normal, Heal, Freeze, Glitch }

public class WordNode : MonoBehaviour
{
    public string originalWord;
    public int characterIndex = 0;
    public WordType myType = WordType.Normal; 

    [Header("Movimiento y Límites")]
    public float fallSpeed = 2f;
    public float destroyY = -4.5f;

    [HideInInspector]
    public TMP_Text wordText;

    private void Awake()
    {
        if (wordText == null)
        {
            wordText = GetComponent<TMP_Text>();
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        if (transform.position.y < destroyY)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }

    public void SetWord(string word)
    {
        originalWord = word.ToUpper();
        characterIndex = 0;
        
        if (wordText == null) wordText = GetComponent<TMP_Text>();
        UpdateTextDisplay(false);
    }

    public void SetupSpecialType(WordType newType)
    {
        myType = newType;
        if (wordText == null) wordText = GetComponent<TMP_Text>();

        if (wordText != null)
        {
            switch (myType)
            {
                case WordType.Normal: wordText.color = Color.white; break;
                case WordType.Heal: wordText.color = Color.green; break;
                case WordType.Freeze: wordText.color = Color.cyan; break;
                case WordType.Glitch: wordText.color = Color.magenta; break;
            }
        }
    }

    public char GetNextLetter()
    {
        return characterIndex < originalWord.Length ? originalWord[characterIndex] : '\0';
    }

    public void TypeLetter()
    {
        characterIndex++;
        UpdateTextDisplay(false);
    }

    public bool IsWordComplete()
    {
        return characterIndex >= originalWord.Length;
    }

    public void TriggerErrorEffect()
    {
        UpdateTextDisplay(true);
    }

    public void UpdateTextDisplay(bool hasError)
    {
        if (wordText == null)
        {
            wordText = GetComponent<TMP_Text>();
            if (wordText == null) return;
        }

        int safeIndex = Mathf.Clamp(characterIndex, 0, originalWord.Length);
        string typedPart = originalWord.Substring(0, safeIndex);
        string untypedPart = originalWord.Substring(safeIndex);

        string colorHex = hasError ? "#FF0000" : "#55FF55"; 
        wordText.text = $"<color={colorHex}>{typedPart}</color>{untypedPart}";
    }
}