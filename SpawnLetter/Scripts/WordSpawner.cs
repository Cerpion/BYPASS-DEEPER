using System.Collections;
using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    public GameObject wordPrefab;
    public float xBound = 5f; 

    [Header("Bancos de Palabras de la Deep Web")]
    public string[] layer1Words = { "PING", "DATA", "NODE", "PORT", "HTML", "IP", "MAC", "USER" };
    public string[] layer2Words = { "PROXY", "SERVER", "PYTHON", "HACKER", "ROUTER", "CACHE", "BOTNET" };
    public string[] layer3Words = { "ENCRYPT", "FIREWALL", "PROTOCOL", "MALWARE", "SYSTEM", "PHISHING" };
    public string[] layer4Words = { "SQL_INJECTION", "ROOT_ACCESS", "BYPASS_KEY", "OVERRIDE", "MAINFRAME" };

    [Header("Banco de Power-Ups y Especiales")]
    public string[] healWords = { "PATCH", "FIX", "HEAL", "RECOVER", "RESTORE" };
    public string[] freezeWords = { "HALT", "FREEZE", "PAUSE", "SLOW", "LOCK" };
    public string[] glitchWords = { "VIRUS", "CORRUPT", "GLITCH", "ERROR", "TROJAN" };

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            SpawnWord();

            float difficulty = 0.25f;
            if (GameManager.Instance != null)
            {
                difficulty = GameManager.Instance.GetDifficultyMultiplier();
            }

            float currentInterval = Mathf.Lerp(3.5f, 1f, difficulty);
            yield return new WaitForSeconds(currentInterval);
        }
    }

   private void SpawnWord()
    {
        if (wordPrefab == null)
        {
            Debug.LogError("¡Falta asignar el WordPrefab en el WordSpawner!");
            return;
        }

        float randomX = Random.Range(-xBound, xBound);
        Vector3 spawnPosition = new Vector3(randomX, transform.position.y, 0f);

        GameObject newWordObj = Instantiate(wordPrefab, spawnPosition, Quaternion.identity);
        
        newWordObj.transform.SetParent(null);

        WordNode wordNode = newWordObj.GetComponent<WordNode>();

        if (wordNode != null)
        {
            float roll = Random.value;
            WordType chosenType = WordType.Normal;
            string selectedWord = "DATA";

            if (roll < 0.15f && healWords != null && healWords.Length > 0)
            {
                chosenType = WordType.Heal;
                selectedWord = healWords[Random.Range(0, healWords.Length)];
            }
            else if (roll < 0.30f && freezeWords != null && freezeWords.Length > 0)
            {
                chosenType = WordType.Freeze;
                selectedWord = freezeWords[Random.Range(0, freezeWords.Length)];
            }
            else if (roll < 0.45f && glitchWords != null && glitchWords.Length > 0)
            {
                chosenType = WordType.Glitch;
                selectedWord = glitchWords[Random.Range(0, glitchWords.Length)];
            }
            else
            {
                chosenType = WordType.Normal;
                selectedWord = GetRandomWordForCurrentLayer();
            }

            wordNode.SetWord(selectedWord);
            wordNode.SetupSpecialType(chosenType);
        }
    }
    private string GetRandomWordForCurrentLayer()
    {
        int layer = 1;
        if (GameManager.Instance != null)
        {
            layer = GameManager.Instance.depthLevel;
        }

        switch (layer)
        {
            case 1: 
                return layer1Words.Length > 0 ? layer1Words[Random.Range(0, layer1Words.Length)] : "DATA";
            case 2: 
                return layer2Words.Length > 0 ? layer2Words[Random.Range(0, layer2Words.Length)] : "SERVER";
            case 3: 
                return layer3Words.Length > 0 ? layer3Words[Random.Range(0, layer3Words.Length)] : "SYSTEM";
            case 4: default: 
                return layer4Words.Length > 0 ? layer4Words[Random.Range(0, layer4Words.Length)] : "HACK";
        }
    }
}