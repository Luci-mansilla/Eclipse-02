using UnityEngine;
using UnityEngine.UI;

// Arrastra este script a cualquier boton en el Inspector.
// Le da fondo negro rojizo + borde rojo gotico automaticamente.

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class UIBorderButton : MonoBehaviour
{
    [Header("Colores")]
    public Color colorFondo = new Color(0.06f, 0.01f, 0.01f, 0.92f); // negro rojizo
    public Color colorBorde = new Color(0.75f, 0.22f, 0.17f, 1.00f); // rojo gotico
    public Color colorHover = new Color(0.25f, 0.05f, 0.05f, 1.00f); // rojo oscuro hover
    public Color colorPressed = new Color(0.55f, 0.12f, 0.10f, 1.00f); // rojo medio click

    [Header("Grosor del borde (pixeles)")]
    public float grosor = 2f;

    void Awake()
    {
        AplicarEstilo();
    }

    public void AplicarEstilo()
    {
        // ── 1. Fondo del boton ──────────────────────────────────────────────
        Image imgFondo = GetComponent<Image>();
        imgFondo.color = colorFondo;

        // ── 2. Colores de interaccion ───────────────────────────────────────
        Button btn = GetComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white; // multiplicado por imgFondo.color
        cb.highlightedColor = new Color(
            colorHover.r / colorFondo.r,
            colorHover.g / colorFondo.g,
            colorHover.b / colorFondo.b, 1f);
        cb.pressedColor = new Color(
            colorPressed.r / colorFondo.r,
            colorPressed.g / colorFondo.g,
            colorPressed.b / colorFondo.b, 1f);
        cb.selectedColor = cb.highlightedColor;
        cb.colorMultiplier = 1f;
        btn.colors = cb;

        // ── 3. Borde como outline usando Outline component ──────────────────
        // Borramos outline viejo si existe
        Outline outlineViejo = GetComponent<Outline>();
        if (outlineViejo != null)
            DestroyImmediate(outlineViejo);

        // Borramos el hijo de borde viejo si existe
        Transform bordeViejo = transform.Find("Borde_Gotico");
        if (bordeViejo != null)
            DestroyImmediate(bordeViejo.gameObject);

        // Creamos el borde como objeto hijo detras del texto
        GameObject bordeObj = new GameObject("Borde_Gotico");
        bordeObj.transform.SetParent(transform, false);
        bordeObj.transform.SetAsFirstSibling(); // va DETRAS del texto/icono

        Image imgBorde = bordeObj.AddComponent<Image>();
        imgBorde.color = colorBorde;
        imgBorde.raycastTarget = false; // no bloquea los clicks

        RectTransform rt = bordeObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(-grosor, -grosor);
        rt.offsetMax = new Vector2(grosor, grosor);
    }
}
