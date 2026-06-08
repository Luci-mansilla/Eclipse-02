# Menú Principal — Guía de Setup en Unity

## Archivos incluidos

| Archivo | Descripción |
|---|---|
| `ParallaxController.cs` | Efecto parallax de 4 capas reaccionando al mouse |
| `MainMenuController.cs` | Lógica de botones, paneles, redes sociales y fade |
| `ButtonHoverEffect.cs` | Animación de escala y color en hover/click |

---

## 1. Preparar la escena

1. Crear una nueva escena llamada `MainMenu`.
2. Agregar la escena a **File → Build Settings**.
3. La cámara debe ser **Orthographic** (Camera → Projection: Orthographic).

---

## 2. Configurar el Parallax (fondo)

### Estructura de GameObjects

```
Scene
├── ParallaxController   ← Vacío, con ParallaxController.cs
├── Layers
│   ├── Layer_Cielo      ← SpriteRenderer con imagen del cielo
│   ├── Layer_Luna       ← SpriteRenderer con la luna
│   ├── Layer_Edificios  ← SpriteRenderer con edificios
│   └── Layer_Protagonista ← SpriteRenderer con el protagonista
```

### Pasos

1. Crear un GameObject vacío → renombrarlo `ParallaxController`.
2. Arrastrar `ParallaxController.cs` a ese GameObject.
3. En el Inspector, asignar las 4 capas en los campos `Layer 0` a `Layer 3`.
4. Hacer clic derecho en el componente → **"Aplicar factores parallax predeterminados"**
   para asignar automáticamente los valores recomendados.
5. Ajustar `Parallax Strength` (recomendado: 0.3 – 0.6).

### Factores sugeridos

| Capa | Factor | Sensación |
|---|---|---|
| Cielo | 0.02 | Casi estático |
| Luna | 0.06 | Muy lento |
| Edificios | 0.14 | Medio |
| Protagonista | 0.25 | Más cercano |

---

## 3. Configurar el Canvas y los botones

### Estructura del Canvas

```
Canvas (Screen Space - Overlay)
└── MenuController      ← Vacío, con MainMenuController.cs
    ├── BotonesMenu
    │   ├── BtnNuevaPartida
    │   ├── BtnOpciones
    │   ├── BtnCreditos
    │   └── BtnSalir
    ├── BotonSocial
    │   ├── BtnDiscord
    │   ├── BtnX
    │   ├── BtnYouTube
    │   └── BtnSteam
    ├── PanelOpciones   ← Desactivado al inicio
    ├── PanelCreditos   ← Desactivado al inicio
    └── PanelFade       ← CanvasGroup negro, para fade in/out
```

### Pasos

1. Crear un **Canvas** (GameObject → UI → Canvas).
   - Canvas Scaler → Scale Mode: **Scale With Screen Size**
   - Reference Resolution: **1920 x 1080**
2. Dentro del Canvas, crear un GameObject vacío `MenuController`.
3. Arrastrar `MainMenuController.cs` a `MenuController`.
4. En el Inspector, asignar cada botón a su campo correspondiente.
5. Ingresar las URLs de tus redes en los campos de texto del Inspector.
6. Asignar el nombre de escena del juego en `Nombre Escena Juego`.

### PanelFade

1. Crear un Panel (UI → Panel) → renombrar `PanelFade`.
2. Color: **negro puro** (#000000).
3. Agregar componente **CanvasGroup**.
4. Asignarlo al campo `Panel Fade` en MainMenuController.

---

## 4. Agregar efectos a los botones

Para cada botón (principales y de redes):

1. Seleccionar el botón en la jerarquía.
2. Agregar componente → buscar `ButtonHoverEffect`.
3. Ajustar los colores `Color Normal`, `Color Hover` y `Color Presionado`
   para que coincidan con el estilo visual del juego.

---

## 5. Iconos de redes sociales

Usar imágenes PNG transparentes como sprites en cada botón:
- `BtnDiscord` → ícono blanco de Discord
- `BtnX` → ícono blanco de X/Twitter
- `BtnYouTube` → ícono blanco de YouTube
- `BtnSteam` → ícono blanco de Steam

Podés encontrar packs de íconos gratuitos en:
- https://simpleicons.org (SVG exportable a PNG)
- https://www.flaticon.com (buscar "discord white icon")

---

## 6. Checklist final

- [ ] Las 4 capas de parallax asignadas en ParallaxController
- [ ] Los 4 botones principales asignados en MainMenuController
- [ ] Los 4 botones de redes con sus URLs correctas
- [ ] El Panel Fade tiene CanvasGroup y está asignado
- [ ] El nombre de la escena del juego está en Build Settings
- [ ] ButtonHoverEffect en cada botón
- [ ] Canvas Scaler configurado en Scale With Screen Size (1920×1080)
