using UnityEngine;

public class ShieldPulse : MonoBehaviour
{
    [Header("Pulso visual")]
    public float speed = 5f;
    public float minAlpha = 0.25f;
    public float maxAlpha = 1f;

    [Header("Escala")]
    public float scaleAmount = 0.08f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalScale;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        originalScale = transform.localScale;
    }


    void Update()
    {
        if (spriteRenderer == null)
            return;


        float pulse = (Mathf.Sin(Time.time * speed) + 1f) / 2f;


        float alpha = Mathf.Lerp(
            minAlpha,
            maxAlpha,
            pulse
        );


        Color newColor = originalColor;
        newColor.a = alpha;

        spriteRenderer.color = newColor;


        float scale = 1f + (pulse * scaleAmount);

        transform.localScale = originalScale * scale;
    }
}