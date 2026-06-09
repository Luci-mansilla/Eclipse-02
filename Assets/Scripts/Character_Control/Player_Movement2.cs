using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 30f;

    private Rigidbody2D rb;
    private Animator _animator;
    private Vector2 movement;
    private const string _horizontal = "Horizontal";
    private const string _vertical = "Vertical";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Prevent faster diagonal movement
        movement = movement.normalized;


    }

    void FixedUpdate()
    {
        // Move the player
        rb.linearVelocity = movement * moveSpeed;

        _animator.SetFloat(_horizontal, movement.x);
        _animator.SetFloat(_vertical, movement.y);

    }
}

