using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Displays a randomly generated (fake) IP address below the game over message,
/// as if the player's IP had just been "revealed" — it's never the real IP.
/// Attach to an empty object or the TMP_Text itself. Automatically generates a
/// new fake IP each time it's enabled (e.g. when the game over panel activates),
/// with an optional quick-scramble reveal animation for a hacking-movie feel.
/// </summary>
public class FakeIPGenerator : MonoBehaviour
{
    [Header("Setup")]
    public TMP_Text ipText;
    public string prefix = "IP RASTREADA: ";

    [Header("Reveal Animation")]
    public bool animateReveal = true;
    public float revealDuration = 0.6f;
    public float revealTickInterval = 0.04f;

    private Coroutine revealCoroutine;

    private void OnEnable()
    {
        TriggerReveal();
    }

    /// <summary>
    /// Call this explicitly when the game over screen activates (e.g. from
    /// GameOverManager.ActivarGameOver()) since the panel likely stays "enabled"
    /// the whole time (shown/hidden via CanvasGroup alpha), so OnEnable alone
    /// won't refire on repeat game overs.
    /// </summary>
    public void TriggerReveal()
    {
        if (revealCoroutine != null) StopCoroutine(revealCoroutine);

        if (animateReveal)
            revealCoroutine = StartCoroutine(RevealRoutine());
        else
            GenerateNewIP();
    }

    private IEnumerator RevealRoutine()
    {
        float elapsed = 0f;
        while (elapsed < revealDuration)
        {
            SetRandomIP();
            elapsed += revealTickInterval;
            yield return new WaitForSecondsRealtime(revealTickInterval);
        }

        SetRandomIP(); // final settled value
        revealCoroutine = null;
    }

    /// <summary>Generates and displays a new random (fake) IP immediately, no animation.</summary>
    public void GenerateNewIP()
    {
        SetRandomIP();
    }

    private void SetRandomIP()
    {
        int a = Random.Range(1, 255);
        int b = Random.Range(0, 255);
        int c = Random.Range(0, 255);
        int d = Random.Range(1, 255);

        if (ipText != null)
            ipText.text = $"{prefix}{a}.{b}.{c}.{d}";
    }
}