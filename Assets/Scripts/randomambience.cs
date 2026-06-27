using UnityEngine;

public class randomambience : MonoBehaviour
{
    [Header("Ambient (loop)")]
    public AudioClip constantClip;
    public float constantVolume = 0.5f;
    public bool playOnStart = true;

    [Header("Random SFX")]
    public AudioClip[] randomClips;
    public float randomMinInterval = 5f;
    public float randomMaxInterval = 20f;
    public float randomVolume = 1f;

    private AudioSource ambientSource;
    private AudioSource sfxSource;
    private float nextRandomTime;

    void Awake()
    {
        ambientSource = GetComponent<AudioSource>();
        if (ambientSource == null)
            ambientSource = gameObject.AddComponent<AudioSource>();

        ambientSource.loop = true;
        ambientSource.playOnAwake = false;
        ambientSource.volume = constantVolume;
        if (constantClip != null)
            ambientSource.clip = constantClip;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    }

    void Start()
    {
        ScheduleNextRandom();
        if (playOnStart && constantClip != null)
            ambientSource.Play();
    }

    void Update()
    {
        if (randomClips != null && randomClips.Length > 0 && Time.time >= nextRandomTime)
        {
            PlayRandomClip();
            ScheduleNextRandom();
        }
    }

    private void PlayRandomClip()
    {
        if (randomClips == null || randomClips.Length == 0)
            return;

        AudioClip clip = randomClips[Random.Range(0, randomClips.Length)];
        if (clip != null)
            sfxSource.PlayOneShot(clip, randomVolume);
    }

    private void ScheduleNextRandom()
    {
        nextRandomTime = Time.time + Random.Range(randomMinInterval, randomMaxInterval);
    }

    public void SetAmbientPlaying(bool play)
    {
        if (play)
        {
            if (!ambientSource.isPlaying && ambientSource.clip != null)
                ambientSource.Play();
        }
        else
        {
            if (ambientSource.isPlaying)
                ambientSource.Stop();
        }
    }
}
