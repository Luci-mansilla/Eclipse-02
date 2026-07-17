using UnityEngine;

// ================================================================
// MichelangeloDivision
//
// Divide a Michelangelo en dos copias pequeñas cuando su vida
// alcanza el porcentaje configurado.
//
// Este script funciona por separado:
// NO modifica EnemyHealth, EnemyPatrol ni EnemyAttackMiguel.
// ================================================================
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyPatrol))]
public class MichelangeloDivision : MonoBehaviour
{
  [Header("=== ACTIVACIÓN ===")]

  [Tooltip("Porcentaje de vida en el que se divide. 0.5 equivale al 50 %.")]
  [Range(0.1f, 0.9f)]
  public float divideAtHealthPercent = 0.5f;

  [Tooltip("Indica si este Michelangelo puede dividirse.")]
  public bool canDivide = true;


  [Header("=== PREFAB ===")]

  [Tooltip("Prefab original de Michelangelo, arrastrado desde Project.")]
  public GameObject michelangeloPrefab;


  [Header("=== COPIAS ===")]

  [Tooltip("Tamaño de las copias. 0.7 equivale al 70 %.")]
  [Range(0.2f, 1f)]
  public float copyScale = 0.7f;

  [Tooltip("Vida de cada copia respecto a la vida máxima original.")]
  [Range(0.1f, 1f)]
  public float copyHealthPercent = 0.4f;

  [Tooltip("Distancia entre las dos copias.")]
  public float separationDistance = 0.8f;


  [Header("=== BARRA DE VIDA ===")]

  [Tooltip("Objeto principal de la barra de vida del enemigo.")]
  public Transform healthBarTransform;


  [Header("=== EFECTO VISUAL OPCIONAL ===")]

  [Tooltip("Partícula o efecto que aparece durante la división.")]
  public GameObject divisionEffectPrefab;


  // Componentes del enemigo.
  private EnemyHealth health;
  private EnemyPatrol patrol;
  private EnemyAttackMiguel attack;
  private Rigidbody2D rb;

  // Componentes visuales.
  private SpriteRenderer spriteRenderer;
  private Animator animator;

  // Guarda el color y tamaño originales.
  private Color normalColor = Color.white;
  private Vector3 originalScale;

  // Impide que la división suceda varias veces.
  private bool hasDivided = false;


  // =============================================================
  // Awake se ejecuta cuando el objeto aparece.
  // Guarda todos los componentes necesarios.
  // =============================================================
  void Awake()
  {
    health = GetComponent<EnemyHealth>();
    patrol = GetComponent<EnemyPatrol>();
    attack = GetComponent<EnemyAttackMiguel>();
    rb = GetComponent<Rigidbody2D>();

    spriteRenderer = GetComponent<SpriteRenderer>();
    animator = GetComponent<Animator>();

    originalScale = transform.localScale;

    // Guarda el color normal del personaje.
    if (spriteRenderer != null)
    {
      normalColor = spriteRenderer.color;
    }
  }


  // =============================================================
  // Comprueba constantemente si debe dividirse.
  // =============================================================
  void Update()
  {
    // No continúa si ya se dividió o no tiene permiso.
    if (!canDivide || hasDivided)
      return;

    // No continúa si no encuentra la vida o está muerto.
    if (health == null || health.IsDead())
      return;

    // Espera a que termine el retroceso del golpe.
    if (health.IsInKnockback())
      return;

    // Espera a que termine un ataque.
    if (patrol != null && patrol.attackingOverride)
      return;

    // Calcula cuánta vida debe tener para dividirse.
    float divisionHealth =
        health.maxHealth * divideAtHealthPercent;

    // Se divide cuando alcanza el porcentaje indicado.
    if (health.currentHealth <= divisionHealth &&
        health.currentHealth > 0f)
    {
      Divide();
    }
  }


  // =============================================================
  // Realiza la división.
  //
  // El original se convierte en una copia pequeña y se crea
  // una segunda copia usando el prefab.
  // =============================================================
  void Divide()
  {
    hasDivided = true;
    canDivide = false;

    if (michelangeloPrefab == null)
    {
      Debug.LogError(
          "Falta asignar el prefab de Michelangelo en "
          + gameObject.name
      );

      return;
    }

    // Detiene el movimiento antes de dividirse.
    if (rb != null)
    {
      rb.linearVelocity = Vector2.zero;
    }

    // Guarda al jugador antes de crear la copia.
    Transform playerTarget = null;

    if (attack != null)
    {
      playerTarget = attack.player;
    }

    // Posiciones de las dos copias.
    Vector3 leftPosition =
        transform.position + Vector3.left * separationDistance;

    Vector3 rightPosition =
        transform.position + Vector3.right * separationDistance;

    // Calcula la vida máxima de cada copia.
    float copyMaxHealth = Mathf.Max(
        1f,
        health.maxHealth * copyHealthPercent
    );

    // Crea el efecto visual, si fue asignado.
    if (divisionEffectPrefab != null)
    {
      Instantiate(
          divisionEffectPrefab,
          transform.position,
          Quaternion.identity
      );
    }

    // ---------------------------------------------------------
    // PRIMERA COPIA
    // El Michelangelo original se convierte en una copia pequeña.
    // ---------------------------------------------------------
    transform.position = leftPosition;

    ConfigureCopy(
        gameObject,
        copyMaxHealth,
        playerTarget
    );

    // ---------------------------------------------------------
    // SEGUNDA COPIA
    // Se crea otro Michelangelo desde el prefab.
    // ---------------------------------------------------------
    GameObject secondCopy = Instantiate(
        michelangeloPrefab,
        rightPosition,
        transform.rotation
    );

    ConfigureCopy(
        secondCopy,
        copyMaxHealth,
        playerTarget
    );

    Debug.Log(
        gameObject.name + " se dividió en dos copias."
    );
  }


