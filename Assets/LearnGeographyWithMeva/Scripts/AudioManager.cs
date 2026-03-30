using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;
    private AudioSource audio;

    public AudioClip positiveFeedback;
    public AudioClip negativeFeedback;

    private void Awake()
    {
        if(Instance != null)
            Destroy(this.gameObject);
        Instance = this;

        audio = this.GetComponent<AudioSource>();
    }

    public void PlayPositiveFeedback()
    {
        audio.PlayOneShot(positiveFeedback);
    }

    public void PlayNegativeFeedback()
    {
        audio.PlayOneShot(negativeFeedback);
    }
}
