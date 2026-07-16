using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GlitchManager : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Escena a cargar al terminar")]
    [SerializeField] private string escenaMenu = "Escena-menu";

    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer == null)
        {
            Debug.LogError("No se encontró ningún componente VideoPlayer.");
            return;
        }

        videoPlayer.loopPointReached += VideoTerminado;
        videoPlayer.errorReceived += ErrorVideo;

        videoPlayer.Play();
    }

    private void VideoTerminado(VideoPlayer vp)
    {
        Debug.Log("Terminó el video. Cargando: " + escenaMenu);
        SceneManager.LoadScene(escenaMenu);
    }

    private void ErrorVideo(VideoPlayer vp, string mensaje)
    {
        Debug.LogError("Error al reproducir el video: " + mensaje);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= VideoTerminado;
            videoPlayer.errorReceived -= ErrorVideo;
        }
    }
}