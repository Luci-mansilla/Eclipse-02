using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("=== VELOCIDAD ===")]
    public float speed = 2f;                  // Qué tan rápido camina

    [Header("=== PATRULLA ===")]
    public float waitTime = 1.5f;             // Segundos que espera en cada punto
    public float detectionRadius = 0.3f;      // Qué tan cerca del punto para cambiar

    [Header("=== AREA DE PATRULLA ===")]
    public float areaWidth = 5f;              // Ancho del área de patrulla
    public float areaHeight = 3f;             // Alto del área de patrulla

    // Variables privadas (internas, no las toques)
    private Vector2 targetPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Vector2 startPosition;

    // Todas las 8 direcciones posibles
    private Vector2[] directions = new Vector2[]
    {
        new Vector2(1, 0),      // Derecha
        new Vector2(-1, 0),     // Izquierda
        new Vector2(0, 1),      // Arriba
        new Vector2(0, -1),     // Abajo
        new Vector2(1, 1),      // Diagonal arriba-derecha
        new Vector2(-1, 1),     // Diagonal arriba-izquierda
        new Vector2(1, -1),     // Diagonal abajo-derecha
        new Vector2(-1, -1)     // Diagonal abajo-izquierda
    };

    void Start()
    {
        // Guardamos la posición inicial del enemigo
        startPosition = transform.position;

        // Elegimos el primer punto de patrulla al azar
        ChooseNewTarget();
    }

    void Update()
    {
        if (isWaiting)
        {
            // Está esperando en un punto
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                // Terminó de esperar, elige nuevo destino
                isWaiting = false;
                ChooseNewTarget();
            }
        }
        else
        {
            // Se mueve hacia el punto objetivo
            MoveToTarget();

            // ¿Llegó al punto? (está muy cerca)
            float distanceToTarget = Vector2.Distance(transform.position, targetPoint);
            if (distanceToTarget <= detectionRadius)
            {
                // Llegó, ahora espera
                isWaiting = true;
                waitTimer = waitTime;
            }
        }
    }

    void MoveToTarget()
    {
        // Calcula la dirección hacia el objetivo
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;

        // Mueve el personaje
        transform.Translate(direction * speed * Time.deltaTime);

        // (Opcional) Voltea el sprite según la dirección horizontal
        if (direction.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);   // Mira a la derecha
        else if (direction.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);  // Mira a la izquierda
    }

    void ChooseNewTarget()
    {
        // Elige UNA dirección al azar de las 8 posibles
        Vector2 randomDir = directions[Random.Range(0, directions.Length)];

        // Calcula cuánta distancia caminar en esa dirección (entre 1 y 3 unidades)
        float randomDistance = Random.Range(1f, 3f);

        // El punto objetivo es: posición actual + dirección * distancia
        Vector2 newTarget = (Vector2)transform.position + randomDir * randomDistance;

        // Nos aseguramos que no se salga del área definida
        newTarget.x = Mathf.Clamp(newTarget.x, startPosition.x - areaWidth, startPosition.x + areaWidth);
        newTarget.y = Mathf.Clamp(newTarget.y, startPosition.y - areaHeight, startPosition.y + areaHeight);

        targetPoint = newTarget;
    }

    // Dibuja el área de patrulla en el Editor (solo para ayudarte a verla)
    void OnDrawGizmos()
    {
        // Área de patrulla en AMARILLO
        Gizmos.color = Color.yellow;
        Vector2 center = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Gizmos.DrawWireCube(center, new Vector3(areaWidth * 2, areaHeight * 2, 0));

        // Punto objetivo en ROJO (solo cuando el juego está corriendo)
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPoint, detectionRadius);
        }
    }
}
