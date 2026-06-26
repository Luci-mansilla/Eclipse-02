using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("Sliders")]
    public Slider sliderMusica;
    public Slider sliderEfectos;
    public Slider sliderBrillo;

    [Header("Textos de porcentaje")]
    public TMP_Text textoMusica;
    public TMP_Text textoEfectos;
    public TMP_Text textoBrillo;

    [Header("Botones de calidad")]
    public Button btnBaja;
    public Button btnMedia;
    public Button btnAlta;

    // Colores para boton activo/inactivo
    private Color colorActivo = new Color(0.75f, 0.22f, 0.17f, 1f);
    private Color colorInactivo = new Color(0.10f, 0.02f, 0.02f, 1f);

    private string calidadActual = "Media";
    private float musicaOriginal, efectosOriginal, brilloOriginal;
    private string calidadOriginal;

    void Start()
    {
        // ── Cargamos valores desde GameSettings si existe, si no desde PlayerPrefs ──
        if (GameSettings.Instancia != null)
        {
            sliderMusica.value = GameSettings.Instancia.VolumenMusica;
            sliderEfectos.value = GameSettings.Instancia.VolumenEfectos;
            sliderBrillo.value = GameSettings.Instancia.Brillo;
            calidadActual = GameSettings.Instancia.Calidad;
        }
        else
        {
            sliderMusica.value = PlayerPrefs.GetFloat("VolMusica", 0.8f);
            sliderEfectos.value = PlayerPrefs.GetFloat("VolEfectos", 0.7f);
            sliderBrillo.value = PlayerPrefs.GetFloat("Brillo", 0.6f);
            calidadActual = PlayerPrefs.GetString("Calidad", "Media");
        }

        // Guardamos los originales para poder descartar cambios
        musicaOriginal = sliderMusica.value;
        efectosOriginal = sliderEfectos.value;
        brilloOriginal = sliderBrillo.value;
        calidadOriginal = calidadActual;

        // Conectamos los sliders para que actualicen el texto en tiempo real
        sliderMusica.onValueChanged.AddListener(v =>
            textoMusica.text = Mathf.RoundToInt(v * 100) + "%");

        sliderEfectos.onValueChanged.AddListener(v =>
            textoEfectos.text = Mathf.RoundToInt(v * 100) + "%");

        sliderBrillo.onValueChanged.AddListener(v =>
            textoBrillo.text = Mathf.RoundToInt(v * 100) + "%");

        // Mostramos los valores iniciales
        ActualizarTextos();
        ActualizarBotonesCalidad();
    }

    // ── Funciones internas ──────────────────────────────────────────────────

    void ActualizarTextos()
    {
        textoMusica.text = Mathf.RoundToInt(sliderMusica.value * 100) + "%";
        textoEfectos.text = Mathf.RoundToInt(sliderEfectos.value * 100) + "%";
        textoBrillo.text = Mathf.RoundToInt(sliderBrillo.value * 100) + "%";
    }

    void ActualizarBotonesCalidad()
    {
        // El boton activo se pone rojo, los otros oscuros
        btnBaja.GetComponent<Image>().color =
            calidadActual == "Baja" ? colorActivo : colorInactivo;
        btnMedia.GetComponent<Image>().color =
            calidadActual == "Media" ? colorActivo : colorInactivo;
        btnAlta.GetComponent<Image>().color =
            calidadActual == "Alta" ? colorActivo : colorInactivo;
    }

    // ── Botones de calidad ──────────────────────────────────────────────────

    public void SetCalidadBaja()
    {
        calidadActual = "Baja";
        ActualizarBotonesCalidad();
    }

    public void SetCalidadMedia()
    {
        calidadActual = "Media";
        ActualizarBotonesCalidad();
    }

    public void SetCalidadAlta()
    {
        calidadActual = "Alta";
        ActualizarBotonesCalidad();
    }

    // ── Botones del pie ─────────────────────────────────────────────────────

    public void AplicarCambios()
    {
        // FIX: usamos GameSettings para guardar Y aplicar todo junto
        if (GameSettings.Instancia != null)
        {
            GameSettings.Instancia.GuardarYAplicar(
                sliderMusica.value,
                sliderEfectos.value,
                sliderBrillo.value,
                calidadActual
            );
        }
        else
        {
            // Fallback si GameSettings no existe en la escena
            PlayerPrefs.SetFloat("VolMusica", sliderMusica.value);
            PlayerPrefs.SetFloat("VolEfectos", sliderEfectos.value);
            PlayerPrefs.SetFloat("Brillo", sliderBrillo.value);
            PlayerPrefs.SetString("Calidad", calidadActual);
            PlayerPrefs.Save();
        }

        Debug.Log("Cambios guardados correctamente.");
    }

    public void DescartarCambios()
    {
        // Volvemos a los valores que habia cuando se abrio el menu
        sliderMusica.value = musicaOriginal;
        sliderEfectos.value = efectosOriginal;
        sliderBrillo.value = brilloOriginal;
        calidadActual = calidadOriginal;
        ActualizarTextos();
        ActualizarBotonesCalidad();
    }

    public void VolverAlMenu()
    {
        // FIX: nombre corregido para que coincida con tu escena real
        SceneManager.LoadScene("Escena-menu");
    }
}
