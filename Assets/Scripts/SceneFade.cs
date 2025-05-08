using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class SceneFade : MonoBehaviour
{
    public Image fadeImage; 
    public float fadeDuration = 0.3f;
    void Start()
    {
        // Ensure fully transparent at start
        SetAlpha(0f);
    }
    public IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            SetAlpha(Mathf.Lerp(0f, 1f, elapsed / fadeDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetAlpha(1f);
    }
    public IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            SetAlpha(Mathf.Lerp(1f, 0f, elapsed / fadeDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetAlpha(0f);
    }
    private void SetAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}