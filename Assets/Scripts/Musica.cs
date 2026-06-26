using UnityEngine;

public class Musica : MonoBehaviour
{
    public AudioClip clickAudio;
    public AudioClip switchAudio;
    private AudioSource audioSource;

    void Awake()  // ← Cambiado de Start a Awake, corre antes
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ClickAudioOn()
    {
        if (audioSource == null || clickAudio == null) return; // ← null check de seguridad
        audioSource.PlayOneShot(clickAudio);
    }

    public void SwitchAudioOn()
    {
        if (audioSource == null || switchAudio == null) return; // ← null check de seguridad
        audioSource.PlayOneShot(switchAudio);
    }
}