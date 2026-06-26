using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnSiguienteVictoria : MonoBehaviour
{
    // Arrastrá este script al objeto Button de Escena-Victoria
    // Luego en Button -> OnClick() seleccioná: BtnSiguienteVictoria -> IrADemo

    public void IrADemo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Escena-Demo");
    }
}
