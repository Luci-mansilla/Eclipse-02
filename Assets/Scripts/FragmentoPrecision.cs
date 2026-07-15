using UnityEngine;

public class FragmentoPrecision : MonoBehaviour
{
    [Header("Flotación")]
    public float floatSpeed = 1.5f;
    public float floatHeight = 0.2f;
    public float tiltAmount = 5f;
    public float tiltSpeed = 1f;

    [Header("Interacción")]
    public KeyCode teclaActivar = KeyCode.E;
    public GameObject textoActivar;

    [Header("Potenciador de precisión")]
    public float aumentoAlcance = 0.5f;
    public float aumentoRetroceso = 4f;
    public float aumentoDistanciaAtaque = 0.3f;

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

        // Activa la animación del fragmento
        if (animador != null)
            animador.enabled = true;

        // Reproduce sonido
        if (audioSource != null && sonidoActivacion != null)
            audioSource.PlayOneShot(sonidoActivacion);

        // Mejora al jugador
        if (playerCombat != null)
        {
            // Más alcance del ataque
            playerCombat.weaponRange += aumentoAlcance;

            // Más fuerza del knockback
            playerCombat.knockbackMultiplier += aumentoRetroceso;
            Debug.Log("Knockback actual: " + playerCombat.knockbackMultiplier);

            // El golpe sale un poco más adelante
            playerCombat.attackPointDistanceMultiplier += aumentoDistanciaAtaque;

            Debug.Log("===== FRAGMENTO DE PRECISIÓN ACTIVADO =====");
            Debug.Log("Nuevo alcance: " + playerCombat.weaponRange);
            Debug.Log("Nuevo knockback: " + playerCombat.knockbackMultiplier);
            Debug.Log("Nueva distancia del AttackPoint: " + playerCombat.attackPointDistanceMultiplier);
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

        Player_Combat combatDetectado = other.GetComponent<Player_Combat>();

        if (combatDetectado == null)
            combatDetectado = other.GetComponentInParent<Player_Combat>();

        if (combatDetectado == null)
            combatDetectado = other.GetComponentInChildren<Player_Combat>();

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
        Player_Combat combatDetectado = other.GetComponent<Player_Combat>();

        if (combatDetectado == null)
            combatDetectado = other.GetComponentInParent<Player_Combat>();

        if (combatDetectado == null)
            combatDetectado = other.GetComponentInChildren<Player_Combat>();

        if (combatDetectado != null)
        {
            jugadorCerca = false;

            if (textoActivar != null)
                textoActivar.SetActive(false);
        }
    }
}