using UnityEngine;

[RequireComponent(typeof(EnemyPatrol))]
[RequireComponent(typeof(EnemyAttackTroyanogame))]
public class EnemyChestTransform : MonoBehaviour
{
    [Header("=== REFERENCIAS ===")]
    public Transform player;

    [Header("=== DETECCIÓN ===")]
    public float interactRange = 1.5f;
    public KeyCode interactKey = KeyCode.F;

    [Header("=== ANIMATOR ===")]
    public string paramOpenTrigger = "Reveal";   // Trigger que ya tenés creado — dispara el clip ChestOpen
    public string paramIsChest = "IsHidden";     // Bool que bloquea las transiciones de Any State mientras es cofre
    public float openToActiveDelay = 0.6f;       // Duración del clip ChestOpen (ajustá según tu clip real)

    [Header("=== SONIDO (opcional) ===")]
    public AudioSource audioSource;
    public AudioClip openSound;

    [Header("=== UI (opcional) ===")]
    public GameObject interactPrompt;   // Ej: ícono "Presioná F" que aparece al acercarte

    private Animator anim;
    private EnemyPatrol patrol;
    private EnemyAttackTroyanogame attack;

    private enum State { ChestIdle, ChestOpen, Transformed }
    private State current = State.ChestIdle;

    private float openTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
        patrol = GetComponent<EnemyPatrol>();
        attack = GetComponent<EnemyAttackTroyanogame>();

        // Mientras es cofre, el enemigo NO debe moverse ni atacar.
        // Esto evita que EnemyPatrol/EnemyAttackTroyanogame manden
        // Speed/IsAttacking al Animator y disparen Any State de una.
        patrol.enabled = false;
        attack.enabled = false;

        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        SetBool(paramIsChest, true);
    }

    void Update()
    {
        switch (current)
        {
            case State.ChestIdle: ChestIdleState(); break;
            case State.ChestOpen: ChestOpenState(); break;
            case State.Transformed: break; // De acá en más EnemyPatrol/EnemyAttackTroyanogame manejan todo
        }
    }

    // ── COFRE QUIETO ─────────────────────────────────────────────
    void ChestIdleState()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool inRange = dist <= interactRange;

        if (interactPrompt != null)
            interactPrompt.SetActive(inRange);

        if (inRange && Input.GetKeyDown(interactKey))
            OpenChest();
    }

    void OpenChest()
    {
        current = State.ChestOpen;
        openTimer = openToActiveDelay;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (anim != null)
            anim.SetTrigger(paramOpenTrigger); // Reveal

        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);
    }

    // ── COFRE ABRIÉNDOSE ─────────────────────────────────────────
    void ChestOpenState()
    {
        openTimer -= Time.deltaTime;
        if (openTimer <= 0f)
            ActivateEnemy();
    }

    // ── TRANSFORMACIÓN FINAL ─────────────────────────────────────
    void ActivateEnemy()
    {
        current = State.Transformed;

        SetBool(paramIsChest, false);  // Recién ahora Any State puede llevarlo a Movimientos/attack

        patrol.enabled = true;
        attack.enabled = true;

        this.enabled = false; // este script ya cumplió su función
    }

    void SetBool(string paramName, bool value)
    {
        if (anim != null) anim.SetBool(paramName, value);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
