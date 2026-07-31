using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spawns and manages multiple falling ASCII "rain" columns behind gameplay.
/// Reacts to GameManager's depth level (intensity + color) and to WordNode
/// power-up effects (Freeze / Glitch / Heal) via public trigger methods.
/// </summary>
public class AsciiRainEffect : MonoBehaviour
{
    public static AsciiRainEffect Instance;

    [Header("Setup")]
    [SerializeField] private RectTransform container;
    [SerializeField] private TMP_FontAsset font;

    [Header("Column Settings")]
    [SerializeField] private int columnCount = 30;
    [SerializeField] private float columnWidth = 24f;
    [SerializeField] private int minLength = 8;
    [SerializeField] private int maxLength = 20;
    [SerializeField] private float minSpeed = 100f;
    [SerializeField] private float maxSpeed = 300f;
    [SerializeField] private float charRefreshInterval = 0.1f;

    [Header("Character Pool")]
    [SerializeField] private string charPool = "01#@$%*+-./\\<>[]{}=;:";

    [Header("Level Color/Intensity Range (Layer -1 to -4)")]
    [SerializeField] private float minIntensity = 0.5f;   // Layer -1
    [SerializeField] private float maxIntensity = 2.5f;   // Layer -4
    [SerializeField] private Color colorLayer1 = new Color(0.1f, 0.6f, 0.3f, 0.6f); // green
    [SerializeField] private Color colorLayer4 = new Color(0.8f, 0.15f, 0.15f, 0.6f); // red

    [Header("Density Range (Layer -1 to -4)")]
    [Tooltip("Fraction of the full column pool active at Layer -1. Layer -4 always uses 100%.")]
    [SerializeField, Range(0.1f, 1f)] private float minDensityFraction = 0.35f;

    [Header("Effect Colors")]
    [SerializeField] private Color freezeColor = new Color(0.2f, 0.5f, 1f, 0.7f);
    [SerializeField] private Color healColor = new Color(0.6f, 1f, 0.6f, 0.9f);

    private List<AsciiRainColumn> columns = new List<AsciiRainColumn>();

    // Base state, driven by depth level
    private Color baseColor;
    private float baseIntensity = 1f;

    // Transient override state, driven by word effects
    private Color? overrideColor = null;
    private float freezeMultiplier = 1f;
    private float glitchChanceBoost = 0f;

    private Coroutine freezeRoutine;
    private Coroutine glitchRoutine;
    private Coroutine healRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (container == null || font == null)
        {
            Debug.LogError("AsciiRainEffect: container or font not assigned.");
            return;
        }

