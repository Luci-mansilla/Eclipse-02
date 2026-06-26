using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnVolverMenu : MonoBehaviour
{
    // Arrastrá este script al objeto Button de Escena-Demo
    // Luego en Button -> OnClick() seleccioná: BtnVolverMenu -> IrAMenu

    public void IrAMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Escena-menu");
    }
}
