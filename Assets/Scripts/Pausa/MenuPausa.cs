using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ================================================================
//  MenuPausa — conecta los botones automaticamente, sin Inspector
//
//  COMO USARLO:
//   1. Este script va en el Canvas raiz (Panel-Pausa)
//   2. El campo "Panel Pausa" debe tener el objeto Contenido asignado
//   3. Los botones se conectan solos si se llaman:
//      "Btn-reiniciar", "Btn-continuar", "Btn-menu"
//   4. No hace falta conectar nada en On Click()
// ================================================================
public class MenuPausa : MonoBehaviour
{
    [Tooltip("El objeto hijo con la imagen y los botones (Contenido)")]
    public GameObject panelPausa;

    private bool pausado = false;

    void Start()
    {
        // Ocultar el panel al inicio
        if (panelPausa != null)
            panelPausa.SetActive(false);

        // Conectar botones automaticamente por nombre
        ConectarBoton("Btn-reiniciar", ReiniciarPartida);
        ConectarBoton("Btn-continuar", Continuar);
        ConectarBoton("Btn-menu", VolverAlMenu);
    }

    // Busca el boton por nombre en toda la jerarquia y le asigna el metodo
    void ConectarBoton(string nombreBoton, UnityEngine.Events.UnityAction accion)
    {
        Button[] botones = GetComponentsInChildren<Button>(true);

        foreach (Button btn in botones)
        {
            if (btn.gameObject.name == nombreBoton)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(accion);
                Debug.Log("Boton conectado: " + nombreBoton);
                return;
            }
        }

        Debug.LogWarning("No se encontro el boton: " + nombreBoton);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado)
                Continuar();
            else
                Pausar();
        }
    }

    void Pausar()
    {
        pausado = true;
        Time.timeScale = 0f;
        if (panelPausa != null)
            panelPausa.SetActive(true);
    }

    public void Continuar()
    {
        pausado = false;
        Time.timeScale = 1f;
        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Escena-menu");
    }
}
