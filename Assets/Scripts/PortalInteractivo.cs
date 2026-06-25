using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalInteractivo : MonoBehaviour
{
    private Animator animador;
    private bool jugadorCerca = false;
    private bool portalAbierto = false;

    public int escenaDestino;
    public string enemyTag = "Enemy";

    public GameObject textoPortalBloqueado;
    public GameObject textoCruzarPortal;

    void Start()
    {
        animador = GetComponent<Animator>();

        if (animador != null)
            animador.enabled = false;

        if (textoPortalBloqueado != null)
            textoPortalBloqueado.SetActive(false);

        if (textoCruzarPortal != null)
            textoCruzarPortal.SetActive(false);
    }

    void Update()
    {
        if (!jugadorCerca || portalAbierto) return;

        if (TodosLosEnemigosMuertos())
        {
            textoPortalBloqueado.SetActive(false);
            textoCruzarPortal.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                AbrirPortal();
            }
        }
        else
        {
            textoPortalBloqueado.SetActive(true);
            textoCruzarPortal.SetActive(false);
        }
    }

    bool TodosLosEnemigosMuertos()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag(enemyTag);

        Debug.Log("Enemigos encontrados: " + enemigos.Length);

        foreach (GameObject enemigo in enemigos)
        {
            EnemyHealth enemyHealth = enemigo.GetComponent<EnemyHealth>();

            if (enemyHealth != null && !enemyHealth.IsDead())
            {
                return false;
            }
        }

        return true;
    }

    void AbrirPortal()
    {
        portalAbierto = true;

        textoPortalBloqueado.SetActive(false);
        textoCruzarPortal.SetActive(false);

        if (animador != null)
        {
            animador.enabled = true;
            animador.Play("PortalAbriendose", 0, 0f);
        }

        Invoke("CambiarEscena", 2f);
    }

    void CambiarEscena()
    {
        SceneManager.LoadScene(escenaDestino);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entró al portal: " + other.name);

        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Jugador cerca del portal");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Salió del portal: " + other.name);

        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            // Mantiene el mensaje visible 3 segundos
            Invoke(nameof(OcultarTextos), 3f);
        }
    }

    void OcultarTextos()
    {
        // Solo los oculta si el jugador sigue lejos
        if (!jugadorCerca)
        {
            if (textoPortalBloqueado != null)
                textoPortalBloqueado.SetActive(false);

            if (textoCruzarPortal != null)
                textoCruzarPortal.SetActive(false);
        }
    }
}