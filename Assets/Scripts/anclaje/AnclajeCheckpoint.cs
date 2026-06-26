using UnityEngine;
using UnityEngine.SceneManagement;

public class AnclajeCheckpoint : MonoBehaviour
{
    [Header("Interacción")]
    public KeyCode teclaActivar = KeyCode.R;
    public GameObject textoActivar;

    [Header("Respawn")]
    public Transform puntoRespawn;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoActivacion;

    private bool jugadorCerca = false;
    private bool activado = false;

    private PlayerHealth playerHealth;
    private Animator animador;

    void Start()
    {
        animador = GetComponent<Animator>();

        if (animador != null)
            animador.enabled = false;

        if (textoActivar != null)
            textoActivar.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (jugadorCerca && !activado && Input.GetKeyDown(teclaActivar))
        {
            ActivarAnclaje();
        }
    }

    void ActivarAnclaje()
    {
        activado = true;

        if (textoActivar != null)
            textoActivar.SetActive(false);

        if (animador != null)
        {
            animador.enabled = true;
            animador.Play("Anclaje", 0, 0f);
        }

        if (audioSource != null && sonidoActivacion != null)
            audioSource.PlayOneShot(sonidoActivacion);

        if (playerHealth != null)
        {
            Transform destino = puntoRespawn != null ? puntoRespawn : transform;

            playerHealth.respawnPoint = destino;

            CheckpointManager.GuardarCheckpoint(
                SceneManager.GetActiveScene().name,
                destino.position
            );

            Debug.Log("Checkpoint guardado en: " + destino.position);
            Debug.Log("Escena checkpoint: " + SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.LogError("NO encontró el PlayerHealth.");
        }

        jugadorCerca = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activado) return;

        PlayerHealth healthDetectada = other.GetComponent<PlayerHealth>();

        if (healthDetectada == null)
            healthDetectada = other.GetComponentInParent<PlayerHealth>();

        if (healthDetectada == null)
            healthDetectada = other.GetComponentInChildren<PlayerHealth>();

        if (healthDetectada != null)
        {
            jugadorCerca = true;
            playerHealth = healthDetectada;

            if (textoActivar != null)
                textoActivar.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerHealth healthDetectada = other.GetComponent<PlayerHealth>();

        if (healthDetectada == null)
            healthDetectada = other.GetComponentInParent<PlayerHealth>();

        if (healthDetectada == null)
            healthDetectada = other.GetComponentInChildren<PlayerHealth>();

        if (healthDetectada != null)
        {
            jugadorCerca = false;

            if (textoActivar != null)
                textoActivar.SetActive(false);
        }
    }
}