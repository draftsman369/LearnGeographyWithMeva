using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;

    public AudioClip positiveFeedback;
    public AudioClip negativeFeedback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPositiveFeedback()
    {
        if (positiveFeedback != null)
            audioSource.PlayOneShot(positiveFeedback);
    }

    public void PlayNegativeFeedback()
    {
        if (negativeFeedback != null)
            audioSource.PlayOneShot(negativeFeedback);
    }
}