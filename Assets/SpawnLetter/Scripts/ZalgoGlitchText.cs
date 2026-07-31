using UnityEngine;
using TMPro;
using System.Text;

/// <summary>
/// Applies a continuously-mutating Zalgo/glitch effect to a TMP text —
/// intended for the "Tu IP ha sido descubierta" game over message.
/// Attach directly to the TMP_Text object. Regenerates automatically
/// whenever the object is enabled (e.g. when the game over panel activates).
/// </summary>
public class ZalgoGlitchText : MonoBehaviour
{
    [Header("Setup")]
    public TMP_Text targetText;
    [TextArea] public string baseText = "Tu IP ha sido descubierta";

    [Header("Zalgo Intensity")]
    public int minMarksPerChar = 2;
    public int maxMarksPerChar = 6;

    [Header("Random Glitch Bursts")]
    [Range(0f, 1f)] public float glitchBurstChance = 0.12f;
    public int burstMinMarks = 10;
    public int burstMaxMarks = 18;

    [Header("Timing")]
    public float refreshInterval = 0.15f;

    [Header("Visual Glitch")]
    public bool jitterPosition = true;
    public float jitterAmount = 2f;
    public bool flickerColor = true;
    public Color[] flickerColors = new Color[]
    {
        Color.white,
        new Color(1f, 0.15f, 0.15f), // red
        Color.white
    };

    // Combining diacritical marks: above, below, and through the character
    private static readonly char[] zalgoAbove =
    {
        '\u030d','\u030e','\u0304','\u0305','\u033f','\u0311','\u0306','\u0310','\u0352','\u0357',
        '\u0351','\u0307','\u0308','\u030a','\u0342','\u0343','\u0344','\u034a','\u034b','\u034c',
        '\u0303','\u0302','\u030c','\u0350','\u0300','\u0301','\u030b','\u030f','\u0312','\u0313',
        '\u0314','\u033d','\u0309','\u0363','\u0364','\u0365','\u0366','\u0367','\u0368','\u0369',
        '\u036a','\u036b','\u036c','\u036d','\u036e','\u036f','\u033e','\u035b','\u0346','\u031a'
    };
    private static readonly char[] zalgoMiddle =
    {
        '\u0315','\u031b','\u0340','\u0341','\u0358','\u0321','\u0322','\u0327','\u0328','\u0334',
        '\u0335','\u0336','\u034f','\u035c','\u035d','\u035e','\u035f','\u0360','\u0362','\u0338',
        '\u0337','\u0361','\u0489'
    };
    private static readonly char[] zalgoBelow =
    {
        '\u0316','\u0317','\u0318','\u0319','\u031c','\u031d','\u031e','\u031f','\u0320','\u0324',
        '\u0325','\u0326','\u0329','\u032a','\u032b','\u032c','\u032d','\u032e','\u032f','\u0330',
        '\u0331','\u0332','\u0333','\u0339','\u033a','\u033b','\u033c','\u0345','\u0347','\u0348',
        '\u0349','\u034d','\u034e','\u0353','\u0354','\u0355','\u0356','\u0359','\u035a','\u0323'
    };

    private float timer;
    private RectTransform rt;
    private Vector2 basePos;

    private void Awake()
    {
        if (targetText == null) targetText = GetComponent<TMP_Text>();
        rt = targetText != null ? targetText.rectTransform : null;
        if (rt != null) basePos = rt.anchoredPosition;
    }

    private void OnEnable()
    {
        TriggerGlitch();
    }

    private void Update()
    {
        if (targetText == null) return;

        timer += Time.unscaledDeltaTime;
        if (timer >= refreshInterval)
        {
            timer = 0f;
            RefreshGlitch();
        }
    }

    /// <summary>
    /// Call this explicitly when the game over screen activates (e.g. from
    /// GameOverManager.ActivarGameOver()) since the panel likely stays "enabled"
    /// the whole time (shown/hidden via CanvasGroup alpha), so OnEnable alone
    /// won't refire on repeat game overs.
    /// </summary>
    public void TriggerGlitch()
    {
        timer = refreshInterval; // forces an immediate refresh next Update
        RefreshGlitch();
    }

    private void RefreshGlitch()
    {
        bool burst = Random.value < glitchBurstChance;
        int minM = burst ? burstMinMarks : minMarksPerChar;
        int maxM = burst ? burstMaxMarks : maxMarksPerChar;

        targetText.text = ApplyZalgo(baseText, minM, maxM);

        if (jitterPosition && rt != null)
        {
            float jx = Random.Range(-jitterAmount, jitterAmount);
            float jy = Random.Range(-jitterAmount, jitterAmount);
            rt.anchoredPosition = basePos + new Vector2(jx, jy);
        }

        if (flickerColor && flickerColors.Length > 0)
        {
            targetText.color = flickerColors[Random.Range(0, flickerColors.Length)];
        }
    }

    private string ApplyZalgo(string input, int minMarks, int maxMarks)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in input)
        {
            sb.Append(c);

            if (char.IsWhiteSpace(c)) continue; // keep spaces clean so words stay legible

            int markCount = Random.Range(minMarks, maxMarks + 1);
            for (int i = 0; i < markCount; i++)
            {
                float roll = Random.value;
                char[] pool = roll < 0.4f ? zalgoAbove : (roll < 0.7f ? zalgoBelow : zalgoMiddle);
                sb.Append(pool[Random.Range(0, pool.Length)]);
            }
        }
        return sb.ToString();
    }

    /// <summary>Call if this text's layout/anchored position changes at runtime.</summary>
    public void RecalculateBasePosition()
    {
        if (rt != null) basePos = rt.anchoredPosition;
    }
}