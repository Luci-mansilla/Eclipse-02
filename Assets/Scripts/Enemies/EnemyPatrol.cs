using UnityEngine;

// ================================================================
//  EnemyPatrol — 8 direcciones con Blend Tree isométrico
//
//  Tu Blend Tree tiene estas posiciones (Pos X / Pos Y):
//  Walk-Left          (-1,  0)
//  Walk-rigth         ( 1,  0)
//  Walk-up            ( 0,  1)
//  Walk-down          ( 0, -1)
//  Walk-Up and Rigth  ( 1,  1)
//  Walk-down and rigth( 1, -1)
//  Walk-down and Left (-1, -1)
//  Walk-Up and Left   (-1,  1)
// ================================================================
public class EnemyPatrol : MonoBehaviour
{
    [Header("=== VELOCIDAD ===")]
    public float speed = 2f;

    [Header("=== PERSPECTIVA ISOMÉTRICA ===")]
    [Tooltip("Compresión del movimiento FÍSICO en Y (no afecta el Animator). Usar 0.5 para isométrico.")]
    public float isometricYScale = 0.5f;

    [Header("=== PATRULLA ===")]
    public float waitTime = 1.5f;
    public float detectionRadius = 0.4f;

    [Header("=== ÁREA DE PATRULLA ===")]
    public float areaWidth = 5f;
    public float areaHeight = 3f;

    [Header("=== COLISIONES ===")]
    public float collisionWaitTime = 0.5f;

    [Header("=== ANIMATOR — Parámetros ===")]
    public string paramHorizontal = "Horizontal";
    public string paramVertical = "Vertical";
    public string paramSpeed = "Speed";

    [Header("=== SONIDO CAMINAR ===")]
    public AudioSource walkAudioSource;
    public AudioClip walkSound;
    [Range(0f, 1f)]
    public float walkVolume = 0.4f;

    // ── Acceso público para EnemyAttack ──
    [HideInInspector] public bool attackingOverride = false;

    // ── Privadas ──
    private Vector2 targetPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Vector2 startPosition;
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyHealth health; 
    private bool collidedRecently = false;
  

    // Las 8 direcciones para elegir el punto destino (movimiento físico, Y comprimido)
    // Los parámetros del Animator van NORMALIZADOS a -1/0/1 (ver SetBlend)
    private Vector2[] moveDirections;

