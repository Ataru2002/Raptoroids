using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingEffect : MonoBehaviour
{
    public float fadeDuration = 1f; // Duration of fade in seconds
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void tryFadeIn(){
        StartCoroutine(FadeIn());
    }
    public void tryFadeOut(){
        StartCoroutine(FadeOut());
    }

    // Update is called once per frame
    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return new WaitForEndOfFrame();
        }
    }
}
