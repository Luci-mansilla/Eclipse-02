using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Musica : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Sonidos")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // Cuando el mouse pasa por encima
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    // Cuando hace click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}