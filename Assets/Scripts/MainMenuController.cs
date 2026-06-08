using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Controla toda la lógica del menú principal:
/// botones principales, botonera de redes sociales y transiciones.
///
/// SETUP EN UNITY:
/// 1. En la escena del menú, crear un Canvas (Screen Space - Overlay).
/// 2. Crear un GameObject vacío "MenuController" dentro del Canvas
///    y adjuntar este script.
/// 3. Asignar todas las referencias en el Inspector.
/// 4. El nombre de escena para "Nueva Partida" debe coincidir con
///    el nombre real en Build Settings.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // REFERENCIAS UI — BOTONES PRINCIPALES
    // ─────────────────────────────────────────────
    [Header("Botones Principales")]
    [Tooltip("Botón 'Nueva Partida'")]
    public Button btnNuevaPartida;

    [Tooltip("Botón 'Opciones'")]
    public Button btnOpciones;

    [Tooltip("Botón 'Créditos'")]
    public Button btnCreditos;

    [Tooltip("Botón 'Salir del juego'")]
    public Button btnSalir;

    // ─────────────────────────────────────────────
    // PANELES
    // ─────────────────────────────────────────────
    [Header("Paneles Secundarios")]
    [Tooltip("Panel que contiene las opciones del juego.")]
    public GameObject panelOpciones;

    [Tooltip("Panel que muestra los créditos.")]
    public GameObject panelCreditos;

    // ─────────────────────────────────────────────
    // BOTONES DE REDES SOCIALES
    // ─────────────────────────────────────────────
    [Header("Botonera de Redes Sociales")]
    public Button btnDiscord;
    public Button btnTwitterX;
    public Button btnYouTube;
    public Button btnSteam;

    // ─────────────────────────────────────────────
    // URLs DE REDES SOCIALES — EDITAR EN INSPECTOR
    // ─────────────────────────────────────────────
    [Header("URLs de Redes Sociales")]
    [Tooltip("URL de tu servidor de Discord.")]
    public string urlDiscord  = "https://discord.gg/TU_SERVIDOR";

    [Tooltip("URL de tu perfil o página en X (Twitter).")]
    public string urlTwitterX = "https://x.com/TU_USUARIO";

    [Tooltip("URL de tu canal de YouTube.")]
    public string urlYouTube  = "https://youtube.com/@TU_CANAL";

    [Tooltip("URL de tu juego en Steam.")]
    public string urlSteam    = "https://store.steampowered.com/app/TU_ID";

    // ─────────────────────────────────────────────
    // CONFIGURACIÓN DE ESCENAS
    // ─────────────────────────────────────────────
    [Header("Nombre de Escenas")]
    [Tooltip("Nombre exacto de la escena del juego (debe estar en Build Settings).")]
    public string nombreEscenaJuego = "GameScene";

    // ─────────────────────────────────────────────
    // TRANSICIÓN
    // ─────────────────────────────────────────────
    [Header("Transición de Pantalla")]
    [Tooltip("Panel negro para el fade in/out. Debe tener un CanvasGroup.")]
    public CanvasGroup panelFade;

    [Tooltip("Duración en segundos del fade al cargar escena.")]
    public float duracionFade = 0.8f;

    // ─────────────────────────────────────────────
    // AUDIO
    // ─────────────────────────────────────────────
    [Header("Audio del Menú")]
    [Tooltip("AudioSource con la música de fondo del menú.")]
    public AudioSource musicaFondo;

    [Tooltip("Clip de sonido al hacer hover sobre un botón.")]
    public AudioClip sonidoHover;

    [Tooltip("Clip de sonido al presionar un botón.")]
    public AudioClip sonidoClick;

    private AudioSource _sfxSource;

    // ─────────────────────────────────────────────
    // UNITY LIFECYCLE
    // ─────────────────────────────────────────────
    void Awake()
    {
        // Asegurarse de que los paneles secundarios estén cerrados al inicio
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelCreditos != null) panelCreditos.SetActive(false);

        // Crear fuente de audio para efectos si no existe
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
    }

    void Start()
    {
        // Fade in al iniciar el menú
        if (panelFade != null)
            StartCoroutine(FadeIn());

        // Asignar listeners a botones principales
        if (btnNuevaPartida != null) btnNuevaPartida.onClick.AddListener(OnNuevaPartida);
        if (btnOpciones     != null) btnOpciones.onClick.AddListener(OnOpciones);
        if (btnCreditos     != null) btnCreditos.onClick.AddListener(OnCreditos);
        if (btnSalir        != null) btnSalir.onClick.AddListener(OnSalir);

        // Asignar listeners a botones de redes sociales
        if (btnDiscord  != null) btnDiscord.onClick.AddListener(() => AbrirURL(urlDiscord));
        if (btnTwitterX != null) btnTwitterX.onClick.AddListener(() => AbrirURL(urlTwitterX));
        if (btnYouTube  != null) btnYouTube.onClick.AddListener(() => AbrirURL(urlYouTube));
        if (btnSteam    != null) btnSteam.onClick.AddListener(() => AbrirURL(urlSteam));

        // Agregar sonido hover a todos los botones
        AddHoverSoundToAll();
    }

    // ─────────────────────────────────────────────
    // CALLBACKS BOTONES PRINCIPALES
    // ─────────────────────────────────────────────

    public void OnNuevaPartida()
    {
        PlayClickSound();
        StartCoroutine(FadeOutAndLoad(nombreEscenaJuego));
    }

    public void OnOpciones()
    {
        PlayClickSound();
        bool estaActivo = panelOpciones != null && panelOpciones.activeSelf;

        // Cerrar todos los paneles y abrir/cerrar opciones
        CerrarPaneles();
        if (!estaActivo && panelOpciones != null)
            panelOpciones.SetActive(true);
    }

    public void OnCreditos()
    {
        PlayClickSound();
        bool estaActivo = panelCreditos != null && panelCreditos.activeSelf;

        CerrarPaneles();
        if (!estaActivo && panelCreditos != null)
            panelCreditos.SetActive(true);
    }

    public void OnSalir()
    {
        PlayClickSound();
        Debug.Log("[MainMenu] Saliendo del juego...");
        // En el editor no cierra la ventana; en build sí.
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // ─────────────────────────────────────────────
    // REDES SOCIALES
    // ─────────────────────────────────────────────

    private void AbrirURL(string url)
    {
        PlayClickSound();
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("[MainMenu] URL vacía o no configurada.");
            return;
        }
        Application.OpenURL(url);
        Debug.Log($"[MainMenu] Abriendo URL: {url}");
    }

    // ─────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────

    private void CerrarPaneles()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelCreditos != null) panelCreditos.SetActive(false);
    }

    /// <summary>
    /// Agrega efecto de sonido hover a todos los botones de la escena.
    /// Usa EventTrigger para detectar PointerEnter.
    /// </summary>
    private void AddHoverSoundToAll()
    {
        if (sonidoHover == null) return;

        Button[] todosLosBotones = FindObjectsOfType<Button>();
        foreach (var btn in todosLosBotones)
        {
            var trigger = btn.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (trigger == null)
                trigger = btn.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
            entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            entry.callback.AddListener((_) => PlayHoverSound());
            trigger.triggers.Add(entry);
        }
    }

    // ─────────────────────────────────────────────
    // AUDIO
    // ─────────────────────────────────────────────

    private void PlayClickSound()
    {
        if (sonidoClick != null && _sfxSource != null)
            _sfxSource.PlayOneShot(sonidoClick);
    }

    private void PlayHoverSound()
    {
        if (sonidoHover != null && _sfxSource != null)
            _sfxSource.PlayOneShot(sonidoHover, 0.5f);
    }

    // ─────────────────────────────────────────────
    // TRANSICIONES FADE
    // ─────────────────────────────────────────────

    private IEnumerator FadeIn()
    {
        if (panelFade == null) yield break;

        panelFade.alpha = 1f;
        panelFade.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < duracionFade)
        {
            elapsed += Time.deltaTime;
            panelFade.alpha = Mathf.Lerp(1f, 0f, elapsed / duracionFade);
            yield return null;
        }

        panelFade.alpha = 0f;
        panelFade.gameObject.SetActive(false);
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (panelFade != null)
        {
            panelFade.gameObject.SetActive(true);
            float elapsed = 0f;

            while (elapsed < duracionFade)
            {
                elapsed += Time.deltaTime;
                panelFade.alpha = Mathf.Lerp(0f, 1f, elapsed / duracionFade);
                yield return null;
            }
            panelFade.alpha = 1f;
        }

        // Fade de audio si hay música
        if (musicaFondo != null)
            StartCoroutine(FadeOutAudio(musicaFondo, duracionFade));

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOutAudio(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
}
