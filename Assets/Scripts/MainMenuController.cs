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

    // FIX: panelCreditos eliminado — créditos ahora usa su propia escena (Escena-creditos)

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
    public string nombreEscenaJuego = "Escena-intro";

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

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
    }

    void Start()
    {
        // Aplicamos el volumen de musica guardado
        if (musicaFondo != null && GameSettings.Instancia != null)
            musicaFondo.volume = GameSettings.Instancia.VolumenMusica;

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

    // ── NUEVA PARTIDA ──────────────────────────────────────────────────────
    public void OnNuevaPartida()
    {
        if (panelFade != null)
            StartCoroutine(FadeOutYCargar(nombreEscenaJuego));
        else
            SceneManager.LoadScene(nombreEscenaJuego);
    }

    // ── OPCIONES ───────────────────────────────────────────────────────────
    public void OnOpciones()
    {
        // FIX: ahora abre la escena dedicada en vez de un panel
        if (panelFade != null)
            StartCoroutine(FadeOutYCargar("Escena-options"));
        else
            SceneManager.LoadScene("Escena-options");
    }

    // ── CREDITOS ───────────────────────────────────────────────────────────
    public void OnCreditos()
    {
        if (panelFade != null)
            StartCoroutine(FadeOutYCargar("Escena-creditos"));
        else
            SceneManager.LoadScene("Escena-creditos");
    }

    // ── SALIR ──────────────────────────────────────────────────────────────
    public void OnSalir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ── REDES SOCIALES ─────────────────────────────────────────────────────
    private void AbrirURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
            Application.OpenURL(url);
    }

    // ── FADE OUT y carga de escena ─────────────────────────────────────────
    private IEnumerator FadeOutYCargar(string escena)
    {
        if (panelFade == null) yield break;
        panelFade.alpha = 0f;
        panelFade.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duracionFade)
        {
            elapsed += Time.deltaTime;
            panelFade.alpha = Mathf.Lerp(0f, 1f, elapsed / duracionFade);
            yield return null;
        }
        panelFade.alpha = 1f;
        SceneManager.LoadScene(escena);
    }

    // ── FADE IN al abrir el menu ───────────────────────────────────────────
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
