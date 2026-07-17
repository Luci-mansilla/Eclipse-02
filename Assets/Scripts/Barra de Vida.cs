using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image fillImage;

    public float smoothSpeed = 2f;

    void Update()
    {
        if (playerHealth == null)
        {
            Debug.LogError("ERROR: PlayerHealth no está asignado.");
            return;
        }

        if (fillImage == null)
        {
            Debug.LogError("ERROR: FillImage no está asignado.");
            return;
        }

        if (playerHealth.maxHealth <= 0)
        {
            Debug.LogError("ERROR: maxHealth es 0.");
            return;
        }

        float targetFill = playerHealth.currentHealth / playerHealth.maxHealth;

        fillImage.fillAmount = Mathf.Lerp(
            fillImage.fillAmount,
            targetFill,
            smoothSpeed * Time.deltaTime
        );
    }
}
