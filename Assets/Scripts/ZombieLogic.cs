using UnityEngine;
using System.Collections;

public class ZombieLogic : MonoBehaviour
{
    private enum ZombieState { Wandering, Chasing, Attacking }
    private ZombieState currentState = ZombieState.Wandering;

    [Header("Health & Stats")]
    public float health = 100f;
    private bool isDead = false;

    [Header("Movement & Swarming")]
    public float minMoveSpeed = 1.5f;
    public float maxMoveSpeed = 4.0f;
    private float currentMoveSpeed;
    private Vector3 moveDirection = Vector3.zero;

    [Header("AI Logic Ranges")]
    public float agroRange = 10f;       
    public float attackRange = 1.8f;    
    public float attackCooldown = 1.5f; 
    private float lastAttackTime;

    [Header("Wandering Settings")]
    public float wanderRadius = 5f;
    private Vector3 wanderTarget;
    private float wanderTimer;
    public float wanderWaitTime = 2f;

    [Header("References")]
    public Animator anim;
    private Transform player;
    private Rigidbody rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Swarming Feature: Give every zombie a different random speed
        currentMoveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

        GetNewWanderTarget();
    }

    // void Update()
    // {
    //     if (isDead) return;

    //     if (player == null)
    //     {
    //         GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
    //         if (playerObj != null) player = playerObj.transform;
    //         else return; 
    //     }

    //     float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    //     // --- State Machine Switching Logic ---
    //     if (distanceToPlayer <= attackRange)
    //     {
    //         currentState = ZombieState.Attacking;
    //     }
    //     else if (distanceToPlayer <= agroRange)
    //     {
    //         currentState = ZombieState.Chasing;
    //     }
    //     else
    //     {
    //         if (currentState == ZombieState.Chasing) 
    //         {
    //             currentState = ZombieState.Wandering;
    //             GetNewWanderTarget();
    //         }
    //     }

    //     // --- Handle Logic (Rotation & Direction Calculation) ---
    //     switch (currentState)
    //     {
    //         case ZombieState.Wandering:
    //             CalculateWander();
    //             break;
    //         case ZombieState.Chasing:
    //             CalculateChase();
    //             break;
    //         case ZombieState.Attacking:
    //             CalculateAttack();
    //             break;
    //     }
    // }

    void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            else return; 
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // --- State Machine Switching Logic ---
        if (distanceToPlayer <= attackRange)
        {
            currentState = ZombieState.Attacking;
        }
        else if (distanceToPlayer <= agroRange)
        {
            currentState = ZombieState.Chasing;
        }
        else
        {
            if (currentState == ZombieState.Chasing) 
            {
                currentState = ZombieState.Wandering;
                GetNewWanderTarget();
            }
        }

        // --- Handle Logic (Only Triggers/Timers here, NO movement/rotation) ---
        // if (currentState == ZombieState.Attacking)
        // {
        //     if (anim.GetBool("isWalking")) anim.SetBool("isWalking", false);

        //     if (Time.time - lastAttackTime >= attackCooldown)
        //     {
        //         anim.SetTrigger("Attack");
        //         lastAttackTime = Time.time;
        //         Debug.Log("Zombie bit the player!");
        //     }
        // }

        if (currentState == ZombieState.Attacking)
        {
            if (anim.GetBool("isWalking")) anim.SetBool("isWalking", false);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                anim.SetTrigger("Attack");
                lastAttackTime = Time.time;

                // --- DEAL DAMAGE TO PLAYER ---
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(15f); // Deals 15 damage per bite
                }
                else
                {
                    Debug.LogWarning("Zombie is biting, but the Player GameObject is missing the PlayerHealth script!");
                }
            }
        }

        else if (currentState == ZombieState.Wandering)
        {
            Vector3 targetPos = new Vector3(wanderTarget.x, transform.position.y, wanderTarget.z);
            if (Vector3.Distance(transform.position, targetPos) < 0.6f)
            {
                wanderTimer += Time.deltaTime;
                if (anim.GetBool("isWalking")) anim.SetBool("isWalking", false);
                moveDirection = Vector3.zero;

                if (wanderTimer >= wanderWaitTime)
                {
                    GetNewWanderTarget();
                    wanderTimer = 0f;
                }
            }
            else
            {
                if (!anim.GetBool("isWalking")) anim.SetBool("isWalking", true);
            }
        }
        else if (currentState == ZombieState.Chasing)
        {
            if (!anim.GetBool("isWalking")) anim.SetBool("isWalking", true);
        }
    }

    // Physics movement updates MUST happen in FixedUpdate to prevent lagging/shaking
    // void FixedUpdate()
    // {
    //     if (isDead) return;

    //     if (currentState == ZombieState.Attacking || !anim.GetBool("isWalking"))
    //     {
    //         // Stop forward momentum but retain gravity
    //         rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    //     }
    //     else
    //     {
    //         // Apply forward velocity safely using Unity's physics system
    //         Vector3 velocity = moveDirection * currentMoveSpeed;
    //         rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    //     }
    // }

    void FixedUpdate()
    {
        if (isDead) return;

        Vector3 targetPos = Vector3.zero;

        if (currentState == ZombieState.Wandering)
        {
            targetPos = new Vector3(wanderTarget.x, transform.position.y, wanderTarget.z);
            moveDirection = (targetPos - transform.position).normalized;
        }
        else if (currentState == ZombieState.Chasing || currentState == ZombieState.Attacking)
        {
            targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            moveDirection = (targetPos - transform.position).normalized;
        }

        // 1. Handle Smooth Physics Rotation
        if (moveDirection != Vector3.zero)
        {
            SmoothLookAt(targetPos);
        }

        // 2. Handle Physics Velocity Movement
        if (currentState == ZombieState.Attacking || !anim.GetBool("isWalking"))
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
        else
        {
            Vector3 velocity = moveDirection * currentMoveSpeed;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }
    }

    void CalculateWander()
    {
        if (!anim.GetBool("isWalking")) anim.SetBool("isWalking", true);

        Vector3 targetPos = new Vector3(wanderTarget.x, transform.position.y, wanderTarget.z);
        moveDirection = (targetPos - transform.position).normalized;
        
        SmoothLookAt(targetPos);

        if (Vector3.Distance(transform.position, targetPos) < 0.6f)
        {
            wanderTimer += Time.deltaTime;
            if (anim.GetBool("isWalking")) anim.SetBool("isWalking", false);
            moveDirection = Vector3.zero;

            if (wanderTimer >= wanderWaitTime)
            {
                GetNewWanderTarget();
                wanderTimer = 0f;
            }
        }
    }

    void GetNewWanderTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        wanderTarget = new Vector3(transform.position.x + randomCircle.x, transform.position.y, transform.position.z + randomCircle.y);
    }

    void CalculateChase()
    {
        if (!anim.GetBool("isWalking")) anim.SetBool("isWalking", true);

        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        moveDirection = (targetPos - transform.position).normalized;
        
        SmoothLookAt(targetPos);
    }

    void CalculateAttack()
    {
        if (anim.GetBool("isWalking")) anim.SetBool("isWalking", false);
        moveDirection = Vector3.zero;

        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        SmoothLookAt(targetPos);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
            Debug.Log("Zombie bit the player!");
        }
    }

    // void SmoothLookAt(Vector3 target)
    // {
    //     Vector3 direction = (target - transform.position).normalized;
    //     direction.y = 0; // Absolute safety lock to keep rotation perfectly horizontal

    //     if (direction != Vector3.zero)
    //     {
    //         Quaternion targetRotation = Quaternion.LookRotation(direction);
    //         // Slerp the transform directly since velocity-based physics won't fight it anymore
    //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
    //     }
    // }

// void SmoothLookAt(Vector3 target)
// {
//     Vector3 direction = (target - transform.position);
//     direction.y = 0; // Keep it flat on the ground

//     if (direction.magnitude > 0.1f)
//     {
//         Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
//         // Snaps the transform rotation cleanly
//         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
//     }
// }

    void SmoothLookAt(Vector3 target)
    {
        Vector3 direction = (target - transform.position);
        direction.y = 0; // Lock vertical axis rotation

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            // Using rb.MoveRotation inside FixedUpdate keeps physics in perfect sync
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        anim.SetTrigger("Hit"); 
        currentState = ZombieState.Chasing;

        if (health <= 0)
        {
            StartCoroutine(HandleDeath());
        }
    }

    IEnumerator HandleDeath()
    {
        isDead = true;
        anim.SetTrigger("Die"); 
        moveDirection = Vector3.zero;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.None; 
        }

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            capsule.direction = 2; 
            capsule.height = 0.5f; 
        }

        yield return new WaitForSeconds(5f); 
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, agroRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}