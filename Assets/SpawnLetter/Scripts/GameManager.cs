using System.Collections;
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

    [Header("UI General")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text depthText;
    public TMP_Text comboText;

    [Header("UI de Capas (Deep Web)")]
    public TMP_Text layerAnnouncementText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        isGameOver = false;
        Time.timeScale = 1f;

        if (layerAnnouncementText != null)
        {
            layerAnnouncementText.gameObject.SetActive(false);
            StartCoroutine(AnnounceLayerRoutine());
        }

        UpdateUI();
    }

    public float GetDifficultyMultiplier()
    {
        if (depthLevel == 1) return 0.25f;
        if (depthLevel == 2) return 0.50f;
        if (depthLevel == 3) return 0.75f;
        return 1.0f;
    }

    public void AddScore(int basePoints)
    {
        if (isGameOver) return;

        currentCombo++;
        comboMultiplier = 1 + (currentCombo / 5);
        score += (basePoints * comboMultiplier);

        if (score >= depthLevel * 100 && depthLevel < 4)
        {
            depthLevel++;
            if (layerAnnouncementText != null)
            {
                StartCoroutine(AnnounceLayerRoutine());
            }
            else
            {
                Debug.Log($"¡AVANZANDO A CAPA -{depthLevel}!");
            }
        }

        UpdateUI();
    }

    public void ResetCombo()
    {
        if (currentCombo > 0)
        {
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
        if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.3f, 0.4f);

        UpdateUI();
        if (lives <= 0) GameOver();
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"SCORE: {score}";
        if (livesText != null) livesText.text = $"INTEGRITY: {lives}";
        if (depthText != null) depthText.text = $"LAYER: -{depthLevel}";
        if (comboText != null) comboText.text = $"COMBO: x{comboMultiplier}";
    }

  private IEnumerator AnnounceLayerRoutine()
    {
        if (layerAnnouncementText == null) yield break;

        layerAnnouncementText.text = $"LAYER -{depthLevel}\n<size=50%>{(GetDifficultyMultiplier() * 100)}% SPEED</size>";
        layerAnnouncementText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        layerAnnouncementText.gameObject.SetActive(false);
    } 

    private void GameOver()
{
    isGameOver = true;

    Debug.LogError("<color=red><size=20>!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!</size></color>");
    Debug.LogError("<color=red><size=25>>>> CYBER-LIFE TERMINATED - GAME OVER <<<</size></color>");
    Debug.LogError("<color=red><size=20>!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!</size></color>");

    if (GameOverManager.Instance != null)
    {
        GameOverManager.Instance.ActivarGameOver();
    }
}
} 
