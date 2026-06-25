using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float runSpeed = 45f;
    public KeyCode runKey = KeyCode.LeftShift;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    public Player_Combat player_combat;

    [Header("Sonido de pasos")]
    public AudioSource walkAudioSource;
    public AudioClip walkSound;
    public AudioClip runSound;

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

        player_combat.SetAttackDirection(movement);

        bool isMoving = movement != Vector2.zero;
        bool isRunning = isMoving && Input.GetKey(runKey);

        if (isMoving)
        {
            AudioClip clipToPlay = isRunning ? runSound : walkSound;

            if (walkAudioSource != null && clipToPlay != null)
            {
                if (walkAudioSource.clip != clipToPlay)
                {
                    walkAudioSource.Stop();
                    walkAudioSource.clip = clipToPlay;
                }

                walkAudioSource.loop = true;

                if (!walkAudioSource.isPlaying)
                {
                    walkAudioSource.Play();
                }
            }
        }
        else
        {
            if (walkAudioSource != null && walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
        }
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
        float currentSpeed = Input.GetKey(runKey) ? runSpeed : moveSpeed;

        rb.linearVelocity = movement * currentSpeed;

        animator.SetFloat(Horizontal, movement.x);
        animator.SetFloat(Vertical, movement.y);
    }
}