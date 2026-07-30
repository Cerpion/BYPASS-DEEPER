using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("UI")]
    public TMP_Text timeText;

    private float survivalTime;
    private bool timerRunning = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        survivalTime += Time.deltaTime;

        UpdateUI();
    }

    void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(survivalTime / 60);
        int seconds = Mathf.FloorToInt(survivalTime % 60);

        timeText.text = $"TIME {minutes:00}:{seconds:00}";
    }

  public void StopTimer()
{
    timerRunning = false;

    Debug.Log("===== STOP TIMER EJECUTADO =====");

    float bestTime = PlayerPrefs.GetFloat("BestTime", 0f);

    Debug.Log("Tiempo actual: " + survivalTime);

    if (survivalTime > bestTime)
    {
        bestTime = survivalTime;
        PlayerPrefs.SetFloat("BestTime", bestTime);
        PlayerPrefs.Save();
    }

    Debug.Log("Mejor tiempo: " + bestTime);

    if (GameOverManager.Instance != null)
    {
        Debug.Log("Enviando tiempos al GameOver");
        GameOverManager.Instance.SetTimes(survivalTime, bestTime);
    }
}

    public float GetCurrentTime()
    {
        return survivalTime;
    }
}