    void Start()
    {
        health = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rb == null) Debug.LogError("Falta Rigidbody2D en " + gameObject.name);
        if (anim == null) Debug.LogWarning("No se encontró Animator en " + gameObject.name);
        if (walkAudioSource == null)
         walkAudioSource = GetComponent<AudioSource>();
        BuildDirections();
        startPosition = transform.position;
        ChooseNewTarget();
    }

    void BuildDirections()
    {
        // Movimiento físico: Y comprimido para que se vea correcto en isométrico
        float y = isometricYScale;
        moveDirections = new Vector2[]
        {
            new Vector2( 1,  0),                     // Derecha
            new Vector2(-1,  0),                     // Izquierda
            new Vector2( 0,  y),                     // Arriba
            new Vector2( 0, -y),                     // Abajo
            new Vector2( 1,  y).normalized,          // ↗ Arriba-Derecha
            new Vector2(-1,  y).normalized,          // ↖ Arriba-Izquierda
            new Vector2( 1, -y).normalized,          // ↘ Abajo-Derecha
            new Vector2(-1, -y).normalized           // ↙ Abajo-Izquierda
        };
    }


    void Update()
    {

        if (health != null && health.IsInKnockback())
        {
            return;
        }

        if (attackingOverride)
        {
            SetBlend(Vector2.zero);
            return;
        }
        
        if (attackingOverride)
        {
        rb.linearVelocity = Vector2.zero;
        SetBlend(Vector2.zero);
        StopWalkSound();
        return;
        }

        if (isWaiting)
        {
            rb.linearVelocity = Vector2.zero;
            SetBlend(Vector2.zero);   // Speed = 0 → Idle
            StopWalkSound();

            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                collidedRecently = false;
                ChooseNewTarget();
            }
        }
        else
        {
            MoveToTarget();

            if (Vector2.Distance(transform.position, targetPoint) <= detectionRadius)
            {
                isWaiting = true;
                waitTimer = waitTime;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void MoveToTarget()
    {
        Vector2 raw = targetPoint - (Vector2)transform.position;

        // Dirección de MOVIMIENTO FÍSICO (Y comprimido para isométrico)
        Vector2 moveDir = new Vector2(raw.x, raw.y * isometricYScale).normalized;
        rb.linearVelocity = moveDir * speed;

        // Dirección para el ANIMATOR (Y sin comprimir, normalizado a -1/0/1)
        // Así el Blend Tree elige la animación correcta
        Vector2 animDir = GetAnimatorDirection(raw.normalized);
        SetBlend(animDir);
        PlayWalkSound();
    }

    // Convierte la dirección real a la dirección más cercana de las 8 del Blend Tree
    // usando los valores exactos que configuraste: (-1,0), (1,0), (0,1), (0,-1), etc.
    Vector2 GetAnimatorDirection(Vector2 rawDir)
    {
        float angle = Mathf.Atan2(rawDir.y, rawDir.x) * Mathf.Rad2Deg;

        // Mapea el ángulo a una de las 8 direcciones del Blend Tree
        // Ángulos: 0=derecha, 90=arriba, 180=izquierda, -90=abajo
        if (angle > 67.5f && angle <= 112.5f) return new Vector2(0, 1);  // Walk-up
        else if (angle > 112.5f && angle <= 157.5f) return new Vector2(-1, 1);  // Walk-Up and Left
        else if (angle > 157.5f || angle <= -157.5f) return new Vector2(-1, 0);  // Walk-Left
        else if (angle > -157.5f && angle <= -112.5f) return new Vector2(-1, -1);  // Walk-down and Left
        else if (angle > -112.5f && angle <= -67.5f) return new Vector2(0, -1);  // Walk-down
        else if (angle > -67.5f && angle <= -22.5f) return new Vector2(1, -1);  // Walk-down and rigth
        else if (angle > -22.5f && angle <= 22.5f) return new Vector2(1, 0);  // Walk-rigth
        else return new Vector2(1, 1);  // Walk-Up and Rigth
    }

    // Envía los valores al Blend Tree
    void SetBlend(Vector2 dir)
    {
        if (anim == null) return;
        anim.SetFloat(paramHorizontal, dir.x);
        anim.SetFloat(paramVertical, dir.y);
        anim.SetFloat(paramSpeed, dir.magnitude);
    }

    void ChooseNewTarget()
    {
        Shuffle(moveDirections);
        Vector2 dir = moveDirections[Random.Range(0, moveDirections.Length)];
        float dist = Random.Range(1.5f, 3.5f);
        Vector2 newTarget = (Vector2)transform.position + dir * dist;

        newTarget.x = Mathf.Clamp(newTarget.x, startPosition.x - areaWidth, startPosition.x + areaWidth);
        newTarget.y = Mathf.Clamp(newTarget.y, startPosition.y - areaHeight, startPosition.y + areaHeight);

        targetPoint = newTarget;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Si EnemyAttack tiene el control (atacando), que EnemyAttack decida qué hacer.
        // EnemyPatrol no debe interferir frenando o "esperando" en este caso.
        if (attackingOverride) return;

        // Tampoco reaccionar ante el propio Player chocando mientras patrulla,
        // así no se queda "trabado" empujándolo en vez de dejar que EnemyAttack ataque
        if (col.gameObject.CompareTag("Player")) return;

        if (collidedRecently) return;
        collidedRecently = true;
        rb.linearVelocity = Vector2.zero;
        isWaiting = true;
        waitTimer = collisionWaitTime;
    }

    void Shuffle(Vector2[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Vector2 temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
    }

    void PlayWalkSound()
    {    
    if (walkAudioSource != null && walkSound != null && !walkAudioSource.isPlaying)
    {
        walkAudioSource.clip = walkSound;
        walkAudioSource.loop = true;
        walkAudioSource.volume = walkVolume;
        walkAudioSource.Play();
    }
    }

    void StopWalkSound()
    {
    if (walkAudioSource != null && walkAudioSource.isPlaying)
    {
        walkAudioSource.Stop();
    }
    }

    void OnDrawGizmos()
    {
        Vector2 center = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, new Vector3(areaWidth * 2, areaHeight * isometricYScale * 2, 0));

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPoint, detectionRadius);
        }
    }
}
