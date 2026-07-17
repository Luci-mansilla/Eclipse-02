using UnityEngine;
using UnityEngine.UI;

// ================================================================
// EnemyHealthBar
//
// Lee la vida del componente EnemyHealth y actualiza una imagen
// utilizada como barra de vida.
//
// Este script NO modifica EnemyHealth.
// Solamente consulta currentHealth y maxHealth.
// ================================================================
public class EnemyHealthBar : MonoBehaviour
{
  [Header("=== REFERENCIAS ===")]

  [Tooltip("Componente que contiene la vida del enemigo")]
  public EnemyHealth enemyHealth;

  [Tooltip("Imagen roja que disminuye cuando el enemigo recibe daño")]
  public Image fillImage;

  [Header("=== OPCIONES ===")]

  [Tooltip("Oculta la barra cuando el enemigo tiene la vida completa")]
  public bool hideWhenFull = false;

  private Canvas canvas;

  void Start()
  {
    // Busca el Canvas que contiene esta barra.
    canvas = GetComponent<Canvas>();

    // Si no asignamos EnemyHealth manualmente,
    // lo busca en el objeto padre.
    if (enemyHealth == null)
    {
      enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    // Muestra un error si la barra no encuentra el sistema de vida.
    if (enemyHealth == null)
    {
      Debug.LogError(
          "EnemyHealthBar no encontró EnemyHealth en "
          + gameObject.name
      );
    }

    // Muestra un error si olvidamos asignar la imagen Fill.
    if (fillImage == null)
    {
      Debug.LogError(
          "Falta asignar Fill Image en "
          + gameObject.name
      );
    }

    UpdateHealthBar();
  }

  void Update()
  {
    // Actualiza la barra continuamente para mostrar
    // cualquier cambio de vida.
    UpdateHealthBar();
  }

  void UpdateHealthBar()
  {
    if (enemyHealth == null || fillImage == null)
      return;

    // Calcula el porcentaje de vida.
    // Ejemplo: 5 de 10 puntos = 0.5.
    float healthPercentage =
        enemyHealth.currentHealth / enemyHealth.maxHealth;

    // Impide que el valor sea menor que 0 o mayor que 1.
    healthPercentage = Mathf.Clamp01(healthPercentage);

    // Cambia cuánto se muestra de la imagen roja.
    fillImage.fillAmount = healthPercentage;

    // Esta opción permite ocultar la barra cuando está llena.
    if (hideWhenFull && canvas != null)
    {
      canvas.enabled = healthPercentage < 1f;
    }
  }
}
