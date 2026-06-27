using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class menumusic : MonoBehaviour
{
    public AudioClip targetSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.clip = targetSound;
        if (audioSource.clip == null)
        {
            Debug.LogWarning("menumusic: No targetSound AudioClip assigned.");
            return;
        }

        UpdatePlaybackState();
        SceneManager.sceneLoaded += OnSceneChanged;
        SceneManager.sceneUnloaded += OnSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneChanged;
        SceneManager.sceneUnloaded -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        UpdatePlaybackState();
    }

    private void OnSceneChanged(Scene scene)
    {
        UpdatePlaybackState();
    }

    private void UpdatePlaybackState()
    {
        if (SceneManager.sceneCount == 1)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
