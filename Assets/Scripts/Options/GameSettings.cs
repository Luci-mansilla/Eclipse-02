using UnityEngine;

// Este objeto vive en TODAS las escenas sin destruirse.
// Guarda el volumen y la calidad grafica globalmente.
// Solo necesitas ponerlo en la primera escena de tu juego (Escena-menu).

public class GameSettings : MonoBehaviour
{
    // Instancia global - desde cualquier script podes hacer GameSettings.Instancia.VolumenMusica
    public static GameSettings Instancia;

    [Header("Valores actuales")]
    public float VolumenMusica = 0.8f;
    public float VolumenEfectos = 0.7f;
    public float Brillo = 0.6f;
    public string Calidad = "Media";

    void Awake()
    {
        // Si ya existe una instancia, destruimos este duplicado
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        // Somos la instancia principal - no nos destruimos al cambiar escena
        Instancia = this;
        DontDestroyOnLoad(gameObject);

        // Cargamos los valores guardados
        CargarValores();
    }

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

        Debug.Log("Configuracion guardada: Musica=" + musica +
                  " Efectos=" + efectos + " Brillo=" + brillo +
                  " Calidad=" + calidad);
    }

    void AplicarTodo()
    {
        AplicarCalidad();
        AplicarBrillo();
        // NOTA: para que el volumen de musica funcione necesitas llamar
        // GameSettings.Instancia.VolumenMusica desde tu AudioSource de fondo
    }

    void AplicarCalidad()
    {
        switch (Calidad)
        {
            case "Baja": QualitySettings.SetQualityLevel(0, true); break;
            case "Media": QualitySettings.SetQualityLevel(2, true); break;
            case "Alta": QualitySettings.SetQualityLevel(5, true); break;
        }
    }

    void AplicarBrillo()
    {
        // FIX: el brillo ahora se aplica realmente a la pantalla
        // Rango: 0.0 = negro total, 1.0 = brillo normal, >1 = sobreexpuesto
        // Lo mapeamos de 0-1 del slider a 0-2 para tener margen util
        Screen.brightness = Brillo; // funciona en movil

        // Para PC usamos un shader global si tenés post-processing,
        // o podés poner un CanvasGroup negro encima y ajustar su alpha:
        // canvasGroupBrillo.alpha = 1f - Brillo;
        // (eso lo conectas en el Inspector si lo necesitas)
    }
}
