using UnityEngine;
using UnityEngine.Events;

// ================================================================
//  RasomwareAttack — Ataque CUERPO A CUERPO del boss "Rasomware"
//  Mismo patrón que PhysicalAttack en EnemyAttack.cs: se lanza
//  (lunge) hacia el player y daña por colisión.
//
//  MECÁNICA ESPECIAL:
//   Cada N golpes conectados (hitsToBlockAbility, default 3) dispara
//   onAbilityBlockTrigger, un UnityEvent<float> con la duración del
//   bloqueo. Enganchalo en el Inspector a tu sistema de habilidades
//   del player (ej: PlayerAbilities.BlockRandomAbility(float duration)).
//   Así no hace falta que este script conozca tu clase de habilidades.
// ================================================================
[RequireComponent(typeof(RasomwareController))]
public class RasomwareAttack : MonoBehaviour
{
    [Header("=== DETECCIÓN ===")]
    public float attackRange = 2f;
    public Transform player;

    [Header("=== ATAQUE CUERPO A CUERPO (lunge) ===")]
    public float damage = 15f;
    public float attackCooldown = 2f;
    public float lungeSpeed = 7f;
    public float lungeDuration = 0.4f;

    [Header("=== BLOQUEO DE HABILIDAD (cada N golpes) ===")]
    public int hitsToBlockAbility = 3;
    public float abilityBlockDuration = 4f;
    [Tooltip("Se invoca cada vez que Rasomware conecta el golpe N, 2N, 3N... Parámetro = duración del bloqueo en segundos.")]
    public UnityEvent<float> onAbilityBlockTrigger;

    [Header("=== ANIMATOR — Parámetros ===")]
    public string paramHorizontal = "Horizontal";
    public string paramVertical = "Vertical";
    public string paramSpeed = "Speed";
    public string paramIsAttacking = "IsAttacking";

    [Header("=== SONIDO ===")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    // ── Privadas ──
    private Animator anim;
    private Rigidbody2D rb;
    private RasomwareController controller;
    private PlayerHealth playerHealth;
    private RasomwareHealth selfHealth;

    private float cooldownTimer = 0f;
    private float stateTimer = 0f;
    private bool damageDone = false;
    private int hitCounter = 0;

    private enum State { Patrolling, Attacking }
    private State current = State.Patrolling;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<RasomwareController>();
        selfHealth = GetComponent<RasomwareHealth>();

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
        else
            Debug.LogWarning("Asigná el Player en el Inspector de RasomwareAttack.");
    }

    void Update()
    {
        if (selfHealth != null && selfHealth.IsInKnockback()) return;
        if (player == null) return;
        if (selfHealth != null && selfHealth.IsDead()) return;

        cooldownTimer -= Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (current)
        {
            case State.Patrolling: Patrolling(dist); break;
            case State.Attacking: Attacking(); break;
        }
    }

    // ── PATRULLANDO ───────────────────────────────────────────────
    void Patrolling(float dist)
    {
        controller.attackingOverride = false;

        if (dist <= attackRange && cooldownTimer <= 0f)
            GoTo(State.Attacking);
    }

    // ── EMBESTIDA (lunge) ────────────────────────────────────────
    void Attacking()
    {
        controller.attackingOverride = true;
        SetBool(paramIsAttacking, true);

        if (!damageDone && audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        stateTimer -= Time.deltaTime;

        if (stateTimer > lungeDuration * 0.5f)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
            if (rb != null) rb.linearVelocity = dir * lungeSpeed;

            SetBlend(dir);
        }
        else if (stateTimer > 0f)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
        else
        {
            cooldownTimer = attackCooldown;
            if (rb != null) rb.linearVelocity = Vector2.zero;
            damageDone = false;
            SetBool(paramIsAttacking, false);
            GoTo(State.Patrolling);
        }
    }

    // ── DAÑO POR CONTACTO (durante la embestida) ────────────────────
    void OnCollisionEnter2D(Collision2D col)
    {
        if (current != State.Attacking || damageDone) return;

        if (col.gameObject.CompareTag("Player"))
        {
            playerHealth?.TakeDamage(damage);
            damageDone = true;
            if (rb != null) rb.linearVelocity = Vector2.zero;

            RegisterHit();
        }
    }

    // ── CONTADOR DE GOLPES → BLOQUEO DE HABILIDAD ───────────────────
    void RegisterHit()
    {
        hitCounter++;

        if (hitCounter >= hitsToBlockAbility)
        {
            hitCounter = 0;
            onAbilityBlockTrigger?.Invoke(abilityBlockDuration);
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

    void GoTo(State next)
    {
        current = next;
        if (next == State.Attacking)
            stateTimer = lungeDuration + 0.3f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
