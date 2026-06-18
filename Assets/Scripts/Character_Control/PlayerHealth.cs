using UnityEngine;
using UnityEngine.UI;   // Para la barra de vida (opcional)

public class PlayerHealth : MonoBehaviour
{
    [Header("=== VIDA ===")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("=== INVENCIBILIDAD POST-DAÑO ===")]
    public float invincibleTime = 1f;      // Segundos de invencibilidad tras recibir daño
    private float invincibleTimer = 0f;
    public bool isInvincible = false;

    [Header("=== UI (opcional) ===")]
    public Slider healthBar;               // Arrastrá acá un Slider de la UI si tenés

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void Update()
    {
        // Cuenta regresiva de invencibilidad
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
                isInvincible = false;
        }
    }

    // Esta función la llaman los enemigos para hacerte daño
    public void TakeDamage(float amount)
    {
        if (isInvincible) return;   // Si sos invencible, no recibís daño

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player recibió daño. Vida restante: " + currentHealth);

        // Activa invencibilidad temporal
        isInvincible = true;
        invincibleTimer = invincibleTime;

        // Actualiza barra de vida si existe
        if (healthBar != null)
            healthBar.value = currentHealth;

        // ¿Murió?
        if (currentHealth <= 0)
            Die();
    }

    // Curarse (por si necesitás items de vida después)
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    void Die()
    {
        Debug.Log("¡El jugador murió!");
        // Acá después podés: mostrar pantalla de Game Over, recargar escena, etc.
        // Ejemplo: SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
