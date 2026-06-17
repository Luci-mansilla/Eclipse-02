using UnityEngine;

public class Musica : MonoBehaviour
{
    public AudioClip clickAudio;
    public AudioClip switchAudio;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ClickAudioOn()
    {
        audioSource.PlayOneShot(clickAudio);
    }

    public void SwitchAudioOn()
    {
        audioSource.PlayOneShot(switchAudio);
    }
}
