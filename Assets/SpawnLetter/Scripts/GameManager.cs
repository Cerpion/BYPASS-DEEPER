using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Estadísticas")]
    public int score = 0;
    public int lives = 3;
    public int depthLevel = 1;

    [Header("Sistema de Combo")]
    public int currentCombo = 0;
    public int comboMultiplier = 1;

    [Header("Estado del Juego")]
    public bool isGameOver = false;

    [Header("UI (Opcional)")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text depthText;
    public TMP_Text comboText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        UpdateUI();
    }

    public void AddScore(int basePoints)
    {
        if (isGameOver) return;
        currentCombo++;
        comboMultiplier = 1 + (currentCombo / 5); 
        int finalPoints = basePoints * comboMultiplier;
        score += finalPoints;

        if (score >= depthLevel * 50)
        {
            depthLevel++;
            Debug.Log($"¡Descendiendo a la Capa {depthLevel}!");
        }

        UpdateUI();
    }

    public void ResetCombo()
    {
        if (currentCombo > 0)
        {
            Debug.Log("¡Combo perdido!");
            currentCombo = 0;
            comboMultiplier = 1;
            UpdateUI();
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (isGameOver) return;

        ResetCombo();
        lives -= damage;

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.3f, 0.4f);
        }

        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"SCORE: {score}";
        if (livesText != null) livesText.text = $"INTEGRITY: {lives}";
        if (depthText != null) depthText.text = $"LAYER: -{depthLevel}";
        if (comboText != null) comboText.text = $"COMBO: x{comboMultiplier}";
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log(">>> SYSTEM FAILURE - GAME OVER <<<");
        Time.timeScale = 0f;
    }
}