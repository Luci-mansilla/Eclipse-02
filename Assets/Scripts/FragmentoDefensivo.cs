using UnityEngine;

public class FragmentoDefensivo : MonoBehaviour
{
    [Header("Flotación")]
    public float floatSpeed = 1.5f;
    public float floatHeight = 0.2f;
    public float tiltAmount = 5f;
    public float tiltSpeed = 1f;

    [Header("Interacción")]
    public KeyCode teclaActivar = KeyCode.E;
    public GameObject textoActivar;

    [Header("Potenciador defensivo")]
    [Range(0f, 1f)]
    public float reduccionDanio = 0.5f;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoActivacion;

    private Vector3 startPos;
    private bool jugadorCerca = false;
    private bool activado = false;

    private PlayerHealth playerHealth;
    private Animator animador;

    void Start()
    {
        startPos = transform.position;

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
        Flotar();

        if (jugadorCerca && !activado)
        {
            if (Input.GetKeyDown(teclaActivar))
            {
                ActivarFragmento();
            }
        }
    }

    void Flotar()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            startPos.x,
            newY,
            startPos.z
        );

        float tiltZ = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;

        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            tiltZ
        );
    }

    void ActivarFragmento()
    {
        activado = true;

        if (textoActivar != null)
            textoActivar.SetActive(false);

        // Activar animación
        if (animador != null)
        {
            animador.enabled = true;
        }

        // Reproducir sonido
        if (audioSource != null && sonidoActivacion != null)
        {
            audioSource.PlayOneShot(sonidoActivacion);
        }

        // Reducir daño recibido
        if (playerHealth != null)
        {
            playerHealth.damageReduction = reduccionDanio;

            Debug.Log("Fragmento defensivo activado.");
            Debug.Log("Reducción de daño: " + playerHealth.damageReduction);
        }
        else
        {
            Debug.LogWarning("No se encontró PlayerHealth.");
        }

        jugadorCerca = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activado) return;

        PlayerHealth healthDetectada =
            other.GetComponent<PlayerHealth>();

        if (healthDetectada == null)
            healthDetectada =
                other.GetComponentInParent<PlayerHealth>();

        if (healthDetectada == null)
            healthDetectada =
                other.GetComponentInChildren<PlayerHealth>();

        if (healthDetectada != null)
        {
            jugadorCerca = true;
            playerHealth = healthDetectada;

            if (textoActivar != null)
                textoActivar.SetActive(true);

            Debug.Log("Jugador detectado por el fragmento defensivo.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerHealth healthDetectada =
            other.GetComponent<PlayerHealth>();

        if (healthDetectada == null)
            healthDetectada =
                other.GetComponentInParent<PlayerHealth>();

        if (healthDetectada == null)
            healthDetectada =
                other.GetComponentInChildren<PlayerHealth>();

        if (healthDetectada != null)
        {
            jugadorCerca = false;

            if (textoActivar != null)
                textoActivar.SetActive(false);
        }
    }
}