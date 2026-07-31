using UnityEngine;

public class FadeController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    private const float MAX_FADE = 1f;
    private const float MIN_FADE = 0f;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        _canvasGroup.alpha = 0f;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public LTDescr FadeIn(float duration)
    {
        return LeanTween.alphaCanvas(_canvasGroup, MAX_FADE, duration);
    }

    public LTDescr FadeOut(float duration)
    {
        return LeanTween.alphaCanvas(_canvasGroup, MIN_FADE, duration);
    }
}