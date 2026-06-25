using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public Transform attackpoint;
    public float weaponRange = 1;
    public LayerMask enemyLayer;
    public int damage = 10;
    public float coolDown = 1;
    private float timer;
    public Vector2 attackDirection = Vector2.right;

    private Vector2 lastDirection;

    public void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime; 
        }
    }

    public void Attack()
    {
        Debug.Log("Attack called");

        if (timer <= 0)
        {
            if (anim == null)
            {
                Debug.LogError("Animator is NULL!");
                return;
            }

            anim.SetBool("IsAttacking", true);

            attackpoint.localPosition = attackDirection;

            Collider2D[] Enemies = Physics2D.OverlapCircleAll(
                attackpoint.position,
                weaponRange,
                enemyLayer
            );

            if (Enemies.Length > 0)
            {
                // Se pasa transform.position para que EnemyHealth calcule
                // la direcci�n del knockback (retroceso) correctamente
                Enemies[0].GetComponent<EnemyHealth>().TakeDamage(damage, transform.position);
            }

            timer = coolDown;
        }
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
        anim.SetBool("IsAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackpoint.position, weaponRange);
    }
}
