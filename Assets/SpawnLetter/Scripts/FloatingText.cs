using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fadeDuration = 0.8f;
    private TMP_Text tmpText;
    private Color textColor;
    private float timer = 0f;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            textColor = tmpText.color;
        }
    }

    public void SetText(string text)
    {
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = text;
        }
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (tmpText != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            tmpText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
        }

        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}