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
    public float VolumenMusica = 0.5f;   // FIX: default 50%
    public float VolumenEfectos = 0.5f;  // FIX: default 50%
    public float Brillo = 0.5f;          // FIX: default 50%
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
        // FIX: defaults en 0.5 (50%) para primera ejecucion sin PlayerPrefs
        VolumenMusica = PlayerPrefs.GetFloat("VolMusica", 0.5f);
        VolumenEfectos = PlayerPrefs.GetFloat("VolEfectos", 0.5f);
        Brillo = PlayerPrefs.GetFloat("Brillo", 0.5f);
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
            // FIX: detectamos el tipo por tag primero, luego por nombre.
            // Esto resuelve el problema con nombres como "SONIDO. MENU"
            // que no contienen las palabras clave esperadas.
            if (EsMusica(src))
            {
                src.volume = VolumenMusica;
            }
            else
            {
                src.volume = VolumenEfectos;
            }
        }
    }

    // FIX: funcion centralizada para decidir si un AudioSource es musica.
    // Prioridad: 1) Tag "Music", 2) Tag "SFX", 3) nombre del GameObject.
    // Si no matchea nada de efectos, se trata como musica (comportamiento
    // seguro: mejor subir musica de mas que dejarla en silencio).
    private bool EsMusica(AudioSource src)
    {
        if (src == null) return true;

        // Por tag (configura esto en Unity: tag "Music" o "SFX")
        string tag = src.gameObject.tag.ToLower();
        if (tag == "music" || tag == "musica") return true;
        if (tag == "sfx" || tag == "efecto" || tag == "sound") return false;

        // Por nombre del GameObject (insensible a mayusculas y espacios)
        string nombre = src.gameObject.name.ToLower().Replace(" ", "").Replace(".", "");
        bool esEfecto =
            nombre.Contains("efecto") ||
            nombre.Contains("sfx") ||
            nombre.Contains("sonido") ||
            nombre.Contains("sound") ||
            nombre.Contains("click") ||
            nombre.Contains("hover");

        // Todo lo demas (incluido "SONIDOMENU" normalizado) se trata como musica
        return !esEfecto;
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
