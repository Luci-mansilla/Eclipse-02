using UnityEngine;
using UnityEngine.UI;

// Controla la energía de la habilidad especial.
// No controla el ataque ni la vida del jugador.
public class PlayerAbilityEnergy : MonoBehaviour
{
  [Header("=== ENERGÍA ===")]
  public float maxEnergy = 100f;

  [SerializeField]
  private float currentEnergy = 0f;

  [Header("=== BARRA ===")]
  [Tooltip("Slider visual de la habilidad")]
  public Slider abilitySlider;

  [Header("=== ICONO ===")]
  [Tooltip("Imagen UI donde se mostrará el icono")]
  public Image abilityIcon;

  [Tooltip("Sprite gris mientras la habilidad no está lista")]
  public Sprite disabledIcon;

  [Tooltip("Sprite activo cuando la habilidad llega al máximo")]
  public Sprite readyIcon;

  [Header("=== EFECTO DEL PERSONAJE ===")]
  [Tooltip("Objeto visual que se activa sobre el jugador cuando la habilidad está lista")]
  public GameObject readyPlayerEffect;

  public bool IsReady => currentEnergy >= maxEnergy;

  private void Start()
  {
    currentEnergy = 0f;

    if (abilitySlider != null)
    {
      abilitySlider.minValue = 0f;
      abilitySlider.maxValue = maxEnergy;
      abilitySlider.value = currentEnergy;
    }

    UpdateVisuals();
  }

  // Se llama por cada enemigo golpeado.
  public void AddEnergy(float amount)
  {
    if (amount <= 0f)
      return;

    if (IsReady)
      return;

    currentEnergy += amount;
    currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

    UpdateVisuals();

    Debug.Log(
        "Energía de habilidad: "
        + currentEnergy
        + " / "
        + maxEnergy
    );
  }

  // Se usará después cuando implementemos la habilidad.
  public bool TrySpendEnergy()
  {
    if (!IsReady)
      return false;

    currentEnergy = 0f;
    UpdateVisuals();

    return true;
  }

  public void ResetEnergy()
  {
    currentEnergy = 0f;
    UpdateVisuals();
  }

  private void UpdateVisuals()
  {
    if (abilitySlider != null)
    {
      abilitySlider.maxValue = maxEnergy;
      abilitySlider.value = currentEnergy;
    }

    if (abilityIcon != null)
    {
      abilityIcon.sprite = IsReady
          ? readyIcon
          : disabledIcon;
    }

    if (readyPlayerEffect != null)
    {
      readyPlayerEffect.SetActive(IsReady);
    }
  }

}