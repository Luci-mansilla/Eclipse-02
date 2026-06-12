using UnityEngine;
public class CameraFollow : MonoBehaviour
{
  public Transform objetivo;
  public float suavizado = 0.15f;
  public Vector3 offset;
  public Vector2 minLimite; // esquina inferior izquierda del mapa
  public Vector2 maxLimite; // esquina superior derecha del mapa
  private Vector3 velocidad = Vector3.zero;
  private Camera cam;
  void Start()
  {
    cam = GetComponent<Camera>();
  }
  void LateUpdate()
  {
    if (objetivo == null) return;
    Vector3 objetivoPos = objetivo.position + offset;
    // Calculamos el tamaño visible de la cámara
    float alturaCam = cam.orthographicSize;
    float anchoCam = alturaCam * cam.aspect;
    // Limitamos la posición de la cámara para que no pase los bordes
    float posX = Mathf.Clamp(objetivoPos.x, minLimite.x + anchoCam, maxLimite.x - anchoCam);
    float posY = Mathf.Clamp(objetivoPos.y, minLimite.y + alturaCam, maxLimite.y - alturaCam);
    Vector3 posLimitada = new Vector3(posX, posY, objetivoPos.z);
    transform.position = Vector3.SmoothDamp(transform.position, posLimitada, ref velocidad, suavizado);
  }
}