  // =============================================================
  // Configura una copia:
  // • Reduce su tamaño.
  // • Cambia su vida.
  // • Le asigna el jugador.
  // • Evita que vuelva a dividirse.
  // • Restaura su visibilidad.
  // =============================================================
  void ConfigureCopy(
      GameObject copy,
      float newMaxHealth,
      Transform playerTarget
  )
  {
    if (copy == null)
      return;

    // Reduce el tamaño.
    copy.transform.localScale =
        originalScale * copyScale;

    // Configura la vida de la copia.
    EnemyHealth copyHealth =
        copy.GetComponent<EnemyHealth>();

    if (copyHealth != null)
    {
      copyHealth.maxHealth = newMaxHealth;
      copyHealth.currentHealth = newMaxHealth;
    }

    // Le asigna el mismo jugador.
    EnemyAttackMiguel copyAttack =
        copy.GetComponent<EnemyAttackMiguel>();

    if (copyAttack != null)
    {
      copyAttack.player = playerTarget;
      copyAttack.enabled = true;
    }

    // Reinicia el control de movimiento.
    EnemyPatrol copyPatrol =
        copy.GetComponent<EnemyPatrol>();

    if (copyPatrol != null)
    {
      copyPatrol.attackingOverride = false;
      copyPatrol.enabled = true;
    }

    // Evita que vuelva a dividirse.
    MichelangeloDivision copyDivision =
        copy.GetComponent<MichelangeloDivision>();

    if (copyDivision != null)
    {
      copyDivision.canDivide = false;
      copyDivision.hasDivided = true;

      // Corrige el problema del enemigo invisible.
      copyDivision.ResetVisualState();

      // Mantiene la barra con un tamaño legible.
      copyDivision.KeepHealthBarReadable();
    }

    // Detiene cualquier movimiento anterior.
    Rigidbody2D copyRb =
        copy.GetComponent<Rigidbody2D>();

    if (copyRb != null)
    {
      copyRb.linearVelocity = Vector2.zero;
    }

    // Asegura que la colisión esté activa.
    Collider2D copyCollider =
        copy.GetComponent<Collider2D>();

    if (copyCollider != null)
    {
      copyCollider.enabled = true;
    }
  }


  // =============================================================
  // Corrige el problema del enemigo invisible.
  //
  // Al recibir daño, EnemyHealth ejecuta un parpadeo. Si la
  // división sucede mientras el sprite está apagado, el original
  // puede quedar invisible. Este método detiene el parpadeo y
  // vuelve a encender el sprite.
  // =============================================================
  void ResetVisualState()
  {
    EnemyHealth enemyHealth =
        GetComponent<EnemyHealth>();

    // Detiene Flicker, Hurt y cambios de color temporales.
    if (enemyHealth != null)
    {
      enemyHealth.StopAllCoroutines();
    }

    // Vuelve a buscar los componentes por seguridad.
    if (spriteRenderer == null)
    {
      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    if (animator == null)
    {
      animator = GetComponent<Animator>();
    }

    // Fuerza al personaje a quedar visible.
    if (spriteRenderer != null)
    {
      spriteRenderer.enabled = true;
      spriteRenderer.color = normalColor;
    }

    // Quita los estados Hurt y Death del Animator.
    if (animator != null)
    {
      string hurtParameter =
          enemyHealth != null
          ? enemyHealth.paramIsHurt
          : "IsHurt";

      string deathParameter =
          enemyHealth != null
          ? enemyHealth.paramIsDeath
          : "IsDeath";

      SetAnimatorBoolIfExists(
          animator,
          hurtParameter,
          false
      );

      SetAnimatorBoolIfExists(
          animator,
          deathParameter,
          false
      );
    }
  }


  // =============================================================
  // Mantiene la barra del mismo tamaño visual aunque el enemigo
  // sea más pequeño.
  // =============================================================
  void KeepHealthBarReadable()
  {
    if (healthBarTransform == null)
      return;

    healthBarTransform.localScale =
        healthBarTransform.localScale / copyScale;

    healthBarTransform.localPosition =
        healthBarTransform.localPosition / copyScale;
  }


  // =============================================================
  // Cambia un Bool del Animator solamente si el parámetro existe.
  // Esto evita errores si el nombre está mal escrito.
  // =============================================================
  void SetAnimatorBoolIfExists(
      Animator targetAnimator,
      string parameterName,
      bool value
  )
  {
    if (targetAnimator == null ||
        string.IsNullOrEmpty(parameterName))
    {
      return;
    }

    foreach (
        AnimatorControllerParameter parameter
        in targetAnimator.parameters
    )
    {
      if (parameter.name == parameterName &&
          parameter.type ==
          AnimatorControllerParameterType.Bool)
      {
        targetAnimator.SetBool(
            parameterName,
            value
        );

        return;
      }
    }
  }
}