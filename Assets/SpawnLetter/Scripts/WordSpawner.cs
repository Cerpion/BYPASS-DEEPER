using System.Collections;
using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    public GameObject wordPrefab;
    public RectTransform canvas;
    public RectTransform spawnA;
    public RectTransform spawnB;

    [Header("Bancos de Palabras de la Deep Web")]
    public string[] layer1Words = {
        "PING", "DATA", "NODE", "PORT", "HTML", "IP", "MAC", "USER",
        "BYTE", "WIFI", "SCAN", "LINK", "HOST", "BOOT", "DISK", "CODE",
        "FILE", "LAN", "DNS", "USB", "RAM", "KEY", "BUG", "LOG"
    };
    public string[] layer2Words = {
        "PROXY", "SERVER", "PYTHON", "HACKER", "ROUTER", "CACHE", "BOTNET",
        "KERNEL", "SOCKET", "PACKET", "CLUSTER", "DAEMON", "BACKEND",
        "KEYLOG", "SPYWARE", "NEXLINK", "VORTEXNET", "KRAKEN_NET"
    };
    public string[] layer3Words = {
        "ENCRYPT", "FIREWALL", "PROTOCOL", "MALWARE", "SYSTEM", "PHISHING",
        "RANSOMWARE", "KEYLOGGER", "BACKDOOR", "SANDBOX", "EXPLOIT",
        "SPOOFING", "TUNNELING", "QUANTUMKEY", "CIPHERGRID", "DARKNODE"
    };
    public string[] layer4Words = {
        "SQL_INJECTION", "ROOT_ACCESS", "BYPASS_KEY", "OVERRIDE", "MAINFRAME",
        "ZERO_DAY_EXPLOIT", "KERNEL_PANIC", "DEEP_PACKET_SCAN", "GHOST_PROTOCOL",
        "KRYPTON_CIPHER", "VOID_MAINFRAME", "NEXUS_OVERRIDE", "KRONOS_ARRAY"
    };

    [Header("Banco de Power-Ups y Especiales")]
    public string[] healWords = { "PATCH", "FIX", "HEAL", "RECOVER", "RESTORE", "REBOOT", "REBUILD" };
    public string[] freezeWords = { "HALT", "FREEZE", "PAUSE", "SLOW", "LOCK", "STASIS", "DEADLOCK" };
    public string[] glitchWords = { "VIRUS", "CORRUPT", "GLITCH", "ERROR", "TROJAN", "CRASH", "WORM" };

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

        float randomX = Random.Range(0f, 1f);

        var spawnPosition = Vector3.Lerp(spawnA.position, spawnB.position, randomX);

        GameObject newWordObj = Instantiate(wordPrefab, canvas);
        newWordObj.GetComponent<RectTransform>().position = spawnPosition;

        //newWordObj.transform.SetParent(null);

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

            wordNode.SetLimits(canvas.rect.height);
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