using UnityEngine;

public class AbilityReadyPulse : MonoBehaviour
{
  [Header("Escala")]
  public float pulseSpeed = 3f;
  public float pulseAmount = 0.08f;

  private Vector3 originalScale;

  private void Awake()
  {
    originalScale = transform.localScale;
  }

  private void OnEnable()
  {
    transform.localScale = originalScale;
  }

  private void Update()
  {
    float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

    transform.localScale = new Vector3(
        originalScale.x * pulse,
        originalScale.y * pulse,
        originalScale.z
    );
  }
}
