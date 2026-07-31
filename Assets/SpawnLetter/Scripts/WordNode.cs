using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;

public enum WordType { Normal, Heal, Freeze, Glitch }

public class WordNode : MonoBehaviour
{
    public string originalWord;
    public int characterIndex = 0;
    public WordType myType = WordType.Normal; 

    [Header("Movimiento y Límites")]
    public float fallSpeed = 2f;
    public float destroyY = -4.5f;

    [Header("Duración de Efectos")]
    public float freezeDuration = 3f;
    public float glitchScrambleDuration = 0.3f;
    public float healGlowDuration = 1f;

    [HideInInspector]
    public TMP_Text wordText;

    private float originalFallSpeed;
    private Coroutine freezeCoroutine;

    private void Awake()
    {
        if (wordText == null)
        {
            wordText = GetComponent<TMP_Text>();
        }

        originalFallSpeed = fallSpeed;
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

        // Si se completó toda la palabra al teclear la letra
        if (IsWordComplete())
        {
            TriggerPowerUpEffect();
            
            // Suma puntos normales en el GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(originalWord.Length);
            }

            Destroy(gameObject);
        }
    }

    public bool IsWordComplete()
    {
        return characterIndex >= originalWord.Length;
    }

    public void TriggerErrorEffect()
    {
        UpdateTextDisplay(true);
    }

    private void TriggerPowerUpEffect()
    {
        switch (myType)
        {
            case WordType.Heal:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.lives++;
                    Debug.Log("<color=green>¡HEAL ACTIVADO! Vida recuperada.</color>");
                }

                if (AsciiRainEffect.Instance != null)
                    AsciiRainEffect.Instance.TriggerHeal(healGlowDuration);
                break;

            case WordType.Freeze:
                // Congela temporalmente todas las demás palabras: se tiñen de azul y dejan de moverse
                WordNode[] allWords = FindObjectsOfType<WordNode>();
                foreach (var w in allWords)
                {
                    if (w != this) w.ApplyFreeze(freezeDuration);
                }

                if (AsciiRainEffect.Instance != null)
                    AsciiRainEffect.Instance.TriggerFreeze(freezeDuration);

                Debug.Log("<color=cyan>¡FREEZE ACTIVADO! Palabras congeladas.</color>");
                break;

            case WordType.Glitch:
                // Efecto bomba: las demás palabras se scramblean brevemente antes de destruirse
                WordNode[] activeWords = FindObjectsOfType<WordNode>();
                foreach (var w in activeWords)
                {
                    if (w != this) w.PlayGlitchAndDestroy(glitchScrambleDuration);
                }

                if (AsciiRainEffect.Instance != null)
                    AsciiRainEffect.Instance.TriggerGlitch(glitchScrambleDuration);

                Debug.Log("<color=magenta>¡GLITCH ACTIVADO! Pantalla limpiada.</color>");
                break;
        }
    }

    /// <summary>
    /// Tints this word blue and stops its fall for the given duration, then restores
    /// its original speed and appearance. Called on OTHER words when a Freeze word is typed.
    /// </summary>
    public void ApplyFreeze(float duration)
    {
        if (freezeCoroutine != null) StopCoroutine(freezeCoroutine);
        freezeCoroutine = StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        fallSpeed = 0f;
        if (wordText != null) wordText.color = new Color(0.3f, 0.6f, 1f); // blue tint

        yield return new WaitForSeconds(duration);

        fallSpeed = originalFallSpeed;
        if (wordText != null) wordText.color = Color.white; // back to default; typed/untyped colors are re-applied via rich text tags on next TypeLetter/error call
        freezeCoroutine = null;
    }

    /// <summary>
    /// Scrambles this word's displayed characters briefly, then destroys it.
    /// Called on OTHER words when a Glitch word is typed.
    /// </summary>
    public void PlayGlitchAndDestroy(float duration)
    {
        StopAllCoroutines(); // cancel any freeze in progress on this word, glitch takes priority
        StartCoroutine(GlitchScrambleRoutine(duration));
    }

    private IEnumerator GlitchScrambleRoutine(float duration)
    {
        const string scrambleChars = "!@#$%^&*<>/\\";
        float elapsed = 0f;
        float tick = 0.05f;

        while (elapsed < duration)
        {
            if (wordText != null && originalWord.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < originalWord.Length; i++)
                    sb.Append(scrambleChars[Random.Range(0, scrambleChars.Length)]);

                wordText.text = $"<color=#FF00FF>{sb}</color>";
            }

            elapsed += tick;
            yield return new WaitForSeconds(tick);
        }

        Destroy(gameObject);
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

        // If this word is the one currently targeted, echo progress to the terminal prompt.
        // (Assumes some other script — likely your input handler — calls
        // ShipController.Instance.SetTarget(transform) when this word becomes active.)
        if (ShipController.Instance != null && ShipController.Instance.GetTarget() == transform)
        {
            ShipController.Instance.UpdateTypedText(typedPart, originalWord);
        }
    }
}