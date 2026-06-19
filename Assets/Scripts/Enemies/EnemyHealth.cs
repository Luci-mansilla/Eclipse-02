using UnityEngine;

// ================================================================
//  EnemyHealth — Vida del enemigo, con animaciones Hurt y Death
//  Usa los Bools del Animator: IsHurt, IsDeath
// ================================================================
public class EnemyHealth : MonoBehaviour
{
    [Header("=== VIDA ===")]
    public float maxHealth = 50f;
    public float currentHealth;

    [Header("=== ANIMATOR — Parámetros Bool ===")]
    public string paramIsHurt  = "IsHurt";
    public string paramIsDeath = "IsDeath";

    [Header("=== TIEMPOS ===")]
    [Tooltip("Cuánto dura el Bool IsHurt activado (debe ser corto, solo dispara la transición)")]
    public float hurtFlagDuration = 0.1f;

    [Tooltip("Segundos antes de destruir el objeto tras morir (para que se vea la animación)")]
    public float destroyDelay = 2f;

    

    // ── Privadas ──
    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        anim          = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // Llamado por quien le haga daño al enemigo (ej: ataque del jugador)
    public void TakeDamage(float amount)
    {

        

        if (isDead) return;

        currentHealth -= amount;
        currentHealth  = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " recibió daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(PlayHurt());
        }
    }

    System.Collections.IEnumerator PlayHurt()
    {
        if (anim != null) anim.SetBool(paramIsHurt, true);

        yield return new WaitForSeconds(hurtFlagDuration);

        // Apagamos el Bool — el Animator ya entró al estado Hurt,
        // y la transición de vuelta usa Exit Time, no este Bool
        if (anim != null) anim.SetBool(paramIsHurt, false);
    }

    void Die()
    {
        isDead = true;

        if (anim != null) anim.SetBool(paramIsDeath, true);

        // Desactiva físicas y colisiones para que no siga interactuando
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Desactiva otros scripts de comportamiento (patrulla y ataque)
        EnemyPatrol patrol = GetComponent<EnemyPatrol>();
        if (patrol != null) patrol.enabled = false;

        EnemyAttack attack = GetComponent<EnemyAttack>();
        if (attack != null) attack.enabled = false;

        Debug.Log(gameObject.name + " ha muerto.");

        Destroy(gameObject, destroyDelay);
    }

    public bool IsDead() => isDead;
}
