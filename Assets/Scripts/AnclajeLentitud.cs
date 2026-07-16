using UnityEngine;

public class AnclajeLentitud : MonoBehaviour
{
    [Header("Interacción")]
    public KeyCode teclaActivar = KeyCode.R;
    public GameObject textoActivar;

    [Header("Efecto de lentitud")]
    [Range(0.05f, 1f)]
    public float multiplicadorLentitud = 0.25f;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoActivacion;

    private bool jugadorCerca = false;
    private bool activado = false;

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

        // Ocultar el texto de interacción
        if (textoActivar != null)
            textoActivar.SetActive(false);

        // Reproducir la animación
        if (animador != null)
        {
            animador.enabled = true;
            animador.Play("Anclaje", 0, 0f);
        }

        // Reproducir el sonido
        if (audioSource != null && sonidoActivacion != null)
            audioSource.PlayOneShot(sonidoActivacion);

        // Buscar todos los enemigos que tengan EnemySlowEffect
        EnemySlowEffect[] enemigos = FindObjectsByType<EnemySlowEffect>(
            FindObjectsSortMode.None
        );

        foreach (EnemySlowEffect enemigo in enemigos)
        {
            enemigo.ActivarLentitud(multiplicadorLentitud);
        }

        Debug.Log(
            "Anclaje de lentitud activado. Enemigos afectados: "
            + enemigos.Length
        );

        jugadorCerca = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activado)
            return;

        // Detectar al jugador aunque el collider esté en un hijo
        PlayerHealth jugador = other.GetComponent<PlayerHealth>();

        if (jugador == null)
            jugador = other.GetComponentInParent<PlayerHealth>();

        if (jugador == null)
            jugador = other.GetComponentInChildren<PlayerHealth>();

        if (jugador != null)
        {
            jugadorCerca = true;

            if (textoActivar != null)
                textoActivar.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerHealth jugador = other.GetComponent<PlayerHealth>();

        if (jugador == null)
            jugador = other.GetComponentInParent<PlayerHealth>();

        if (jugador == null)
            jugador = other.GetComponentInChildren<PlayerHealth>();

        if (jugador != null)
        {
            jugadorCerca = false;

            if (textoActivar != null)
                textoActivar.SetActive(false);
        }
    }
}