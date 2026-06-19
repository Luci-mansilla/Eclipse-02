using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Botones Principales")]
    public Button btnNuevaPartida;
    public Button btnOpciones;
    public Button btnCreditos;
    public Button btnSalir;

    [Header("Paneles Secundarios")]
    public GameObject panelOpciones;
    public GameObject panelCreditos;

    [Header("Botonera de Redes Sociales")]
    public Button btnDiscord;
    public Button btnTwitterX;
    public Button btnYouTube;
    public Button btnSteam;

    [Header("URLs de Redes Sociales")]
    public string urlDiscord = "https://discord.gg/eZXyz6u7";
    public string urlTwitterX = "https://x.com/Eclipse_Ar_Game";
    public string urlYouTube = "https://www.youtube.com/@Eclipse-Archive_Game";
    public string urlSteam = "https://store.steampowered.com/";

    [Header("Nombre de Escena del Juego")]
    public string nombreEscenaJuego = "Escena-1";

    [Header("Transicion de Pantalla")]
    public CanvasGroup panelFade;
    public float duracionFade = 0.8f;

    [Header("Audio del Menu")]
    public AudioSource musicaFondo;
    public AudioClip sonidoHover;
    public AudioClip sonidoClick;

    private AudioSource _sfxSource;

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

        if (btnNuevaPartida != null) btnNuevaPartida.onClick.AddListener(OnNuevaPartida);
        if (btnOpciones != null) btnOpciones.onClick.AddListener(OnOpciones);
        if (btnCreditos != null) btnCreditos.onClick.AddListener(OnCreditos);
        if (btnSalir != null) btnSalir.onClick.AddListener(OnSalir);

        if (btnDiscord != null) btnDiscord.onClick.AddListener(() => AbrirURL(urlDiscord));
        if (btnTwitterX != null) btnTwitterX.onClick.AddListener(() => AbrirURL(urlTwitterX));
        if (btnYouTube != null) btnYouTube.onClick.AddListener(() => AbrirURL(urlYouTube));
        if (btnSteam != null) btnSteam.onClick.AddListener(() => AbrirURL(urlSteam));
    }

    // ── NUEVA PARTIDA ──────────────────────────────
    public void OnNuevaPartida()
    {
        SceneManager.LoadScene(1);
    }

    // ── OPCIONES ───────────────────────────────────
    public void OnOpciones()
    {
        if (panelOpciones == null) return;
        bool estaActivo = panelOpciones.activeSelf;
        CerrarPaneles();
        if (!estaActivo) panelOpciones.SetActive(true);
    }

    // ── CREDITOS ───────────────────────────────────
    public void OnCreditos()
    {
        if (panelCreditos == null) return;
        bool estaActivo = panelCreditos.activeSelf;
        CerrarPaneles();
        if (!estaActivo) panelCreditos.SetActive(true);
    }

    // ── SALIR ──────────────────────────────────────
    public void OnSalir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ── REDES SOCIALES ─────────────────────────────
    private void AbrirURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
            Application.OpenURL(url);
    }

    // ── HELPERS ────────────────────────────────────
    private void CerrarPaneles()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelCreditos != null) panelCreditos.SetActive(false);
    }

    // ── FADE IN al abrir el menu ───────────────────
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
}
