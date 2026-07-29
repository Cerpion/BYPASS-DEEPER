using System.Collections;
using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject wordPrefab;
    public float spawnInterval = 2f;
    public float xMin = -5f;
    public float xMax = 5f;
    public float spawnY = 6f;

    [Header("Palabras de la Deep Web")]
    public string[] wordBank = { 
        "SUDO", "BYPASS", "ROOT", "PROXY", "FIREWALL", 
        "DARK", "NODE", "HASH", "ENCRYPT", "DEEP" 
    };

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
            yield break; 
        }

        float currentInterval = spawnInterval;
        if (GameManager.Instance != null)
        {
            currentInterval = Mathf.Max(0.8f, spawnInterval - (GameManager.Instance.depthLevel * 0.2f));
        }

        yield return new WaitForSeconds(currentInterval);
        SpawnWord();
    }
}
    private void SpawnWord()
    {
        if (wordPrefab == null || wordBank.Length == 0) return;

        float randomX = Random.Range(xMin, xMax);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

        GameObject newWordObj = Instantiate(wordPrefab, spawnPos, Quaternion.identity);
        WordNode node = newWordObj.GetComponent<WordNode>();

        string randomWord = wordBank[Random.Range(0, wordBank.Length)];
        node.SetWord(randomWord);
    }
}