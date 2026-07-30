using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("Referencias UI")]
    public CanvasGroup gameOverCanvasGroup;
    public Button botonReiniciar;

    [Header("Tiempo")]
    public TMP_Text finalTimeText;
    public TMP_Text bestTimeText;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        OcultarGameOver();

        if (botonReiniciar != null)
        {
            botonReiniciar.onClick.AddListener(ReiniciarNivel);
        }
    }

    public void ActivarGameOver()
    {
        Debug.Log("--- ACTIVANDO GAME OVER ---");

        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 1f;
            gameOverCanvasGroup.interactable = true;
            gameOverCanvasGroup.blocksRaycasts = true;
        }

        Time.timeScale = 0f;
    }

    private void OcultarGameOver()
    {
        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 0f;
            gameOverCanvasGroup.interactable = false;
            gameOverCanvasGroup.blocksRaycasts = false;
        }
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ===============================
    // NUEVO: Mostrar tiempos
    // ===============================

    public void SetTimes(float currentTime, float bestTime)
    {
        if (finalTimeText != null)
        {
            finalTimeText.text = "SESSION TIME\n" + FormatTime(currentTime);
        }

        if (bestTimeText != null)
        {
            bestTimeText.text = "BEST TIME\n" + FormatTime(bestTime);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}