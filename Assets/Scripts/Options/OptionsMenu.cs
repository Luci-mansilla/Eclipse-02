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

    // Colores botones de calidad
    private Color colorActivoFondo = new Color(0.55f, 0.10f, 0.08f, 1f);
    private Color colorActivoTexto = new Color(1.00f, 0.85f, 0.80f, 1f);
    private Color colorInactivoFondo = new Color(0.08f, 0.02f, 0.02f, 1f);
    private Color colorInactivoTexto = new Color(0.65f, 0.25f, 0.20f, 1f);

    private string calidadActual = "Media";
    private float musicaOriginal, efectosOriginal, brilloOriginal;
    private string calidadOriginal;

    void Start()
    {
        // Cargar valores desde GameSettings o PlayerPrefs
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

        // Guardamos originales para poder descartar
        musicaOriginal = sliderMusica.value;
        efectosOriginal = sliderEfectos.value;
        brilloOriginal = sliderBrillo.value;
        calidadOriginal = calidadActual;

        // Listeners: actualizan texto Y aplican el efecto en tiempo real
        sliderMusica.onValueChanged.AddListener(v => {
            textoMusica.text = Mathf.RoundToInt(v * 100) + "%";
            // Preview en tiempo real del volumen de musica
            if (GameSettings.Instancia != null)
                GameSettings.Instancia.VolumenMusica = v;
            AplicarVolumenesTemporal();
        });

        sliderEfectos.onValueChanged.AddListener(v => {
            textoEfectos.text = Mathf.RoundToInt(v * 100) + "%";
            if (GameSettings.Instancia != null)
                GameSettings.Instancia.VolumenEfectos = v;
            AplicarVolumenesTemporal();
        });

        sliderBrillo.onValueChanged.AddListener(v => {
            textoBrillo.text = Mathf.RoundToInt(v * 100) + "%";
            // Preview en tiempo real del brillo
            if (GameSettings.Instancia != null)
            {
                GameSettings.Instancia.Brillo = v;
                GameSettings.Instancia.AplicarTodo();
            }
        });

        // Desactivar navegacion automatica de Unity (evita deseleccion)
        DesactivarNavegacion(btnBaja);
        DesactivarNavegacion(btnMedia);
        DesactivarNavegacion(btnAlta);

        ActualizarTextos();
        ActualizarBotonesCalidad();
    }

    // Aplica volumenes en tiempo real mientras se mueve el slider
    void AplicarVolumenesTemporal()
    {
        if (GameSettings.Instancia != null)
            GameSettings.Instancia.AplicarTodo();
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    void DesactivarNavegacion(Button btn)
    {
        if (btn == null) return;
        Navigation nav = btn.navigation;
        nav.mode = Navigation.Mode.None;
        btn.navigation = nav;
    }

    void ActualizarTextos()
    {
        textoMusica.text = Mathf.RoundToInt(sliderMusica.value * 100) + "%";
        textoEfectos.text = Mathf.RoundToInt(sliderEfectos.value * 100) + "%";
        textoBrillo.text = Mathf.RoundToInt(sliderBrillo.value * 100) + "%";
    }

    void ActualizarBotonesCalidad()
    {
        AplicarEstadoCalidad(btnBaja, "Baja");
        AplicarEstadoCalidad(btnMedia, "Media");
        AplicarEstadoCalidad(btnAlta, "Alta");
    }

    void AplicarEstadoCalidad(Button btn, string calidad)
    {
        if (btn == null) return;
        bool activo = calidadActual == calidad;

        Image img = btn.GetComponent<Image>();
        if (img != null)
            img.color = activo ? colorActivoFondo : colorInactivoFondo;

        TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
        if (txt != null)
            txt.color = activo ? colorActivoTexto : colorInactivoTexto;

        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1.4f, 1.4f, 1.4f, 1f);
        cb.pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        cb.selectedColor = Color.white;
        cb.colorMultiplier = 1f;
        btn.colors = cb;
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
        // Guardamos permanentemente con GameSettings
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
            PlayerPrefs.SetFloat("VolMusica", sliderMusica.value);
            PlayerPrefs.SetFloat("VolEfectos", sliderEfectos.value);
            PlayerPrefs.SetFloat("Brillo", sliderBrillo.value);
            PlayerPrefs.SetString("Calidad", calidadActual);
            PlayerPrefs.Save();
        }

        // Actualizamos los originales
        musicaOriginal = sliderMusica.value;
        efectosOriginal = sliderEfectos.value;
        brilloOriginal = sliderBrillo.value;
        calidadOriginal = calidadActual;

        Debug.Log("Cambios guardados y aplicados.");
    }

    public void DescartarCambios()
    {
        // Volvemos a los valores originales visualmente
        sliderMusica.value = musicaOriginal;
        sliderEfectos.value = efectosOriginal;
        sliderBrillo.value = brilloOriginal;
        calidadActual = calidadOriginal;

        // Y los aplicamos de verdad
        if (GameSettings.Instancia != null)
        {
            GameSettings.Instancia.VolumenMusica = musicaOriginal;
            GameSettings.Instancia.VolumenEfectos = efectosOriginal;
            GameSettings.Instancia.Brillo = brilloOriginal;
            GameSettings.Instancia.Calidad = calidadOriginal;
            GameSettings.Instancia.AplicarTodo();
        }

        ActualizarTextos();
        ActualizarBotonesCalidad();
    }

    public void VolverAlMenu()
    {
        // Si no aplicaron cambios, descartamos antes de salir
        DescartarCambios();
        SceneManager.LoadScene("Escena-menu");
    }
}
