using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeSceneController : MonoBehaviour
{
    public static ChangeSceneController Instance { get; private set; }

    [SerializeField] private FadeController _fadeController;
    [SerializeField] private TMP_Text _loadingText;
    [SerializeField] private string[] _loadText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadScene(int sceneToLoad, float fadeTime, float fadeWait)
    {
        _loadingText.text = "";

        var sequence = LeanTween.sequence();
        sequence.append(_fadeController.Show);
        sequence.append(_fadeController.FadeIn(fadeTime));
        sequence.append(() => { SceneManager.LoadScene(sceneToLoad); });


        var timeToText = fadeWait / _loadText.Length;
        foreach (var item in _loadText)
        {
            sequence.append(timeToText);
            sequence.append(() => { _loadingText.text += item; _loadingText.text += '\n'; });
        }

        sequence.append(timeToText);
        sequence.append(_fadeController.FadeOut(fadeTime));
        sequence.append(_fadeController.Hide);
    }

    public void ReloadCurrentScene(float fadeTime = 1f, float fadeWait = 1.5f)
    {
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        LoadScene(sceneIndex, fadeTime, fadeWait);
    }

    public void LoadSceneByIndex(int index , float fadeTime = 1f, float fadeWait = 1.5f)
    {
        LoadScene(index, fadeTime, fadeWait);
    }

}
