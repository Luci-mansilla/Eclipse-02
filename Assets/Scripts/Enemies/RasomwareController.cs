using UnityEngine;

// ================================================================
//  RasomwareController — Movimiento del boss "Rasomware"
//  Patrol AUTOMÁTICO: no hace falta armar waypoints a mano.
//  Al arrancar, guarda su posición inicial como "centro" y elige
//  solo puntos random dentro de un radio (wanderRadius). Al llegar,
//  espera un rato (waitTime) y elige otro punto random. Loop infinito.
//
//  Vuela sin gravedad (gravityScale = 0) y expone attackingOverride
//  para que RasomwareAttack le frene el movimiento mientras ataca.
// ================================================================
public class RasomwareController : MonoBehaviour
{
    [Header("=== PATROL AUTOMÁTICO (wander) ===")]
    [Tooltip("Radio alrededor del punto de spawn dentro del cual elige destinos random.")]
    public float wanderRadius = 5f;
    public float moveSpeed = 2.5f;
    [Tooltip("Distancia mínima al destino para considerarlo 'alcanzado'.")]
    public float arriveThreshold = 0.2f;
    [Tooltip("Segundos que espera parado antes de elegir el próximo destino.")]
    public float waitTimeMin = 1f;
    public float waitTimeMax = 3f;
    [Tooltip("Si tarda más de esto en llegar (por quedar trabado contra una pared, por ejemplo), aborta y elige otro destino.")]
    public float stuckTimeout = 4f;

    [Header("=== ANIMATOR — Parámetros Float (movimiento) ===")]
    public string paramHorizontal = "Horizontal";
    public string paramVertical = "Vertical";
    public string paramSpeed = "Speed";

    // Lo controla RasomwareAttack: mientras castea, el patrol se frena
    [HideInInspector] public bool attackingOverride = false;

    private Animator anim;
    private Rigidbody2D rb;

    private Vector2 spawnPoint;
    private Vector2 currentTarget;
    private float waitTimer = 0f;
    private float stuckTimer = 0f;
    private bool waiting = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Vuela: nunca cae por gravedad
        if (rb != null)
            rb.gravityScale = 0f;

        spawnPoint = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        if (attackingOverride)
        {
            // Mientras ataca, RasomwareAttack maneja su propia velocidad
            return;
        }

        if (waiting)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            SetBlend(Vector2.zero);

            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                PickNewTarget();
            }
            return;
        }

        MoveToTarget();
    }

    void MoveToTarget()
    {
        Vector2 toTarget = currentTarget - (Vector2)transform.position;

        if (toTarget.magnitude <= arriveThreshold)
        {
            StartWaiting();
            return;
        }

        // Si tarda demasiado (trabado contra una pared), aborta y busca otro destino
        stuckTimer += Time.deltaTime;
        if (stuckTimer >= stuckTimeout)
        {
            PickNewTarget();
            return;
        }

        Vector2 dir = toTarget.normalized;

        if (rb != null)
            rb.linearVelocity = dir * moveSpeed;
        else
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);

        SetBlend(dir);
    }

    void StartWaiting()
    {
        waiting = true;
        waitTimer = Random.Range(waitTimeMin, waitTimeMax);
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    // Elige un punto random dentro del radio de wander, centrado en el spawn
    void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        currentTarget = spawnPoint + randomOffset;
        stuckTimer = 0f;
        waiting = false;
    }

    // Envía la dirección al Blend Tree (Horizontal, Vertical, Speed)
    void SetBlend(Vector2 dir)
    {
        if (anim == null) return;
        anim.SetFloat(paramHorizontal, dir.x);
        anim.SetFloat(paramVertical, dir.y);
        anim.SetFloat(paramSpeed, dir.magnitude);
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja el área de wander para visualizarla en el editor
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Vector3 center = Application.isPlaying ? (Vector3)spawnPoint : transform.position;
        Gizmos.DrawWireSphere(center, wanderRadius);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(currentTarget, 0.15f);
        }
    }
}
