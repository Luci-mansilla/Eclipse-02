using UnityEngine;

public class FragmentoOfensivo : MonoBehaviour
{
    [Header("Flotación")]
    public float floatSpeed = 1.5f;
    public float floatHeight = 0.2f;
    public float tiltAmount = 5f;
    public float tiltSpeed = 1f;

    [Header("Interacción")]
    public KeyCode teclaActivar = KeyCode.E;
    public GameObject textoActivar;

    [Header("Potenciador de ataque")]
    public int aumentoDanio = 10;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoActivacion;

    private Vector3 startPos;
    private bool jugadorCerca = false;
    private bool activado = false;

    private Player_Combat playerCombat;
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

        // Aumentar daño
        if (playerCombat != null)
        {
            playerCombat.damage += aumentoDanio;

            Debug.Log("Fragmento ofensivo activado.");
            Debug.Log("Nuevo daño: " + playerCombat.damage);
        }
        else
        {
            Debug.LogWarning("No se encontró Player_Combat.");
        }

        jugadorCerca = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activado) return;

        Player_Combat combatDetectado =
            other.GetComponent<Player_Combat>();

        if (combatDetectado == null)
            combatDetectado =
                other.GetComponentInParent<Player_Combat>();

        if (combatDetectado == null)
            combatDetectado =
                other.GetComponentInChildren<Player_Combat>();

        if (combatDetectado != null)
        {
            jugadorCerca = true;
            playerCombat = combatDetectado;

            if (textoActivar != null)
                textoActivar.SetActive(true);

            Debug.Log("Jugador detectado por el fragmento.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Player_Combat combatDetectado =
            other.GetComponent<Player_Combat>();

        if (combatDetectado == null)
            combatDetectado =
                other.GetComponentInParent<Player_Combat>();

        if (combatDetectado == null)
            combatDetectado =
                other.GetComponentInChildren<Player_Combat>();

        if (combatDetectado != null)
        {
            jugadorCerca = false;

            if (textoActivar != null)
                textoActivar.SetActive(false);
        }
    }
}