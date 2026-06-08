using UnityEngine;
using UnityEngine.InputSystem;

public class ParallaxController : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;
        [Range(0f, 1f)]
        public float parallaxFactor;
        [HideInInspector]
        public Vector3 startPosition;
    }

    [Header("Capas Parallax (de fondo a primer plano)")]
    public ParallaxLayer[] layers = new ParallaxLayer[4];

    [Header("Configuración del Movimiento")]
    public float parallaxStrength = 0.5f;

    [Range(1f, 20f)]
    public float smoothSpeed = 5f;

    private Vector2 _inputDelta;

    void Start()
    {
        foreach (var layer in layers)
        {
            if (layer.layerTransform != null)
                layer.startPosition = layer.layerTransform.position;
        }
    }

    void Update()
    {
        if (Mouse.current == null) return; // ← protección clave

        Vector2 mousePos = Mouse.current.position.ReadValue();
        float x = (mousePos.x / Screen.width - 0.5f) * 2f;
        float y = (mousePos.y / Screen.height - 0.5f) * 2f;
        _inputDelta = new Vector2(x, y);

        ApplyParallax();
    }

    private void ApplyParallax()
    {
        foreach (var layer in layers)
        {
            if (layer.layerTransform == null) continue;

            Vector3 targetPos = layer.startPosition + new Vector3(
                _inputDelta.x * parallaxStrength * layer.parallaxFactor,
                _inputDelta.y * parallaxStrength * layer.parallaxFactor * 0.4f,
                0f
            );

            layer.layerTransform.position = Vector3.Lerp(
                layer.layerTransform.position,
                targetPos,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    [ContextMenu("Aplicar factores parallax predeterminados")]
    public void ApplyDefaultFactors()
    {
        if (layers.Length < 4) return;
        layers[0].parallaxFactor = 0.02f;
        layers[1].parallaxFactor = 0.06f;
        layers[2].parallaxFactor = 0.14f;
        layers[3].parallaxFactor = 0.25f;
        Debug.Log("[ParallaxController] Factores aplicados.");
    }
}