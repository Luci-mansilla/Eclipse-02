using UnityEngine;

public static class CheckpointManager
{
    public static bool checkpointActivo = false;
    public static string escenaCheckpoint;
    public static Vector3 posicionCheckpoint;

    public static void GuardarCheckpoint(string escena, Vector3 posicion)
    {
        checkpointActivo = true;
        escenaCheckpoint = escena;
        posicionCheckpoint = posicion;

        Debug.Log("Checkpoint guardado en escena: " + escenaCheckpoint);
        Debug.Log("Posición checkpoint: " + posicionCheckpoint);
    }
}
