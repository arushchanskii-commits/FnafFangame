using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeSpeed = 2f;
    
    private Color currentColor;
    private bool isFading = false;
    private float fadeTarget = 0f;
    
    private void Start()
    {
        if (fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
        }
        
        if (fadeImage != null)
        {
            fadeImage.color = Color.clear;
            fadeImage.raycastTarget = false;
            currentColor = Color.clear;
        }
    }
    
    private void Update()
    {
        if (isFading)
        {
            currentColor.a = Mathf.Lerp(currentColor.a, fadeTarget, fadeSpeed * Time.deltaTime);
            
            if (fadeImage != null)
            {
                fadeImage.color = currentColor;
            }
            
            if (Mathf.Abs(currentColor.a - fadeTarget) < 0.01f)
            {
                isFading = false;
            }
        }
    }
    
    public void FadeToBlack(float duration = 1f)
    {
        StartCoroutine(FadeToBlackCoroutine(duration));
    }
    
    public void FadeToClear(float duration = 1f)
    {
        StartCoroutine(FadeToClearCoroutine(duration));
    }
    
    private System.Collections.IEnumerator FadeToBlackCoroutine(float duration)
    {
        isFading = true;
        fadeTarget = 1f;
        float startTime = Time.time;
        
        while (Time.time - startTime < duration)
        {
            float alpha = Mathf.Clamp01((Time.time - startTime) / duration);
            currentColor.a = alpha;
            
            if (fadeImage != null)
            {
                fadeImage.color = currentColor;
            }
            
            yield return null;
        }
        
        currentColor.a = 1f;
        if (fadeImage != null)
        {
            fadeImage.color = currentColor;
        }
        
        isFading = false;
    }
    
    private System.Collections.IEnumerator FadeToClearCoroutine(float duration)
    {
        isFading = true;
        fadeTarget = 0f;
        float startTime = Time.time;
        
        while (Time.time - startTime < duration)
        {
            float alpha = Mathf.Clamp01(1f - (Time.time - startTime) / duration);
            currentColor.a = alpha;
            
            if (fadeImage != null)
            {
                fadeImage.color = currentColor;
            }
            
            yield return null;
        }
        
        currentColor.a = 0f;
        if (fadeImage != null)
        {
            fadeImage.color = currentColor;
        }
        
        isFading = false;
    }
    
    public void SetBlack()
    {
        if (fadeImage != null)
        {
            fadeImage.color = Color.black;
            currentColor = Color.black;
        }
    }
    
    public void SetClear()
    {
        if (fadeImage != null)
        {
            fadeImage.color = Color.clear;
            currentColor = Color.clear;
        }
    }
}
