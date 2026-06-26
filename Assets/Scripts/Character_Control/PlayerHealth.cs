using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
//   • Sistema de 3 vidas: al morir 3 veces carga Escena-derrota
// ================================================================
public class PlayerHealth : MonoBehaviour
{
    [Header("=== VIDA ===")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("=== DEFENSA ===")]
    [Range(0f, 1f)]
    public float damageReduction = 0f;

    // -----------------------------------------------------------------
    [Header("=== VIDAS ===")]
    [Tooltip("Cantidad de vidas del jugador")]
    public int maxLives = 3;
    public int currentLives;

    [Tooltip("Punto de reaparición (opcional). Si no se asigna, reaparece en la posición inicial.")]
    public Transform respawnPoint;

    [Tooltip("Segundos de espera antes de revivir al jugador")]
    public float respawnDelay = 1.5f;

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

    [Header("=== SONIDOS ===")]
    [Tooltip("Audio Source del jugador")]
    public AudioSource audioSource;

    [Tooltip("Sonido al recibir daño")]
    public AudioClip hurtSound;

    [Tooltip("Sonido al morir")]
    public AudioClip deathSound;

    // ── Privadas ──
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Coroutine invincibilityCoroutine;
    private Color originalColor;
    private Vector3 startPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        currentHealth = maxHealth;
        currentLives = maxLives;

        // Guardamos posición inicial por si no hay respawnPoint asignado
        startPosition = transform.position;

        // Guardamos el color original una sola vez al inicio
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (CheckpointManager.checkpointActivo &&
           CheckpointManager.escenaCheckpoint == SceneManager.GetActiveScene().name)
        {
           transform.position = CheckpointManager.posicionCheckpoint;
           respawnPoint = null;

           Debug.Log("Player apareció en checkpoint: " + CheckpointManager.posicionCheckpoint);
        }

    }

    // ----------------------------------------------------------------
    //  TakeDamage — llamado por los enemigos para hacer daño al jugador
    // ----------------------------------------------------------------
    public void TakeDamage(float amount)
    {
        if (isInvincible) return;

        float finalDamage = amount * (1f - damageReduction);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player recibió daño. Vida restante: " + currentHealth);

        // Reproduce sonido de daño
        if (audioSource != null && hurtSound != null)
            audioSource.PlayOneShot(hurtSound);

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
        SetEnemyCollisions(false);

        StartCoroutine(PlayHitColor());
        yield return StartCoroutine(PlayFlicker());

        SetEnemyCollisions(true);

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

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    // ── Parpadeo del sprite ───────────────────────────────────────
    IEnumerator PlayFlicker()
    {
        if (spriteRenderer == null) yield break;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < flickerDuration)
        {
            spriteRenderer.enabled = visible;
            visible = !visible;
            elapsed += flickerInterval;
            yield return new WaitForSeconds(flickerInterval);
        }

        spriteRenderer.enabled = true;
    }

    // ── Activa / desactiva colisiones con la capa de enemigos ─────
    void SetEnemyCollisions(bool enable)
    {
        int playerLayerIndex = gameObject.layer;
        int layerMaskValue = enemyLayer.value;

        for (int i = 0; i < 32; i++)
        {
            if ((layerMaskValue & (1 << i)) != 0)
                Physics2D.IgnoreLayerCollision(playerLayerIndex, i, !enable);
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
        // Cancela invencibilidad activa
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = null;
        }

        SetEnemyCollisions(true);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }

        isInvincible = false;

        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // Descuenta una vida
        currentLives--;
        Debug.Log("¡El jugador murió! Vidas restantes: " + currentLives);

        if (currentLives <= 0)
        {
            // Sin vidas → pantalla de derrota
            Debug.Log("Game Over. Cargando Escena-derrota...");
            Time.timeScale = 1f;
            SceneManager.LoadScene("Escena-derrota");
        }
       else
       {
           // Quedan vidas → vuelve al checkpoint guardado
           if (CheckpointManager.checkpointActivo)
           {
               SceneManager.LoadScene(CheckpointManager.escenaCheckpoint);
           }
           else
           {
              StartCoroutine(Respawn());
           }
           }

    }

    // ── Reaparición ───────────────────────────────────────────────
    IEnumerator Respawn()
    {
        // Ocultamos el sprite durante la espera
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Teletransporta al punto de reaparición (o posición inicial)
       Vector3 destino = respawnPoint != null ? respawnPoint.position : startPosition;

       Debug.Log("Respawneando en: " + destino);

       transform.position = destino;

        // Restaura la vida completa
        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.value = currentHealth;

        // Reactiva el sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }

        // Pequeña invencibilidad de entrada para no morir al aparecer
        invincibilityCoroutine = StartCoroutine(InvincibilitySequence());

        Debug.Log("Player revivió. Vidas restantes: " + currentLives);
    }
}

