using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnSiguienteDerrota : MonoBehaviour
{
    // Arrastrá este script al objeto Button de Escena-derrota
    // Luego en Button -> OnClick() seleccioná: BtnSiguienteDerrota -> IrADemo

    public void IrADemo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Escena-Demo");
    }
}
