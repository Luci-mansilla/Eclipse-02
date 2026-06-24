using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroController : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;

    [Header("Nombre de Escena Destino")]
    public string nombreEscenaDestino = "Escena-1";

    [Header("Transicion de Pantalla")]
    public CanvasGroup panelFade;
    public float duracionFade = 0.8f;

    [Header("Permitir Saltar el Video")]
    public bool permitirSaltar = true;
    [Tooltip("Cualquier tecla o clic salta el video si permitirSaltar esta activo")]
    public KeyCode teclaSaltar = KeyCode.Space;

    private bool _cargando = false;

    void Start()
    {
        // Fade in al entrar a la escena intro
        if (panelFade != null)
            StartCoroutine(FadeIn());

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoTerminado;

            // Preparar y reproducir
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += (vp) => vp.Play();
        }
        else
        {
            Debug.LogWarning("IntroController: no hay VideoPlayer asignado. Cargando escena directo.");
            IrAJuego();
        }
    }

    void Update()
    {
        if (_cargando) return;

        if (permitirSaltar && (Input.GetKeyDown(teclaSaltar) || Input.GetMouseButtonDown(0)))
            IrAJuego();
    }

    // Llamado automaticamente cuando el video llega al final
    private void OnVideoTerminado(VideoPlayer vp)
    {
        IrAJuego();
    }

    public void IrAJuego()
    {
        if (_cargando) return;
        _cargando = true;

        if (videoPlayer != null) videoPlayer.Stop();

        if (panelFade != null)
            StartCoroutine(FadeOutYCargar());
        else
            SceneManager.LoadScene(nombreEscenaDestino);
    }

    // ── FADE IN al entrar ──────────────────────────
    private IEnumerator FadeIn()
    {
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

    // ── FADE OUT y carga de Escena-1 ───────────────
    private IEnumerator FadeOutYCargar()
    {
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
        SceneManager.LoadScene(nombreEscenaDestino);
    }
}
