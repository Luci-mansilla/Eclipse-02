using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("=== VIDA ===")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("=== DEFENSA ===")]
    [Range(0f, 1f)]
    public float damageReduction = 0f;


    // ================= ESCUDO AGREGADO =================
    [Header("=== ESCUDO ===")]
    public GameObject shieldEffect;
    public KeyCode shieldKey = KeyCode.Mouse0;
    public AudioSource shieldAudioSource;

    public float shieldDuration = 2f;
    public float shieldCooldown = 5f;

    private float shieldCooldownTimer = 0f;
    // ===================================================


    [Header("=== VIDAS ===")]
    public int maxLives = 3;
    public int currentLives;

    public Transform respawnPoint;

    public float respawnDelay = 1.5f;


    [Header("=== INVENCIBILIDAD POST-DAÑO ===")]
    public float invincibleTime = 5f;
    public bool isInvincible = false;


    [Header("=== FLICKER ===")]
    public float flickerDuration = 5f;
    public float flickerInterval = 0.08f;


    [Header("=== COLOR AL RECIBIR DAÑO ===")]
    public Color hitColor = new Color(1f, 0.2f, 0.2f, 1f);
    public float hitColorDuration = 0.15f;


    [Header("=== COLISIÓN CON ENEMIGOS ===")]
    public LayerMask enemyLayer;


    [Header("=== UI (opcional) ===")]
    public Slider healthBar;


    [Header("=== SONIDOS ===")]
    public AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip deathSound;


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

        startPosition = transform.position;


        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;


        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();


        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }


        if (shieldEffect != null)
        {
            shieldEffect.SetActive(false);
        }
    }


    void Update()
    {
        if (shieldCooldownTimer > 0)
        {
            shieldCooldownTimer -= Time.deltaTime;
        }


        if (Input.GetKeyDown(shieldKey) && !isInvincible && shieldCooldownTimer <= 0)
        {
            ActivateShield();
        }
    }


    public void ActivateShield()
    {
        if (invincibilityCoroutine != null)
            StopCoroutine(invincibilityCoroutine);


        shieldCooldownTimer = shieldCooldown;


        if (shieldAudioSource != null)
        {
            shieldAudioSource.Play();
        }


        invincibilityCoroutine = StartCoroutine(ShieldSequence());
    }


    IEnumerator ShieldSequence()
    {
        isInvincible = true;

        SetEnemyCollisions(false);


        if (shieldEffect != null)
        {
            shieldEffect.SetActive(true);
        }


        StartCoroutine(PlayFlicker());


        yield return new WaitForSeconds(shieldDuration);


        SetEnemyCollisions(true);


        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }


        if (shieldEffect != null)
        {
            shieldEffect.SetActive(false);
        }


        isInvincible = false;
        invincibilityCoroutine = null;
    }

     public void TakeDamage(float amount)
    {
        if (isInvincible) return;


        float finalDamage = amount * (1f - damageReduction);

        currentHealth -= finalDamage;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);


        Debug.Log("Player recibió daño. Vida restante: " + currentHealth);


        if (audioSource != null && hurtSound != null)
            audioSource.PlayOneShot(hurtSound);


        if (healthBar != null)
            healthBar.value = currentHealth;


        if (currentHealth <= 0)
        {
            Die();
            return;
        }


        if (invincibilityCoroutine != null)
            StopCoroutine(invincibilityCoroutine);


        invincibilityCoroutine = StartCoroutine(InvincibilitySequence());
    }



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



    IEnumerator PlayHitColor()
    {
        if (spriteRenderer == null)
            yield break;


        spriteRenderer.color = hitColor;


        yield return new WaitForSeconds(hitColorDuration);


        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }



    IEnumerator PlayFlicker()
    {
        if (spriteRenderer == null)
            yield break;


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



    void SetEnemyCollisions(bool enable)
    {
        int playerLayerIndex = gameObject.layer;

        int layerMaskValue = enemyLayer.value;


        for (int i = 0; i < 32; i++)
        {
            if ((layerMaskValue & (1 << i)) != 0)
            {
                Physics2D.IgnoreLayerCollision(
                    playerLayerIndex,
                    i,
                    !enable
                );
            }
        }
    }



    public void Heal(float amount)
    {
        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0,
            maxHealth
        );


        if (healthBar != null)
            healthBar.value = currentHealth;
    }



    void Die()
    {
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = null;
        }


        SetEnemyCollisions(true);


        if (shieldEffect != null)
            shieldEffect.SetActive(false);


        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }


        isInvincible = false;


        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);


        currentLives--;


        if (currentLives <= 0)
        {
            Time.timeScale = 1f;

            SceneManager.LoadScene("Escena-derrota");
        }
        else
        {
            StartCoroutine(Respawn());
        }
    }



    IEnumerator Respawn()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;


        yield return new WaitForSeconds(respawnDelay);


        Vector3 destino =
            respawnPoint != null ?
            respawnPoint.position :
            startPosition;


        transform.position = destino;


        currentHealth = maxHealth;


        if (healthBar != null)
            healthBar.value = currentHealth;


        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }


        invincibilityCoroutine =
            StartCoroutine(InvincibilitySequence());
    }
}