        SpawnColumns();
        ShuffleColumnOrder();
        SetLevel(1); // establishes initial intensity, color, and density
    }

    private void SpawnColumns()
    {
        float totalWidth = container.rect.width;
        int maxColumnsFit = Mathf.Max(1, Mathf.FloorToInt(totalWidth / columnWidth));
        int actualCount = Mathf.Min(columnCount, maxColumnsFit);

        for (int i = 0; i < actualCount; i++)
        {
            GameObject colObj = new GameObject($"RainColumn_{i}", typeof(RectTransform));
            colObj.transform.SetParent(container, false);

            TextMeshProUGUI tmp = colObj.AddComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.color = baseColor;
            tmp.alignment = TextAlignmentOptions.Top;
            tmp.enableWordWrapping = false;
            tmp.fontSize = columnWidth * 0.9f;
            tmp.raycastTarget = false;

            RectTransform rt = colObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(columnWidth, container.rect.height * 2f);

            float xPos = (i * columnWidth) + (columnWidth * 0.5f);
            rt.anchoredPosition = new Vector2(xPos, 0f);

            AsciiRainColumn column = colObj.AddComponent<AsciiRainColumn>();
            column.Init(this, rt, tmp);
            columns.Add(column);
        }
    }

    private void ShuffleColumnOrder()
    {
        // Fisher-Yates shuffle so "first N active" picks varied screen positions
        // rather than always the same leftmost columns as density changes.
        for (int i = columns.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (columns[i], columns[j]) = (columns[j], columns[i]);
        }
    }

    // ---------- Called by GameManager ----------

    /// <summary>
    /// Call whenever depthLevel changes. Levels 1-4 map linearly to
    /// intensity (min->max), color (green->red), and column density.
    /// </summary>
    public void SetLevel(int level)
    {
        float t = Mathf.InverseLerp(1f, 4f, Mathf.Clamp(level, 1, 4));
        baseIntensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        Color c = Color.Lerp(colorLayer1, colorLayer4, t);
        c.a = colorLayer1.a; // keep consistent alpha regardless of lerp
        baseColor = c;

        float densityFraction = Mathf.Lerp(minDensityFraction, 1f, t);
        int targetActiveCount = Mathf.Max(1, Mathf.RoundToInt(densityFraction * columns.Count));
        ApplyDensity(targetActiveCount);
    }

    private void ApplyDensity(int targetActiveCount)
    {
        for (int i = 0; i < columns.Count; i++)
        {
            bool shouldBeActive = i < targetActiveCount;
            bool isActive = columns[i].gameObject.activeSelf;

            if (shouldBeActive && !isActive)
            {
                columns[i].ResetColumn();
                columns[i].gameObject.SetActive(true);
            }
            else if (!shouldBeActive && isActive)
            {
                columns[i].gameObject.SetActive(false);
            }
        }
    }

    // ---------- Called by WordNode power-ups ----------

    public void TriggerFreeze(float duration)
    {
        if (freezeRoutine != null) StopCoroutine(freezeRoutine);
        freezeRoutine = StartCoroutine(FreezeRoutine(duration));
    }

    public void TriggerGlitch(float duration)
    {
        if (glitchRoutine != null) StopCoroutine(glitchRoutine);
        glitchRoutine = StartCoroutine(GlitchRoutine(duration));
    }

    public void TriggerHeal(float duration)
    {
        if (healRoutine != null) StopCoroutine(healRoutine);
        healRoutine = StartCoroutine(HealRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        freezeMultiplier = 0f;
        overrideColor = freezeColor;

        yield return new WaitForSeconds(duration);

        freezeMultiplier = 1f;
        overrideColor = null;
        freezeRoutine = null;
    }

    private IEnumerator GlitchRoutine(float duration)
    {
        glitchChanceBoost = 1f; // every char shuffles every refresh tick

        yield return new WaitForSeconds(duration);

        glitchChanceBoost = 0f;
        glitchRoutine = null;
    }

    private IEnumerator HealRoutine(float duration)
    {
        float half = duration * 0.5f;
        float elapsed = 0f;

        // Fade in toward the heal glow
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            overrideColor = Color.Lerp(baseColor, healColor, elapsed / half);
            yield return null;
        }

        elapsed = 0f;
        // Fade back out to the base level color
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            overrideColor = Color.Lerp(healColor, baseColor, elapsed / half);
            yield return null;
        }

        overrideColor = null;
        healRoutine = null;
    }

    // ---------- Read by columns ----------

    public Color EffectiveColor => overrideColor ?? baseColor;
    public float EffectiveIntensity => baseIntensity * freezeMultiplier;
    public float GlitchChanceBoost => glitchChanceBoost;

    public char RandomChar() => charPool[Random.Range(0, charPool.Length)];
    public int RandomLength() => Random.Range(minLength, maxLength + 1);
    public float RandomSpeed() => Random.Range(minSpeed, maxSpeed);
    public float ContainerHeight() => container.rect.height;
    public float RefreshInterval() => charRefreshInterval;
}

/// <summary>
/// Individual falling column. Created and driven entirely by AsciiRainEffect.
/// </summary>
public class AsciiRainColumn : MonoBehaviour
{
    private AsciiRainEffect manager;
    private RectTransform rt;
    private TextMeshProUGUI tmp;

    private char[] chars;
    private float fallSpeed;
    private float charTimer;

    public void Init(AsciiRainEffect owner, RectTransform rectTransform, TextMeshProUGUI text)
    {
        manager = owner;
        rt = rectTransform;
        tmp = text;

        RegenerateColumn();

        float startY = Random.Range(-manager.ContainerHeight(), 0f);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, startY);
    }

    /// <summary>
    /// Called when a previously-inactive column is turned back on due to a
    /// density increase, so it doesn't resume from a stale position.
    /// </summary>
    public void ResetColumn()
    {
        RegenerateColumn();
        float startY = Random.Range(-manager.ContainerHeight(), 0f);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, startY);
        charTimer = 0f;
    }

    private void Update()
    {
        if (manager == null) return;

        float speed = fallSpeed * Mathf.Max(0f, manager.EffectiveIntensity);
        Vector2 pos = rt.anchoredPosition;
        pos.y -= speed * Time.deltaTime;

        float columnPixelLength = chars.Length * tmp.fontSize;
        if (pos.y < -(manager.ContainerHeight() + columnPixelLength))
        {
            pos.y = 0f;
            RegenerateColumn();
        }

        rt.anchoredPosition = pos;
        tmp.color = manager.EffectiveColor;

        charTimer += Time.deltaTime;
        if (charTimer >= manager.RefreshInterval())
        {
            charTimer = 0f;
            ShuffleChars();
            Render();
        }
    }

    private void RegenerateColumn()
    {
        int length = manager.RandomLength();
        chars = new char[length];
        for (int i = 0; i < length; i++)
            chars[i] = manager.RandomChar();

        fallSpeed = manager.RandomSpeed();
        Render();
    }

    private void ShuffleChars()
    {
        float boost = manager.GlitchChanceBoost;
        for (int i = 0; i < chars.Length; i++)
        {
            float chance = (i == 0 ? 0.6f : 0.08f) + boost;
            if (Random.value < Mathf.Clamp01(chance))
                chars[i] = manager.RandomChar();
        }
    }

    private void Render()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < chars.Length; i++)
        {
            sb.Append(chars[i]);
            if (i < chars.Length - 1)
                sb.Append('\n');
        }
        tmp.text = sb.ToString();
    }
}