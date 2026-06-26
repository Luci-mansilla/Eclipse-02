using UnityEngine;

[RequireComponent(typeof(EnemyPatrol))]
public class EnemyAttackTroyanogame : MonoBehaviour
{
    [Header("=== DETECCIÓN ===")]
    public float physicalAttackRange = 2f;
    public Transform player;

    [Header("=== PERSPECTIVA ISOMÉTRICA ===")]
    public float isometricYScale = 0.5f;

    [Header("=== ATAQUE FÍSICO ===")]
    public float physicalDamage = 15f;
    public float physicalCooldown = 2f;
    public float lungeSpeed = 7f;
    public float lungeDuration = 0.4f;

    [Header("=== ANIMATOR — Parámetros Float (movimiento) ===")]
    public string paramHorizontal = "Horizontal";
    public string paramVertical = "Vertical";
    public string paramSpeed = "Speed";

    [Header("=== ANIMATOR — Parámetros Bool (ataques) ===")]
    public string paramIsAttacking = "IsAttacking";

    [Header("=== SONIDO DE ATAQUE ===")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    // ── Privadas ──
    private Animator anim;
    private Rigidbody2D rb;
    private EnemyPatrol patrol;
    private PlayerHealth playerHealth;
    private EnemyHealth selfHealth;

    private float physicalTimer = 0f;
    private float stateTimer = 0f;
    private bool damageDone = false;

    private enum State { Patrolling, PhysicalAttack }
    private State current = State.Patrolling;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        patrol = GetComponent<EnemyPatrol>();
        selfHealth = GetComponent<EnemyHealth>();

        // Si no está asignado en el Inspector, lo busca por tag automáticamente
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
        else
            Debug.LogWarning("No se encontró el Player. Asegurate de que tenga el tag 'Player'.");
    }

    void Update()
    {
        if (player == null) return;

        if (selfHealth != null && selfHealth.IsDead()) return;

        physicalTimer -= Time.deltaTime;

        float dist = IsoDist(transform.position, player.position);

        switch (current)
        {
            case State.Patrolling: Patrolling(dist); break;
            case State.PhysicalAttack: PhysicalAttack(); break;
        }
    }

    // ── PATRULLANDO ───────────────────────────────────────────────
    void Patrolling(float dist)
    {
        patrol.attackingOverride = false;

        if (dist <= physicalAttackRange && physicalTimer <= 0f)
            GoTo(State.PhysicalAttack);
    }

    // ── ATAQUE FÍSICO ─────────────────────────────────────────────
    void PhysicalAttack()
    {
        patrol.attackingOverride = true;
        SetBool(paramIsAttacking, true);

        if (!damageDone && audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);

        stateTimer -= Time.deltaTime;

        if (stateTimer > lungeDuration * 0.5f)
        {
            Vector2 dir = IsoDir(transform.position, player.position);
            rb.linearVelocity = dir * lungeSpeed;
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
        if (next == State.PhysicalAttack)
            stateTimer = lungeDuration + 0.3f;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawIsoCircle(transform.position, physicalAttackRange);
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

