using UnityEngine;

public class EnemySlowEffect : MonoBehaviour
{
    [Header("Velocidades con el anclaje activo")]

    [Tooltip("Velocidad de patrulla.")]
    public float velocidadPatrullaLenta = 0.08f;

    [Tooltip("Velocidad del lanzamiento físico.")]
    public float velocidadLungeLenta = 0.3f;

    [Tooltip("Velocidad cuando persigue bajo tierra.")]
    public float velocidadSubterraneaLenta = 0.15f;

    private EnemyPatrol enemyPatrol;
    private EnemyAttack enemyAttack;

    private float velocidadPatrullaOriginal;
    private float velocidadLungeOriginal;
    private float velocidadSubterraneaOriginal;

    private bool lentitudActiva = false;

    void Awake()
    {
        BuscarComponentes();
        GuardarVelocidadesOriginales();
    }

    void BuscarComponentes()
    {
        enemyPatrol = GetComponent<EnemyPatrol>();
        enemyAttack = GetComponent<EnemyAttack>();

        if (enemyPatrol == null)
            enemyPatrol = GetComponentInParent<EnemyPatrol>();

        if (enemyAttack == null)
            enemyAttack = GetComponentInParent<EnemyAttack>();

        if (enemyPatrol == null)
            enemyPatrol = GetComponentInChildren<EnemyPatrol>();

        if (enemyAttack == null)
            enemyAttack = GetComponentInChildren<EnemyAttack>();
    }

    void GuardarVelocidadesOriginales()
    {
        if (enemyPatrol != null)
        {
            velocidadPatrullaOriginal = enemyPatrol.speed;
        }
        else
        {
            Debug.LogError(
                gameObject.name + " no encontró EnemyPatrol."
            );
        }

        if (enemyAttack != null)
        {
            velocidadLungeOriginal = enemyAttack.lungeSpeed;
            velocidadSubterraneaOriginal = enemyAttack.undergroundSpeed;
        }
        else
        {
            Debug.LogWarning(
                gameObject.name + " no encontró EnemyAttack."
            );
        }
    }

    public void ActivarLentitud(float multiplicador)
    {
        if (lentitudActiva)
            return;

        if (enemyPatrol != null)
        {
            enemyPatrol.speed = velocidadPatrullaLenta;
        }

        if (enemyAttack != null)
        {
            enemyAttack.lungeSpeed = velocidadLungeLenta;
            enemyAttack.undergroundSpeed = velocidadSubterraneaLenta;
        }

        lentitudActiva = true;

        Debug.Log(
            gameObject.name +
            " ralentizado | Patrulla: " +
            (enemyPatrol != null ? enemyPatrol.speed.ToString() : "sin patrol") +
            " | Lunge: " +
            (enemyAttack != null ? enemyAttack.lungeSpeed.ToString() : "sin attack") +
            " | Subterráneo: " +
            (enemyAttack != null ? enemyAttack.undergroundSpeed.ToString() : "sin attack")
        );
    }

    public void DesactivarLentitud()
    {
        if (enemyPatrol != null)
        {
            enemyPatrol.speed = velocidadPatrullaOriginal;
        }

        if (enemyAttack != null)
        {
            enemyAttack.lungeSpeed = velocidadLungeOriginal;
            enemyAttack.undergroundSpeed = velocidadSubterraneaOriginal;
        }

        lentitudActiva = false;

        Debug.Log(gameObject.name + " recuperó sus velocidades.");
    }
}