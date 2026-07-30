using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] private TMP_Text timeText;
    [SerializeField] private float startTime = 60f;

    private float currentTime;
    private bool gameEnded;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = startTime;
        UpdateUI();
    }

    void Update()
    {
        if (gameEnded)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            GameOver();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        timeText.text = "Time: " + Mathf.CeilToInt(currentTime);
    }

    public void RemoveTime(float amount)
    {
        if (gameEnded)
            return;

        currentTime -= amount;

        if (currentTime <= 0)
        {
            currentTime = 0;
            GameOver();
        }

        UpdateUI();
    }

    void GameOver()
    {
        gameEnded = true;
        Debug.Log("GAME OVER");
    }
}