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
    private GameObject jugador;
    private Animator animador;

    void Start()
    {
        startPos = transform.position;

        animador = GetComponent<Animator>();

        if (animador != null)
            animador.enabled = false;

        if (textoActivar != null)
            textoActivar.SetActive(false);
    }

    void Update()
    {
        Flotar();

        if (jugadorCerca && !activado && Input.GetKeyDown(teclaActivar))
        {
            ActivarFragmento();
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

        transform.rotation = Quaternion.Euler(0f, 0f, tiltZ);
    }

    void ActivarFragmento()
    {
        activado = true;

        if (textoActivar != null)
            textoActivar.SetActive(false);

        // Activa la animación
        if (animador != null)
        {
            animador.enabled = true;
            animador.Play("Fragmento ofensivo", 0, 0f);
        }

        // Reproduce sonido
        if (audioSource != null && sonidoActivacion != null)
        {
            audioSource.PlayOneShot(sonidoActivacion);
        }

        // Aumenta daño
        Player_Combat playerCombat = jugador.GetComponent<Player_Combat>();

        if (playerCombat != null)
        {
            playerCombat.damage += aumentoDanio;

            Debug.Log("Fragmento ofensivo activado.");
            Debug.Log("Nuevo daño: " + playerCombat.damage);
        }

        // Evita que vuelva a activarse
        jugadorCerca = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (activado) return;

            jugadorCerca = true;
            jugador = other.gameObject;

            if (textoActivar != null)
                textoActivar.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            if (textoActivar != null)
                textoActivar.SetActive(false);
        }
    }
}