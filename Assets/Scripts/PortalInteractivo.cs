using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalInteractivo : MonoBehaviour
{
    private Animator animador;
    private bool jugadorCerca = false;

    public int escenaDestino;

    void Start()
    {
        animador = GetComponent<Animator>();
        animador.enabled = false;
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("TOCASTE F CERCA DEL PORTAL");

            animador.enabled = true;
            animador.Play("PortalAbriendose", 0, 0f);

            Invoke("CambiarEscena", 2f);
        }
    }

    void CambiarEscena()
    {
        SceneManager.LoadScene(escenaDestino);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entró algo: " + other.name);

        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Jugador cerca");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            Debug.Log("Jugador salió");
        }
    }
}