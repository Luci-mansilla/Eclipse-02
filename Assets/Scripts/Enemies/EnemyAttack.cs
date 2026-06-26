using UnityEngine;

[RequireComponent(typeof(EnemyPatrol))]
public class EnemyAttack : MonoBehaviour
{
    [Header("=== DETECCIÓN ===")]
    public float physicalAttackRange = 2f;
    public float hideDetectRange = 5f;
    public Transform player;

    [Header("=== PERSPECTIVA ISOMÉTRICA ===")]
    public float isometricYScale = 0.5f;

    [Header("=== ATAQUE FÍSICO ===")]
    public float physicalDamage = 15f;
    public float physicalCooldown = 2f;
    public float lungeSpeed = 7f;
    public float lungeDuration = 0.4f;

    [Header("=== ATAQUE SUBTERRÁNEO ===")]
    public float undergroundDamage = 25f;
    public float undergroundCooldown = 6f;
    public float undergroundWaitTime = 3f;     // Debe ser MAYOR a la duración de tu clip Underground-X
    public float surfaceAttackRadius = 1.2f;
    public float undergroundSpeed = 3f;

    [Header("=== ANIMATOR — Parámetros Float (movimiento) ===")]
    public string paramHorizontal = "Horizontal";
    public string paramVertical = "Vertical";
    public string paramSpeed = "Speed";

    [Header("=== ANIMATOR — Parámetros Bool (ataques) ===")]
    public string paramIsAttacking = "IsAttacking";   // Ataque físico
    public string paramIsHidding = "IsHidding";     // Ataque subterráneo

    [Header("=== SONIDO DE ATAQUE ===")]
    public AudioSource audioSource;
    public AudioClip attackSound;  

    // ── Privadas ──
    private Animator anim;
    private Rigidbody2D rb;
    private EnemyPatrol patrol;
    private PlayerHealth playerHealth;
    private EnemyHealth selfHealth;   // Para chequear si este enemigo ya murió

    private float physicalTimer = 0f;
    private float undergroundTimer = 0f;
    private float stateTimer = 0f;
    private bool damageDone = false;
    private bool inAttackRange = false;   // ← LÍNEA NUEVA

