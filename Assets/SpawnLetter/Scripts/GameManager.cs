using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Estadísticas")]
    public int score = 0;
    public int lives = 3;
    public int depthLevel = 1;

    [Header("Estado del Juego")]
    public bool isGameOver = false;

    [Header("UI (Opcional por ahora)")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text depthText;

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

    public void AddScore(int points)
    {
        if (isGameOver) return;

        score += points;
        
        if (score >= depthLevel * 50)
        {
            depthLevel++;
            Debug.Log($"¡Descendiendo a la Capa {depthLevel}!");
        }

        UpdateUI();
    }

    public void TakeDamage(int damage = 1)
    {
        if (isGameOver) return;

        lives -= damage;
        Debug.Log($"¡Integridad comprometida! Vidas restantes: {lives}");
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
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log(">>> SYSTEM FAILURE - GAME OVER <<<");
        Time.timeScale = 0f; 
    }
}