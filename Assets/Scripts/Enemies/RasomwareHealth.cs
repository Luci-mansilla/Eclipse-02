using System.Collections;
using UnityEngine;

// ================================================================
//  RasomwareHealth — Vida de Rasomware, con animaciones Hurt y Death
//  Clase propia e independiente de EnemyHealth (no comparte tipo
//  con los demás enemigos, así no se mezclan).
//  Usa los Bools del Animator: IsHurt, IsDeath
//
//  FUNCIONALIDADES:
//   • Flicker del sprite al recibir daño (parpadeo visual)
//   • Knockback (retroceso) en dirección opuesta al atacante
//   • FIX: el flicker arranca con visible=true para no empezar apagado
//   • FIX: el color original se guarda en Start() para restaurarlo bien
//   • FIX: la muerte llama StopAllCoroutines() antes de cualquier lógica
//          para evitar que el flicker o el color queden en estado raro
// ================================================================
public class RasomwareHealth : MonoBehaviour, IDamageable
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

    [Header("=== SONIDOS ===")]
    public AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    [Range(0f, 1f)]
    public float hurtVolume = 0.5f;

    [Range(0f, 1f)]
    public float deathVolume = 0.7f;

    [Tooltip("Cuántos segundos dura el color rojizo antes de volver al normal")]
    public float hitColorDuration = 0.15f;

    // ── Privadas ──
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;
    private bool inKnockback = false;
    private Color originalColor;   // FIX: guardado en Start()

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        // FIX: guardamos el color original una sola vez
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // ----------------------------------------------------------------
    //  TakeDamage — llamado por quien ataque al enemigo.
    //  attackerPosition: posición del atacante para calcular knockback.
    //  Si no se pasa, el knockback se omite.
    // ----------------------------------------------------------------
    // agregado: firma requerida por IDamageable (int damage, Vector3 origin, float knockbackMultiplier)
    public void TakeDamage(
        int damage,
        Vector3 origin,
        float knockbackMultiplier = 1f
    )
    {
        TakeDamage((float)damage, (Vector2)origin, knockbackMultiplier);
    }
    // finaliza agregado

    public void TakeDamage(
        float amount,
        Vector2 attackerPosition = default,
        float knockbackMultiplier = 1f
    )
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " recibió daño. Vida restante: " + currentHealth);

        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound, hurtVolume);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(PlayHurt());
            StartCoroutine(PlayFlicker());
            StartCoroutine(PlayHitColor());

            // FIX: se agregaron las llaves. Antes el if solo controlaba el Debug.Log
            // y el StartCoroutine(ApplyKnockback(...)) se ejecutaba SIEMPRE,
            // incluso con rb == null (crasheaba) o en pleno knockback.
            if (attackerPosition != default && rb != null && !inKnockback)
            {
                Debug.Log("Multiplicador recibido: " + knockbackMultiplier);
                StartCoroutine(ApplyKnockback(attackerPosition, knockbackMultiplier));
            }
        }
    }

    // ── Parpadeo del sprite ───────────────────────────────────────
    // FIX: arranca con visible = true para que el primer frame no
    //      apague el sprite antes de que el jugador lo vea
    IEnumerator PlayFlicker()
    {
        if (spriteRenderer == null) yield break;

        float elapsed = 0f;
        bool visible = true;   // <-- FIX: empieza visible

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
    // FIX: usa originalColor guardado en Start en lugar de leerlo
    //      en el momento (que podría ser el hitColor si recibe 2 golpes seguidos)
    IEnumerator PlayHitColor()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = hitColor;

        yield return new WaitForSeconds(hitColorDuration);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;   // <-- FIX
    }

    // ── Retroceso físico ─────────────────────────────────────────
    IEnumerator ApplyKnockback(
        Vector2 attackerPosition,
        float knockbackMultiplier
    )
    {
        inKnockback = true;

        RasomwareController controller = GetComponent<RasomwareController>();
        RasomwareAttack attack = GetComponent<RasomwareAttack>();

        // Detiene temporalmente los scripts que controlan el movimiento
        if (controller != null)
            controller.enabled = false;

        if (attack != null)
            attack.enabled = false;

        Vector2 dir = ((Vector2)transform.position - attackerPosition).normalized;

        rb.linearVelocity = Vector2.zero;

        Debug.Log("Knockback aplicado. Multiplicador recibido: "
        + knockbackMultiplier);

        rb.AddForce(
            dir * knockbackForce * knockbackMultiplier,
            ForceMode2D.Impulse
        );

        // Ahora el enemigo tarda más en frenarse
        yield return new WaitForSeconds(knockbackDuration);

        if (!isDead)
            rb.linearVelocity = Vector2.zero;

        // Reactiva el comportamiento del enemigo
        if (controller != null)
            controller.enabled = true;

        if (attack != null)
            attack.enabled = true;

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

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound, deathVolume);
        }

        // FIX: StopAllCoroutines primero para que ninguna corrutina
        //      pise el estado visual después de la muerte
        StopAllCoroutines();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;   // <-- FIX: color limpio
        }

        if (anim != null) anim.SetBool(paramIsDeath, true);

        if (rb != null) rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        RasomwareController controller = GetComponent<RasomwareController>();
        if (controller != null) controller.enabled = false;

        RasomwareAttack attack = GetComponent<RasomwareAttack>();
        if (attack != null) attack.enabled = false;

        Debug.Log(gameObject.name + " ha muerto.");

        Destroy(gameObject, destroyDelay);
    }

    public bool IsDead() => isDead;
    public bool IsInKnockback() => inKnockback;
}
