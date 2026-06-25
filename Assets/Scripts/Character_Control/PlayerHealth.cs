using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// ================================================================
//  PlayerHealth — Vida del jugador
//
//  FUNCIONALIDADES:
//   • Flicker rápido del sprite al recibir daño
//   • Invencibilidad post-daño con colisiones desactivadas (5 seg),
//     igual al efecto clásico de Sonic: el jugador atraviesa enemigos
//     y no puede volver a recibir daño hasta que termina el período.
//   • FIX: el flicker arranca visible=true para no empezar apagado
//   • FIX: el color rojizo se restaura correctamente aunque el flicker
//     lo haya desactivado el sprite en paralelo
// ================================================================
public class PlayerHealth : MonoBehaviour
{
    [Header("=== VIDA ===")]
    public float maxHealth = 100f;
    public float currentHealth;

    // -----------------------------------------------------------------
    [Header("=== INVENCIBILIDAD POST-DAÑO ===")]
    [Tooltip("Segundos de invencibilidad tras recibir daño (colisiones OFF)")]
    public float invincibleTime = 5f;
    public bool isInvincible = false;

    // -----------------------------------------------------------------
    [Header("=== FLICKER (parpadeo al recibir daño) ===")]
    [Tooltip("Duración total del parpadeo (recomendado: igual a invincibleTime)")]
    public float flickerDuration = 5f;

    [Tooltip("Intervalo entre cada cambio de visibilidad (menor = más rápido)")]
    public float flickerInterval = 0.08f;

    // -----------------------------------------------------------------
    [Header("=== COLOR AL RECIBIR DAÑO ===")]
    [Tooltip("Color que toma el sprite al recibir daño (rojizo por defecto)")]
    public Color hitColor = new Color(1f, 0.2f, 0.2f, 1f);

    [Tooltip("Cuántos segundos dura el color rojizo antes de volver al normal")]
    public float hitColorDuration = 0.15f;

    // -----------------------------------------------------------------
    [Header("=== COLISIÓN CON ENEMIGOS ===")]
    [Tooltip("Layer de los enemigos para ignorar colisiones durante la invencibilidad")]
    public LayerMask enemyLayer;

    // -----------------------------------------------------------------
    [Header("=== UI (opcional) ===")]
    public Slider healthBar;

    // ── Privadas ──
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Coroutine invincibilityCoroutine;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        currentHealth = maxHealth;

        // Guardamos el color original una sola vez al inicio
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // ----------------------------------------------------------------
    //  TakeDamage — llamado por los enemigos para hacer daño al jugador
    // ----------------------------------------------------------------
    public void TakeDamage(float amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player recibió daño. Vida restante: " + currentHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Si ya había una invencibilidad corriendo, la cancela y arranca de nuevo
        if (invincibilityCoroutine != null)
            StopCoroutine(invincibilityCoroutine);

        invincibilityCoroutine = StartCoroutine(InvincibilitySequence());
    }

    // ── Secuencia completa: flicker + colisiones OFF → ON ─────────
    IEnumerator InvincibilitySequence()
    {
        isInvincible = true;

        // Desactiva colisiones con enemigos
        SetEnemyCollisions(false);

        // Tinte rojizo instantáneo al recibir el golpe y flicker en paralelo
        // FIX: corremos ambas corrutinas a la vez pero el flicker respeta
        //      el estado del color, no lo pisa
        StartCoroutine(PlayHitColor());
        yield return StartCoroutine(PlayFlicker());

        // Reactiva colisiones
        SetEnemyCollisions(true);

        // Aseguramos que el sprite quede visible y con su color original
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }

        isInvincible = false;
        invincibilityCoroutine = null;
    }

    // ── Tinte rojizo al recibir daño ─────────────────────────────
    IEnumerator PlayHitColor()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = hitColor;

        yield return new WaitForSeconds(hitColorDuration);

        // Restaura el color original (el flicker sigue parpadeando el enabled,
        // pero el color ya queda en el original)
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    // ── Parpadeo del sprite ───────────────────────────────────────
    // FIX: arranca con visible = true para que el primer estado sea
    //      "apagado" en el siguiente tick, no al inicio
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

    // ── Activa / desactiva colisiones con la capa de enemigos ─────
    //
    //  Usa Physics2D.IgnoreLayerCollision para que el jugador
    //  directamente atraviese a los enemigos (efecto Sonic).
    //
    //  Si preferís solo deshabilitar el collider del player,
    //  reemplazá el cuerpo de esta función por:
    //      if (col != null) col.enabled = enable;
    // ──────────────────────────────────────────────────────────────
    void SetEnemyCollisions(bool enable)
    {
        int playerLayerIndex = gameObject.layer;

        int layerMaskValue = enemyLayer.value;
        for (int i = 0; i < 32; i++)
        {
            if ((layerMaskValue & (1 << i)) != 0)
            {
                // enable = true  → reactiva las colisiones normales
                // enable = false → las ignora (jugador atraviesa enemigos)
                Physics2D.IgnoreLayerCollision(playerLayerIndex, i, !enable);
            }
        }
    }

    // ── Curación ──────────────────────────────────────────────────
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    // ── Muerte ────────────────────────────────────────────────────
    void Die()
    {
        // Cancela cualquier invencibilidad activa
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = null;
        }

        // Reactiva colisiones por si murió durante la invencibilidad
        SetEnemyCollisions(true);

        // Asegura que el sprite quede visible y sin tinte al morir
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }

        isInvincible = false;

        Debug.Log("¡El jugador murió!");
        // Podés agregar acá: pantalla de Game Over, recargar escena, etc.
        // Ejemplo: SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
