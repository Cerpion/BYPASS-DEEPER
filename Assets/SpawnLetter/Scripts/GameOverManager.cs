using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("Referencias UI")]
    public CanvasGroup gameOverCanvasGroup;
    public GameObject _mainButton;
    public Button botonReiniciar;
    public Button botonSalir;

    [Header("Tiempo")]
    public TMP_Text finalTimeText;
    public TMP_Text bestTimeText;

    [Header("Efectos IP / Glitch")]
    public ZalgoGlitchText zalgoText;
    public FakeIPGenerator fakeIpGenerator;

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

        botonReiniciar.onClick.AddListener(ReiniciarNivel);
        botonSalir.onClick.AddListener(SalirNivel);
    }

    public void ActivarGameOver()
    {
        Debug.Log("--- ACTIVANDO GAME OVER ---");

        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 1f;
            gameOverCanvasGroup.interactable = true;
            gameOverCanvasGroup.blocksRaycasts = true;
            _mainButton.gameObject.SetActive(true);

        }

        if (zalgoText != null) zalgoText.TriggerGlitch();
        if (fakeIpGenerator != null) fakeIpGenerator.TriggerReveal();

        LimpiarPalabras();

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

    /// <summary>
    /// Destroys every currently-falling word so the game over screen isn't
    /// cluttered behind it. Relies on word prefabs being tagged "Word".
    /// </summary>
    private void LimpiarPalabras()
    {
        GameObject[] palabrasActivas = GameObject.FindGameObjectsWithTag("Word");
        foreach (GameObject palabra in palabrasActivas)
        {
            Destroy(palabra);
        }
    }

    public void ReiniciarNivel()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
        ChangeSceneController.Instance.ReloadCurrentScene();
    }

    public void SalirNivel()
    {
        Time.timeScale = 1f;
        ChangeSceneController.Instance.LoadSceneByIndex(0);
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