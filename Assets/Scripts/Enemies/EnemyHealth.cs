using System.Collections;
using UnityEngine;

// ================================================================
//  EnemyHealth — Vida del enemigo, con animaciones Hurt y Death
//  Usa los Bools del Animator: IsHurt, IsDeath
//
//  MEJORAS:
//   • Flicker del sprite al recibir daño (parpadeo visual)
//   • Knockback (retroceso) en dirección opuesta al atacante
// ================================================================
public class EnemyHealth : MonoBehaviour
{
    [Header("=== VIDA ===")]
    public float maxHealth = 50f;
    public float currentHealth;

    [Header("=== ANIMATOR — Parámetros Bool ===")]
    public string paramIsHurt = "IsHurt";
    public string paramIsDeath = "IsDeath";

    [Header("=== TIEMPOS ===")]
    [Tooltip("Cuánto dura el Bool IsHurt activado (debe ser corto, solo dispara la transición)")]
    public float hurtFlagDuration = 0.1f;

    [Tooltip("Segundos antes de destruir el objeto tras morir (para que se vea la animación)")]
    public float destroyDelay = 2f;

    // -----------------------------------------------------------------
    [Header("=== FLICKER (parpadeo al recibir daño) ===")]
    [Tooltip("Duración total del parpadeo en segundos")]
    public float flickerDuration = 0.4f;

    [Tooltip("Intervalo entre cada cambio de visibilidad (menor = más rápido)")]
    public float flickerInterval = 0.05f;

    // -----------------------------------------------------------------
    [Header("=== KNOCKBACK (retroceso al recibir daño) ===")]
    [Tooltip("Fuerza del retroceso")]
    public float knockbackForce = 6f;

    [Tooltip("Duración del retroceso en segundos")]
    public float knockbackDuration = 0.2f;

    // -----------------------------------------------------------------
    [Header("=== COLOR AL RECIBIR DAÑO ===")]
    [Tooltip("Color que toma el sprite al recibir daño (rojizo por defecto)")]
    public Color hitColor = new Color(1f, 0.2f, 0.2f, 1f);

    [Tooltip("Cuántos segundos dura el color rojizo antes de volver al normal")]
    public float hitColorDuration = 0.15f;

    // ── Privadas ──
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;
    private bool inKnockback = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    // ----------------------------------------------------------------
    //  TakeDamage — llamado por quien ataque al enemigo.
    //  attackerPosition: posición del atacante para calcular knockback.
    //  Si no se pasa, el knockback se omite.
    // ----------------------------------------------------------------
    public void TakeDamage(float amount, Vector2 attackerPosition = default)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " recibió daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(PlayHurt());
            StartCoroutine(PlayFlicker());
            StartCoroutine(PlayHitColor());

            // Solo aplica knockback si se pasó una posición válida
            if (attackerPosition != default && rb != null && !inKnockback)
                StartCoroutine(ApplyKnockback(attackerPosition));
        }
    }

    // ── Parpadeo del sprite ───────────────────────────────────────
    IEnumerator PlayFlicker()
    {
        if (spriteRenderer == null) yield break;

        float elapsed = 0f;
        bool visible = false;

        while (elapsed < flickerDuration)
        {
            spriteRenderer.enabled = visible;
            visible = !visible;
            elapsed += flickerInterval;
            yield return new WaitForSeconds(flickerInterval);
        }

        // Siempre termina visible
        spriteRenderer.enabled = true;
    }

    // ── Tinte rojizo al recibir daño ─────────────────────────────
    IEnumerator PlayHitColor()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = hitColor;

        yield return new WaitForSeconds(hitColorDuration);

        // Restaura el color original solo si el sprite sigue activo
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    // ── Retroceso físico ─────────────────────────────────────────
    IEnumerator ApplyKnockback(Vector2 attackerPosition)
    {
        inKnockback = true;

        Vector2 dir = ((Vector2)transform.position - attackerPosition).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        if (!isDead)
            rb.linearVelocity = Vector2.zero;

        inKnockback = false;
    }

    // ── Animación Hurt ────────────────────────────────────────────
    IEnumerator PlayHurt()
    {
        if (anim != null) anim.SetBool(paramIsHurt, true);

        yield return new WaitForSeconds(hurtFlagDuration);

        if (anim != null) anim.SetBool(paramIsHurt, false);
    }

    // ── Muerte ────────────────────────────────────────────────────
    void Die()
    {
        isDead = true;

        // Detiene cualquier flicker activo y asegura visibilidad y color final
        StopAllCoroutines();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = Color.white;
        }

        if (anim != null) anim.SetBool(paramIsDeath, true);

        if (rb != null) rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        EnemyPatrol patrol = GetComponent<EnemyPatrol>();
        if (patrol != null) patrol.enabled = false;

        EnemyAttack attack = GetComponent<EnemyAttack>();
        if (attack != null) attack.enabled = false;

        Debug.Log(gameObject.name + " ha muerto.");

        Destroy(gameObject, destroyDelay);
    }

    public bool IsDead() => isDead;
}
