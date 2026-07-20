using UnityEngine;
using UnityEngine.UI;

public class SpriteCycler : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] sprites;
    
    [Header("Speed Settings")]
    public float frameRate = 10f;
    
    private SpriteRenderer spriteRenderer;
    private Image uiImage;
    private int currentFrame = 0;
    private float timer = 0f;
    private bool playingReverse = false;
    private bool isPlaying = false;
    private bool isUI = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiImage = GetComponent<Image>();
        
        if (spriteRenderer == null && uiImage == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        isUI = (uiImage != null);
        
        if (sprites.Length > 0)
        {
            if (isUI && uiImage != null)
            {
                uiImage.sprite = sprites[0];
            }
            else if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprites[0];
            }
        }
    }

    private void Update()
    {
        if (sprites.Length <= 1 || !isPlaying) return;
        
        timer += Time.deltaTime;
        float frameTime = 1f / frameRate;
        
        if (timer >= frameTime)
        {
            timer = 0f;
            
            if (playingReverse)
            {
                currentFrame--;
                if (currentFrame < 0)
                {
                    currentFrame = 0;
                    isPlaying = false;
                }
            }
            else
            {
                currentFrame++;
                if (currentFrame >= sprites.Length)
                {
                    currentFrame = sprites.Length - 1;
                    isPlaying = false;
                }
            }
            
            if (isUI && uiImage != null)
            {
                uiImage.sprite = sprites[currentFrame];
            }
            else if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprites[currentFrame];
            }
        }
    }

    public void PlayForward()
    {
        if (sprites.Length == 0) return;
        
        if (spriteRenderer == null && uiImage == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        if (uiImage == null)
        {
            uiImage = GetComponent<Image>();
        }
        
        isUI = (uiImage != null);
        
        playingReverse = false;
        isPlaying = true;
        currentFrame = 0;
        timer = 0f;
        
        if (isUI && uiImage != null)
        {
            uiImage.sprite = sprites[0];
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprites[0];
        }
    }

    public void PlayReverse()
    {
        if (sprites.Length == 0) return;
        
        if (spriteRenderer == null && uiImage == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        if (uiImage == null)
        {
            uiImage = GetComponent<Image>();
        }
        
        isUI = (uiImage != null);
        
        playingReverse = true;
        isPlaying = true;
        currentFrame = sprites.Length - 1;
        timer = 0f;
        
        if (isUI && uiImage != null)
        {
            uiImage.sprite = sprites[currentFrame];
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprites[currentFrame];
        }
    }

    public float GetAnimationLength()
    {
        if (sprites.Length == 0) return 0f;
        return sprites.Length / frameRate;
    }

    public void Stop()
    {
        isPlaying = false;
    }
}
