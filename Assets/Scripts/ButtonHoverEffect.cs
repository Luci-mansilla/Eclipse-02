using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IPointerClickHandler
{
    [Header("Escala")]
    public float scalaHover = 1.06f;
    public float escalaPresionado = 0.97f;
    public float velocidadAnimacion = 8f;

    [Header("Color del texto")]
    public Color colorNormal = new Color(0.84f, 0.84f, 0.84f, 1f);
    public Color colorHover = new Color(0.90f, 0.10f, 0.10f, 1f);
    public Color colorPresionado = new Color(1f, 0.95f, 0.85f, 1f);

    [Header("Desplazamiento")]
    public float desplazamientoY = 4f;
    public float desplazamientoX = 18f;

    private RectTransform _rect;
    private TextMeshProUGUI _texto;
    private Vector3 _escalaOriginal;
    private Vector3 _posicionOriginal;
    private Vector3 _escalaObjetivo;
    private Vector3 _posicionObjetivo;
    private Color _colorObjetivo;
    private bool _inicializado = false;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _texto = GetComponentInChildren<TextMeshProUGUI>();

        _escalaOriginal = _rect.localScale;
        _posicionOriginal = _rect.localPosition;
        _escalaObjetivo = _escalaOriginal;
        _posicionObjetivo = _posicionOriginal;
        _colorObjetivo = colorNormal;

        if (_texto != null)
            _texto.color = colorNormal;

        _inicializado = true;
    }

    void Update()
    {
        if (!_inicializado) return;

        _rect.localScale = Vector3.Lerp(_rect.localScale, _escalaObjetivo, Time.deltaTime * velocidadAnimacion);
        _rect.localPosition = Vector3.Lerp(_rect.localPosition, _posicionObjetivo, Time.deltaTime * velocidadAnimacion);

        if (_texto != null)
            _texto.color = Color.Lerp(_texto.color, _colorObjetivo, Time.deltaTime * velocidadAnimacion);
    }

    public void OnPointerEnter(PointerEventData _)
    {
        _escalaObjetivo = _escalaOriginal * scalaHover;
        _posicionObjetivo = _posicionOriginal + new Vector3(desplazamientoX, desplazamientoY, 0f);
        _colorObjetivo = colorHover;
    }

    public void OnPointerExit(PointerEventData _)
    {
        _escalaObjetivo = _escalaOriginal;
        _posicionObjetivo = _posicionOriginal;
        _colorObjetivo = colorNormal;
    }

    public void OnPointerDown(PointerEventData _)
    {
        _escalaObjetivo = _escalaOriginal * escalaPresionado;
        _posicionObjetivo = _posicionOriginal;
        _colorObjetivo = colorPresionado;
    }

    public void OnPointerUp(PointerEventData _)
    {
        _escalaObjetivo = _escalaOriginal * scalaHover;
        _posicionObjetivo = _posicionOriginal + new Vector3(desplazamientoX, desplazamientoY, 0f);
        _colorObjetivo = colorHover;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Necesario para que el Button reciba el click correctamente
    }

    public void ResetEstado()
    {
        _escalaObjetivo = _escalaOriginal;
        _posicionObjetivo = _posicionOriginal;
        _colorObjetivo = colorNormal;
    }
}
