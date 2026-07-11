using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public Transform attackpoint;

    [Header("Ataque")]
    public float weaponRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 10;
    public float coolDown = 1f;

    [Header("Precisión")]
    [Tooltip("Multiplicador del retroceso aplicado a los enemigos")]
    public float knockbackMultiplier = 1f;

    private float timer;

    public Vector2 attackDirection = Vector2.right;

    [Header("Sonido de ataque")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        Debug.Log("Attack called");

        if (timer > 0)
            return;

        if (anim == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }

        if (attackpoint == null)
        {
            Debug.LogError("AttackPoint no está asignado.");
            return;
        }

        anim.SetBool("IsAttacking", true);

        // Sonido de ataque
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        attackpoint.localPosition = attackDirection;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackpoint.position,
            weaponRange,
            enemyLayer
        );

        foreach (Collider2D enemyCollider in enemies)
        {
            EnemyHealth enemyHealth =
                enemyCollider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(
                    damage,
                    transform.position,
                    knockbackMultiplier
                );
            }
        }

        timer = coolDown;
    }

    public void SetAttackDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            attackDirection = direction;
        }
    }

    public void FinishedAttacking()
    {
        if (anim != null)
        {
            anim.SetBool("IsAttacking", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackpoint != null)
        {
            Gizmos.DrawWireSphere(
                attackpoint.position,
                weaponRange
            );
        }
    }
}