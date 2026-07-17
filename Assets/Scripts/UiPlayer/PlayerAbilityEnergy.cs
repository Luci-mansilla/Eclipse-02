using UnityEngine;
using UnityEngine.UI;

// ================================================================
// PlayerAbilityEnergy
//
// Controla la energía de la habilidad especial del jugador.
//
// FUNCIONES:
// • Recibe energía cuando el jugador golpea enemigos.
// • Actualiza la barra visual.
// • Cambia el color del icono cuando está lista.
// • Activa un efecto visual sobre el jugador.
// • Permite gastar toda la energía al usar la habilidad.
// ================================================================
public class PlayerAbilityEnergy : MonoBehaviour
{
  [Header("=== ENERGÍA ===")]
  [Tooltip("Cantidad máxima necesaria para activar la habilidad")]
  public float maxEnergy = 100f;

  [Tooltip("Energía actual del jugador")]
  public float currentEnergy = 0f;

  [Header("=== INTERFAZ ===")]
  [Tooltip("Slider que representa la energía de la habilidad")]
  public Slider abilityBar;

  [Tooltip("Icono de la habilidad especial")]
  public Image abilityIcon;

  [Header("=== COLORES DEL ICONO ===")]
  [Tooltip("Color mientras la habilidad todavía se está cargando")]
  public Color chargingIconColor = new Color(0.35f, 0.35f, 0.35f, 1f);

  [Tooltip("Color cuando la habilidad está lista")]
  public Color readyIconColor = Color.white;

  [Header("=== EFECTO SOBRE EL JUGADOR ===")]
  [Tooltip("Objeto visual que aparece cuando la habilidad está lista")]
  public GameObject playerReadyEffect;

  // Permite consultar desde otros scripts si la habilidad está lista.
  public bool IsReady
  {
    get
    {
      return currentEnergy >= maxEnergy;
    }
  }

  private void Start()
  {
    // La energía comienza vacía.
    currentEnergy = 0f;

    // Configura la barra.
    if (abilityBar != null)
    {
      abilityBar.minValue = 0f;
      abilityBar.maxValue = maxEnergy;
    }

    UpdateVisuals();
  }

  // ------------------------------------------------------------
  // Suma energía cuando el jugador golpea un enemigo.
  // ------------------------------------------------------------
  public void AddEnergy(float amount)
  {
    // No permite sumar cantidades negativas.
    if (amount <= 0f)
      return;

    // Si ya está llena, no sigue acumulando.
    if (IsReady)
      return;

    currentEnergy += amount;

    // Evita superar el máximo.
    currentEnergy = Mathf.Clamp(
        currentEnergy,
        0f,
        maxEnergy
    );

    UpdateVisuals();

    Debug.Log(
        "Energía de habilidad: "
        + currentEnergy
        + " / "
        + maxEnergy
    );
  }

  // ------------------------------------------------------------
  // Intenta gastar la energía.
  //
  // Devuelve true si estaba completa y pudo gastarse.
  // Devuelve false si todavía no estaba lista.
  // ------------------------------------------------------------
  public bool TrySpendEnergy()
  {
    if (!IsReady)
      return false;

    currentEnergy = 0f;
    UpdateVisuals();

    return true;
  }

  // ------------------------------------------------------------
  // Vacía manualmente la barra.
  // Puede servir al morir o cambiar de nivel.
  // ------------------------------------------------------------
  public void ResetEnergy()
  {
    currentEnergy = 0f;
    UpdateVisuals();
  }

  // ------------------------------------------------------------
  // Actualiza la barra, el icono y el efecto del jugador.
  // ------------------------------------------------------------
  private void UpdateVisuals()
  {
    if (abilityBar != null)
    {
      abilityBar.maxValue = maxEnergy;
      abilityBar.value = currentEnergy;
    }

    if (abilityIcon != null)
    {
      abilityIcon.color = IsReady
          ? readyIconColor
          : chargingIconColor;
    }

    if (playerReadyEffect != null)
    {
      playerReadyEffect.SetActive(IsReady);
    }
  }
}
