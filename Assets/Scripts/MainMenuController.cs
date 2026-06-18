using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // BOTONES PRINCIPALES
    // ─────────────────────────────────────────────
    [Header("Botones Principales")]
    public Button btnNuevaPartida;
    public Button btnOpciones;
    public Button btnCreditos;
    public Button btnSalir;

    // ─────────────────────────────────────────────
    // PANELES
    // ─────────────────────────────────────────────
    [Header("Paneles Secundarios")]
    public GameObject panelOpciones;
    public GameObject panelCreditos;

    // ─────────────────────────────────────────────
    // BOTONES REDES SOCIALES
    // ─────────────────────────────────────────────
    [Header("Botonera de Redes Sociales")]
    public Button btnDiscord;
    public Button btnTwitterX;
    public Button btnYouTube;
    public Button btnSteam;

    // ─────────────────────────────────────────────
    // URLs — YA CONFIGURADAS
    // ─────────────────────────────────────────────
    [Header("URLs de Redes Sociales")]
    public string urlDiscord = "https://discord.gg/eZXyz6u7";
    public string urlTwitterX = "https://x.com/Eclipse_Ar_Game";
    public string urlYouTube = "https://www.youtube.com/@Eclipse-Archive_Game";
    public string urlSteam = "https://store.steampowered.com/";

    // ─────────────────────────────────────────────
    // ESCENA
    // ─────────────────────────────────────────────
    [Header("Nombre de Escena del Juego")]
    public string nombreEscenaJuego = "Escena-1";

    // ─────────────────────────────────────────────
    // TRANSICIÓN
    // ─────────────────────────────────────────────
    [Header("Transición de Pantalla")]
    public CanvasGroup panelFade;
    public float duracionFade = 0.8f;

    // ─────────────────────────────────────────────
    // AUDIO
    // ─────────────────────────────────────────────
    [Header("Audio del Menú")]
    public AudioSource musicaFondo;
    public AudioClip sonidoHover;
    public AudioClip sonidoClick;

    private AudioSource _sfxSource;

    // ─────────────────────────────────────────────
    // UNITY LIFECYCLE
    // ─────────────────────────────────────────────
    void Awake()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelCreditos != null) panelCreditos.SetActive(false);

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
    }

    void Start()
    {
        if (panelFade != null)
            StartCoroutine(FadeIn());

        // Botones principales
        if (btnNuevaPartida != null) btnNuevaPartida.onClick.AddListener(OnNuevaPartida);
        if (btnOpciones != null) btnOpciones.onClick.AddListener(OnOpciones);
        if (btnCreditos != null) btnCreditos.onClick.AddListener(OnCreditos);
        if (btnSalir != null) btnSalir.onClick.AddListener(OnSalir);

        // Redes sociales
        if (btnDiscord != null) btnDiscord.onClick.AddListener(() => AbrirURL(urlDiscord));
        if (btnTwitterX != null) btnTwitterX.onClick.AddListener(() => AbrirURL(urlTwitterX));
        if (btnYouTube != null) btnYouTube.onClick.AddListener(() => AbrirURL(urlYouTube));
        if (btnSteam != null) btnSteam.onClick.AddListener(() => AbrirURL(urlSteam));

        AddHoverSoundToAll();
    }

    // ─────────────────────────────────────────────
    // BOTONES PRINCIPALES
    // ─────────────────────────────────────────────

    public void OnNuevaPartida()
    {
        PlayClickSound();
        // Verifica que la escena esté en Build Settings antes de cargar
        StartCoroutine(FadeOutAndLoad(nombreEscenaJuego));
    }

    public void OnOpciones()
    {
        PlayClickSound();
        bool estaActivo = panelOpciones != null && panelOpciones.activeSelf;
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
            Debug.LogWarning("[MainMenu] URL vacía.");
            return;
        }
        Application.OpenURL(url);
        Debug.Log($"[MainMenu] Abriendo: {url}");
    }

    // ─────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────

    private void CerrarPaneles()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelCreditos != null) panelCreditos.SetActive(false);
    }

    private void AddHoverSoundToAll()
    {
        if (sonidoHover == null) return;

        Button[] todos = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (var btn in todos)
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
    // TRANSICIONES
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
