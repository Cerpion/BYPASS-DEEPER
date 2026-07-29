using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneController : MonoBehaviour
{
    public static ChangeSceneController Instance { get; private set; }

    [SerializeField] private FadeController _fadeController;

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
        var sequence = LeanTween.sequence();
        sequence.append(_fadeController.Show);
        sequence.append(_fadeController.FadeIn(fadeTime));
        sequence.append(() => { SceneManager.LoadScene(sceneToLoad); });
        sequence.append(fadeWait);
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
