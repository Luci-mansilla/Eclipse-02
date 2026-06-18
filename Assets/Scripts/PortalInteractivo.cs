using UnityEngine;

public class PortalInteractivo : MonoBehaviour
{
    private Animator animador;
    private bool jugadorCerca = false;

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
            animador.Play("PortalAbriendose");
        }
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