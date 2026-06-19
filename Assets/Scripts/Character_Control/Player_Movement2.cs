using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 30f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    public Player_Combat player_combat;

    void Start()
    {
        Debug.Log("START CALLED ON: " + gameObject.name);
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player_combat = GetComponent<Player_Combat>();

        Debug.Log(GetComponent<Player_Combat>());

        Debug.Log("RB: " + rb);
        Debug.Log("Animator: " + animator);
        Debug.Log("Combat: " + player_combat);
        Debug.Log("This object: " + gameObject.name);
        Debug.Log("Combat found on THIS object: " + GetComponent<Player_Combat>());
    }

    void Update()
    {
        HandleMovementInput();
        HandleAttackInput();
    }

    void HandleMovementInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;
      
    }

    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Debug.Log("Attack button pressed");

            if (player_combat == null)
            {
                Debug.LogError("player_combat is NULL!");
                return;
            }

            player_combat.Attack();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;

        animator.SetFloat(Horizontal, movement.x);
        animator.SetFloat(Vertical, movement.y);
    }
}