    private enum State { Patrolling, PhysicalAttack, HidingPrep, Underground }
    private State current = State.Patrolling;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        patrol = GetComponent<EnemyPatrol>();
        selfHealth = GetComponent<EnemyHealth>();

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
        else
            Debug.LogWarning("Asigná el Player en el Inspector de EnemyAttack.");
    }

    void Update()
    {
        if (player == null) return;

        // Si el enemigo está muerto, no hace nada (EnemyHealth ya desactivó este script,
        // pero por seguridad chequeamos también aquí)
        if (selfHealth != null && selfHealth.IsDead()) return;

        physicalTimer -= Time.deltaTime;
        undergroundTimer -= Time.deltaTime;

        float dist = IsoDist(transform.position, player.position);

        switch (current)
        {
            case State.Patrolling: Patrolling(dist); break;
            case State.PhysicalAttack: PhysicalAttack(); break;
            case State.HidingPrep: HidingPrep(); break;
            case State.Underground: Underground(); break;
        }
    }

    // ── PATRULLANDO ───────────────────────────────────────────────
    void Patrolling(float dist)
    {
        patrol.attackingOverride = false;

        if (dist <= physicalAttackRange && physicalTimer <= 0f)
        { GoTo(State.PhysicalAttack); return; }

        if (dist <= hideDetectRange && dist > physicalAttackRange && undergroundTimer <= 0f)
            GoTo(State.HidingPrep);
    }

    // ── ATAQUE FÍSICO ─────────────────────────────────────────────
    void PhysicalAttack()
    {
        patrol.attackingOverride = true;
        SetBool(paramIsAttacking, true);

        if (!damageDone && audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        stateTimer -= Time.deltaTime;

        if (stateTimer > lungeDuration * 0.5f)
        {
            Vector2 dir = IsoDir(transform.position, player.position);
            rb.linearVelocity = dir * lungeSpeed;

            // Actualiza el Blend Tree con la dirección del lanzamiento
            SetBlend(dir);
        }
        else if (stateTimer > 0f)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            physicalTimer = physicalCooldown;
            rb.linearVelocity = Vector2.zero;
            damageDone = false;
            SetBool(paramIsAttacking, false);
            GoTo(State.Patrolling);
        }
    }

    // ── SE ESCONDE (preparación) ─────────────────────────────────
    void HidingPrep()
    {
        patrol.attackingOverride = true;
        rb.linearVelocity = Vector2.zero;
        SetBool(paramIsHidding, true);
        SetBlend(IsoDir(transform.position, player.position)); // ← LÍNEA NUEVA

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
            GoTo(State.Underground);
    }

    // ── BAJO TIERRA ───────────────────────────────────────────────
    // El enemigo persigue al jugador moviéndose. Apenas se acerca lo
    // suficiente, se queda fijo en el lugar (sin cambiar de Motion)
    // para que la animación de Underground-X se reproduzca COMPLETA,
    // ya que el ataque está incluido al final del mismo clip.
    void Underground()
    {
        float dist = IsoDist(transform.position, player.position);

        if (dist > surfaceAttackRadius)
        {
            // Solo persigue si AÚN no llegó al player
            if (!inAttackRange)
            {
                Vector2 dir = IsoDir(transform.position, player.position);
                rb.linearVelocity = dir * undergroundSpeed;
                SetBlend(dir);
            }
            else
            {
                // Ya llegó antes: se queda fijo aunque el player se haya movido
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            // Llegó al player: guarda que ya estuvo en rango
            inAttackRange = true;
            rb.linearVelocity = Vector2.zero;
        }

        stateTimer -= Time.deltaTime;

        // Aplica daño si llegó al rango EN ALGÚN MOMENTO (no solo ahora)
        if (!damageDone && stateTimer <= 0.4f && inAttackRange)
        {
            if (playerHealth != null)
                playerHealth.TakeDamage(undergroundDamage);
            damageDone = true;
        }

        if (stateTimer <= 0f)
        {
            undergroundTimer = undergroundCooldown;
            damageDone = false;
            inAttackRange = false;    // ← Reset para el próximo ataque
            SetBool(paramIsHidding, false);
            GoTo(State.Patrolling);
        }
    }

    // ── DAÑO POR CONTACTO (durante el lanzamiento físico) ─────────
    void OnCollisionEnter2D(Collision2D col)
    {
        if (current != State.PhysicalAttack || damageDone) return;

        if (col.gameObject.CompareTag("Player"))
        {
            playerHealth?.TakeDamage(physicalDamage);
            damageDone = true;
            rb.linearVelocity = Vector2.zero;
        }
    }

    // ── HELPERS ───────────────────────────────────────────────────
    void SetBool(string paramName, bool value)
    {
        if (anim != null) anim.SetBool(paramName, value);
    }

    // Envía la dirección al Blend Tree (Horizontal, Vertical, Speed)
    // Necesario para que las animaciones de ataque muestren la dirección correcta
    void SetBlend(Vector2 dir)
    {
        if (anim == null) return;
        anim.SetFloat(paramHorizontal, dir.x);
        anim.SetFloat(paramVertical, dir.y);
        anim.SetFloat(paramSpeed, dir.magnitude);
    }

    float IsoDist(Vector2 a, Vector2 b)
    {
        float dx = b.x - a.x;
        float dy = (b.y - a.y) / isometricYScale;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    Vector2 IsoDir(Vector2 from, Vector2 to)
    {
        return new Vector2(to.x - from.x,
                          (to.y - from.y) * isometricYScale).normalized;
    }

    void GoTo(State next)
    {
        current = next;
        switch (next)
        {
            case State.PhysicalAttack: stateTimer = lungeDuration + 0.3f; break;
            case State.HidingPrep: stateTimer = 0.8f; break;
            case State.Underground: stateTimer = undergroundWaitTime; break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawIsoCircle(transform.position, physicalAttackRange);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        DrawIsoCircle(transform.position, hideDetectRange);
        Gizmos.color = Color.magenta;
        DrawIsoCircle(transform.position, surfaceAttackRadius);
    }

    void DrawIsoCircle(Vector3 center, float radius)
    {
        int seg = 32;
        float step = 360f / seg;
        Vector3 prev = center + new Vector3(radius, 0, 0);
        for (int i = 1; i <= seg; i++)
        {
            float a = i * step * Mathf.Deg2Rad;
            Vector3 next = center + new Vector3(
                Mathf.Cos(a) * radius,
                Mathf.Sin(a) * radius * isometricYScale, 0);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}