using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image fillImage;

    public float smoothSpeed = 2f;

    void Update()
    {
        float targetFill = playerHealth.currentHealth / playerHealth.maxHealth;

        fillImage.fillAmount = Mathf.Lerp(
            fillImage.fillAmount,
            targetFill,
            smoothSpeed * Time.deltaTime
        );
    }
}
