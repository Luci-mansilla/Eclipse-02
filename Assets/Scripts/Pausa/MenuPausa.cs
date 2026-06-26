using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    [Tooltip("El objeto hijo con la imagen y los botones (Contenido)")]
    public GameObject panelPausa;

    private bool pausado = false;

    void Start()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);

        ConectarBoton("Btn-reiniciar", ReiniciarPartida);
        ConectarBoton("Btn-continuar", Continuar);
        ConectarBoton("Btn-menu", VolverAlMenu);
    }

    void ConectarBoton(string nombreBoton, UnityEngine.Events.UnityAction accion)
    {
        Button[] botones = GetComponentsInChildren<Button>(true);
        foreach (Button btn in botones)
        {
            if (btn.gameObject.name == nombreBoton)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(accion);
                return;
            }
        }
        Debug.LogWarning("No se encontro el boton: " + nombreBoton);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado) Continuar();
            else Pausar();
        }
    }

    void Pausar()
    {
        pausado = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;   // pausa TODO el audio
        if (panelPausa != null)
            panelPausa.SetActive(true);
    }

    public void Continuar()
    {
        pausado = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;  // reanuda el audio
        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("Escena-menu");
    }
}
