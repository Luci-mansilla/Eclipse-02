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

    // ================= DASH =================
    [Header("Dash")]
    public float dashSpeed = 100f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;

    // ---------- ADDED ----------
    public AudioSource dashAudioSource;
    public TrailRenderer dashTrail;
    // ---------------------------

    private bool isDashing = false;
    private float dashTime;
    private float dashCooldownTimer;
    private Vector2 dashDirection;
    // ========================================

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

        // ---------- ADDED ----------
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }
        // ---------------------------
    }

    void Update()
    {
        HandleMovementInput();
        HandleAttackInput();

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && dashCooldownTimer <= 0)
        {
            StartDash();
        }
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

    // ================= DASH =================
    void StartDash()
    {
        if (movement == Vector2.zero)
            return;

        isDashing = true;
        dashTime = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDirection = movement;

        // ---------- ADDED ----------
        if (dashTrail != null)
        {
            dashTrail.emitting = true;
        }

        if (dashAudioSource != null)
        {
            dashAudioSource.Play();
        }
        // ---------------------------
    }

    void HandleDash()
    {
        dashTime -= Time.fixedDeltaTime;

        rb.linearVelocity = dashDirection * dashSpeed;

        if (dashTime <= 0)
        {
            isDashing = false;

            // ---------- ADDED ----------
            if (dashTrail != null)
            {
                dashTrail.emitting = false;
            }
            // ---------------------------
        }
    }
    // ========================================

    void FixedUpdate()
    {
        if (isDashing)
        {
            HandleDash();
            return;
        }

        float currentSpeed = Input.GetKey(runKey) ? runSpeed : moveSpeed;

        rb.linearVelocity = movement * currentSpeed;

        animator.SetFloat(Horizontal, movement.x);
        animator.SetFloat(Vertical, movement.y);
    }
}