using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Formerly a ship that flew to and shot the target word. Now a small "terminal"
/// that follows below the targeted word and echoes typed input like a command
/// prompt (e.g. "> HAC" while typing "HACK"). Public API (SetTarget/GetTarget/
/// ClearTarget/Shoot) is kept intact for compatibility with whatever input
/// manager currently drives this — only the internals and visuals changed.
/// </summary>
public class ShipController : MonoBehaviour
{
    public static ShipController Instance;

    [Header("Movimiento")]
    public float moveSpeed = 8f;

    [Tooltip("Distancia que el terminal mantiene debajo de la palabra")]
    public float followOffset = 1.8f;

    [Header("Terminal / Prompt")]
    public TMP_Text terminalText;
    public string promptPrefix = "> ";
    public string idlePrompt = "> _";
    public Color typedColor = new Color(0.35f, 1f, 0.45f);   // hacked-green for confirmed letters
    public Color remainingColor = new Color(0.5f, 0.5f, 0.5f); // dim gray for untyped letters
    public float cursorBlinkInterval = 0.4f;

    private Transform currentTarget;
    private string lastTyped = "";
    private string lastFull = "";

    private bool cursorVisible = true;
    private float cursorTimer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshDisplay();
    }

    private void Update()
    {
        cursorTimer += Time.deltaTime;
        if (cursorTimer >= cursorBlinkInterval)
        {
            cursorTimer = 0f;
            cursorVisible = !cursorVisible;
            RefreshDisplay();
        }

        if (currentTarget == null)
            return;

        // Mantener el terminal debajo de la palabra
        Vector3 destination = currentTarget.position + Vector3.down * followOffset;
        destination.z = transform.position.z;

        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            moveSpeed * Time.deltaTime
        );
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
        lastTyped = "";
        lastFull = "";
        RefreshDisplay();
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }

    public void ClearTarget()
    {
        currentTarget = null;
        lastTyped = "";
        lastFull = "";
        RefreshDisplay();
    }

    /// <summary>
    /// Call this whenever the targeted word's typed progress changes, so the
    /// terminal echoes it (e.g. from WordNode as each correct letter lands).
    /// </summary>
    public void UpdateTypedText(string typedSoFar, string fullWord)
    {
        lastTyped = typedSoFar;
        lastFull = fullWord;
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (terminalText == null) return;

        if (string.IsNullOrEmpty(lastFull))
        {
            terminalText.text = cursorVisible ? idlePrompt : idlePrompt.Replace("_", " ");
            return;
        }

        string typedHex = ColorUtility.ToHtmlStringRGB(typedColor);
        string remainingHex = ColorUtility.ToHtmlStringRGB(remainingColor);
        string remaining = lastFull.Length > lastTyped.Length ? lastFull.Substring(lastTyped.Length) : "";
        string cursor = cursorVisible ? "_" : " ";

        terminalText.text = $"{promptPrefix}<color=#{typedHex}>{lastTyped}</color><color=#{remainingHex}>{remaining}</color>{cursor}";
    }

    /// <summary>
    /// Kept for compatibility with any existing code that calls Shoot() on word
    /// completion. Now plays a brief "[OK]" confirmation flash instead of firing
    /// a projectile, then resets to idle.
    /// </summary>
    public void Shoot()
    {
        if (terminalText == null) return;
        StopAllCoroutines();
        StartCoroutine(ConfirmFlash());
    }

    private IEnumerator ConfirmFlash()
    {
        string flashHex = ColorUtility.ToHtmlStringRGB(typedColor);
        terminalText.text = $"<color=#{flashHex}>{promptPrefix}{lastFull} [OK]</color>";
        yield return new WaitForSeconds(0.15f);
        ClearTarget();
    }
}