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
            if (textoPortalBloqueado != null)
                textoPortalBloqueado.SetActive(false);

            if (textoCruzarPortal != null)
                textoCruzarPortal.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                AbrirPortal();
            }
        }
        else
        {
            if (textoPortalBloqueado != null)
                textoPortalBloqueado.SetActive(true);

            if (textoCruzarPortal != null)
                textoCruzarPortal.SetActive(false);
        }
    }

    bool TodosLosEnemigosMuertos()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag(enemyTag);

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

        if (textoPortalBloqueado != null)
            textoPortalBloqueado.SetActive(false);

        if (textoCruzarPortal != null)
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
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            if (textoPortalBloqueado != null)
                textoPortalBloqueado.SetActive(false);

            if (textoCruzarPortal != null)
                textoCruzarPortal.SetActive(false);
        }
    }
}