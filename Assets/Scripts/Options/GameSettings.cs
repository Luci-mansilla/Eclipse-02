using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Pone este objeto en Escena-menu. Vive en TODAS las escenas.
// En el Inspector asigna: sourcesMusica y sourcesEfectos son opcionales,
// el sistema los busca automaticamente si no los asignas.

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instancia;

    [Header("Valores actuales")]
    public float VolumenMusica = 0.8f;
    public float VolumenEfectos = 0.7f;
    public float Brillo = 0.6f;
    public string Calidad = "Media";

    // Panel negro que usamos para simular brillo en PC
    // Se crea automaticamente, no hace falta asignar nada
    private GameObject _panelBrillo;
    private Image _imagenBrillo;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);

        CargarValores();

        // Escuchamos cuando cambia la escena para re-aplicar todo
        SceneManager.sceneLoaded += OnScenaCargada;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnScenaCargada;
    }

    // Se llama automaticamente cada vez que carga una escena nueva
    void OnScenaCargada(Scene escena, LoadSceneMode modo)
    {
        // El panel viejo se destruyo con la escena anterior, lo reseteamos
        _panelBrillo = null;
        _imagenBrillo = null;
        AplicarTodo();
    }

    // ── Carga y guardado ────────────────────────────────────────────────────

    public void CargarValores()
    {
        VolumenMusica = PlayerPrefs.GetFloat("VolMusica", 0.8f);
        VolumenEfectos = PlayerPrefs.GetFloat("VolEfectos", 0.7f);
        Brillo = PlayerPrefs.GetFloat("Brillo", 0.6f);
        Calidad = PlayerPrefs.GetString("Calidad", "Media");
        AplicarTodo();
    }

    public void GuardarYAplicar(float musica, float efectos, float brillo, string calidad)
    {
        VolumenMusica = musica;
        VolumenEfectos = efectos;
        Brillo = brillo;
        Calidad = calidad;

        PlayerPrefs.SetFloat("VolMusica", musica);
        PlayerPrefs.SetFloat("VolEfectos", efectos);
        PlayerPrefs.SetFloat("Brillo", brillo);
        PlayerPrefs.SetString("Calidad", calidad);
        PlayerPrefs.Save();

        AplicarTodo();

        Debug.Log("Configuracion guardada y aplicada.");
    }

    // ── Aplicar todo ────────────────────────────────────────────────────────

    public void AplicarTodo()
    {
        AplicarCalidad();
        AplicarVolumenes();
        AplicarBrillo();
    }

    // ── Calidad grafica ─────────────────────────────────────────────────────

    void AplicarCalidad()
    {
        switch (Calidad)
        {
            case "Baja": QualitySettings.SetQualityLevel(0, true); break;
            case "Media": QualitySettings.SetQualityLevel(2, true); break;
            case "Alta": QualitySettings.SetQualityLevel(5, true); break;
        }
    }

    // ── Volumenes ───────────────────────────────────────────────────────────

    void AplicarVolumenes()
    {
        AudioSource[] todos = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource src in todos)
        {
            string nombre = src.gameObject.name.ToLower();

            if (nombre.Contains("music") || nombre.Contains("musica") ||
                nombre.Contains("fondo") || nombre.Contains("bgm"))
            {
                src.volume = VolumenMusica;
            }
            else if (nombre.Contains("efecto") || nombre.Contains("sfx") ||
                     nombre.Contains("sonido") || nombre.Contains("sound"))
            {
                src.volume = VolumenEfectos;
            }
            else
            {
                src.volume = VolumenMusica;
            }
        }
    }

    // Metodo publico para que otros scripts apliquen el volumen correcto
    // Uso: GameSettings.Instancia.AplicarVolumenA(miAudioSource, esSFX: false);
    public void AplicarVolumenA(AudioSource src, bool esSFX = false)
    {
        if (src == null) return;
        src.volume = esSFX ? VolumenEfectos : VolumenMusica;
    }

    // ── Brillo via panel negro superpuesto ──────────────────────────────────

    void AplicarBrillo()
    {
        CrearPanelBrilloSiNoExiste();
        if (_imagenBrillo == null) return;

        // Brillo 1.0 = pantalla normal (alpha 0 = panel invisible)
        // Brillo 0.0 = pantalla muy oscura (alpha 0.85 = casi negro)
        float alpha = Mathf.Lerp(0.85f, 0f, Brillo);
        Color c = _imagenBrillo.color;
        c.a = alpha;
        _imagenBrillo.color = c;
    }

    void CrearPanelBrilloSiNoExiste()
    {
        if (_panelBrillo != null) return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        // Creamos el panel negro como hijo del Canvas
        _panelBrillo = new GameObject("_PanelBrillo");
        _panelBrillo.transform.SetParent(canvas.transform, false);
        // FIX: eliminado DontDestroyOnLoad — el panel se recrea solo
        //      en cada escena gracias a OnScenaCargada

        _panelBrillo.transform.SetAsLastSibling();

        _imagenBrillo = _panelBrillo.AddComponent<Image>();
        _imagenBrillo.color = new Color(0f, 0f, 0f, 0f);
        _imagenBrillo.raycastTarget = false;

        RectTransform rt = _panelBrillo.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
