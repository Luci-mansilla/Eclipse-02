using System.Collections.Generic;
using UnityEngine;

// ================================================================
// PlayerSpecialAbility
//
// Activa la habilidad especial cuando la barra está completa.
// La animación se activa con Q.
// El daño real se ejecuta mediante un Animation Event.
// ================================================================
public class PlayerSpecialAbility : MonoBehaviour
{
  [Header("=== REFERENCIAS ===")]
  public Animator animator;
  public PlayerAbilityEnergy abilityEnergy;

  [Header("=== ÁREA DE LA HABILIDAD ===")]
  [Tooltip("Centro desde donde se calcula el ataque. Puede ser el propio Player.")]
  public Transform abilityCenter;

  [Tooltip("Radio del ataque circular")]
  public float abilityRadius = 3f;

  [Tooltip("Capa donde están los enemigos")]
  public LayerMask enemyLayer;

  [Header("=== DAÑO Y EMPUJE ===")]
  [Tooltip("Daño aplicado a cada enemigo dentro del área")]
  public float abilityDamage = 15f;

  [Tooltip("Multiplicador del knockback. Debe ser mayor que el ataque normal.")]
  public float knockbackMultiplier = 2.5f;

  [Header("=== CONTROL ===")]
  [Tooltip("Evita activar otra habilidad mientras se reproduce")]
  public bool isUsingAbility = false;

  private void Awake()
  {
    if (animator == null)
      animator = GetComponent<Animator>();

    if (abilityEnergy == null)
      abilityEnergy = GetComponent<PlayerAbilityEnergy>();

    if (abilityCenter == null)
      abilityCenter = transform;
  }

  private void Update()
  {
    // La habilidad se activa con Q.
    if (Input.GetKeyDown(KeyCode.Q))
    {
      TryActivateAbility();
    }
  }

  // Comprueba si la barra está llena y comienza la animación.
  private void TryActivateAbility()
  {
    if (isUsingAbility)
      return;

    if (abilityEnergy == null || !abilityEnergy.IsReady)
    {
      Debug.Log("La habilidad especial todavía no está lista.");
      return;
    }

    isUsingAbility = true;

    // Consume la barra al comenzar la habilidad.
    if (!abilityEnergy.TrySpendEnergy())
    {
      isUsingAbility = false;
      return;
    }

    if (animator != null)
    {
      animator.SetTrigger("SpecialAbility");
    }
  }

  // Este método será llamado por un Animation Event
  // en el frame donde la onda alcanza su mayor tamaño.
  public void ApplySpecialAbilityHit()
  {
    Vector2 center = abilityCenter != null
        ? abilityCenter.position
        : transform.position;

    Collider2D[] colliders = Physics2D.OverlapCircleAll(
        center,
        abilityRadius,
        enemyLayer
    );

    // Evita dañar dos veces al mismo enemigo
    // si posee más de un Collider2D.
    HashSet<EnemyHealth> enemiesHit = new HashSet<EnemyHealth>();

    foreach (Collider2D enemyCollider in colliders)
    {
      EnemyHealth enemyHealth =
          enemyCollider.GetComponentInParent<EnemyHealth>();

      if (enemyHealth == null)
        continue;

      if (enemyHealth.IsDead())
        continue;

      if (!enemiesHit.Add(enemyHealth))
        continue;

      // EnemyHealth ya se encarga del daño,
      // el knockback, Hurt y la detención temporal del enemigo.
      enemyHealth.TakeDamage(
          abilityDamage,
          transform.position,
          knockbackMultiplier
      );
    }

    Debug.Log(
        "Habilidad especial impactó a "
        + enemiesHit.Count
        + " enemigos."
    );
  }

  // Animation Event colocado en el último frame.
  public void FinishSpecialAbility()
  {
    isUsingAbility = false;
  }

  private void OnDrawGizmosSelected()
  {
    Transform center = abilityCenter != null
        ? abilityCenter
        : transform;

    Gizmos.DrawWireSphere(
        center.position,
        abilityRadius
    );
  }
}