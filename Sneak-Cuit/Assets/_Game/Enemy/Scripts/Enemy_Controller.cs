
using TreeEditor;
using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    private enum GuardState
    {
        Patrolling,
        Chasing
    }
    private GuardState currentState = GuardState.Patrolling;

    [Header("Dependencies")]
    private Transform player;
    public GameObject playerObject;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] Rigidbody2D enemyRb;
    [SerializeField] BoxCollider2D enemyCollider;
    private int currentIndex = 0;

    [Header("Patrol Settings")]
    public Transform[] wayPoints;
    

    [Header("Detection Settings")]
    public float visionRange = 5f;
    public float visionAngle = 45f;
    public float loseSightTime = 2f;

    [Header("Catch Settings")]
    public float catchDistance = 1f;

    [Header("Speeds")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Alert Icon")]
    public GameObject alertIcon;

    [Header("Game Over UI")]
    public GameObject gameOverUI;

    private float loseSightTimer = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case GuardState.Patrolling:
                enemyCollider.enabled = false;
                Patrol();
                alertIcon.SetActive(false);
                if (CanSeePlayer())
                {
                    currentState = GuardState.Chasing;
                    loseSightTimer = 0f;
                    alertIcon.SetActive(true);
                }
                break;

            case GuardState.Chasing:
                enemyCollider.enabled = true;
                Chase();
                if (CanSeePlayer())
                {
                    loseSightTimer = 0f;
                }
                else
                {
                    loseSightTimer += Time.deltaTime;
                    if (loseSightTimer >= loseSightTime)
                    {
                        currentState = GuardState.Patrolling;
                        alertIcon.SetActive(false);
                    }
                }
                break;
        }
    }

    

    private void Patrol()
    {
        Transform target = wayPoints[currentIndex];

        //Calculate direction
        Vector2 direction = (target.position - transform.position).normalized;

        //Move using RigidBody2D
        Vector2 newPosition = Vector2.MoveTowards(enemyRb.position, target.position, patrolSpeed * Time.fixedDeltaTime);
        enemyRb.MovePosition(newPosition);

        //Flip sprite based on position
        if (direction.x > 0)
        {
            _spriteRenderer.flipX = false; //Facing right
        }
        else if (direction.x < 0)
        {
            _spriteRenderer.flipX = true; //Facing left
        }

        //Check if reached waypoint
        if (Vector2.Distance(enemyRb.position, target.position) < 0.1f)
        {
            currentIndex++;
            if (currentIndex >= wayPoints.Length)
            {
                currentIndex = 0;
            }
        }
    }


    private bool CanSeePlayer()
    {
        //Check distance first 
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > visionRange) return false;

        //Check if inside vision cone
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        //Determine which way the guard is facing
        Vector2 facingDirection = _spriteRenderer.flipX ? Vector2.left : Vector2.right;

        float angle = Vector2.Angle(facingDirection, directionToPlayer);
        if (angle > visionAngle) return false;

        //Check for obstacles using Linecast
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask);
        if (hit.collider != null)
        {
            //If we hit something before the player, vision is blocked
            if (!hit.collider.CompareTag("Player"))
            {
                return false;
            }
        }

        return true;
    }

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        Vector2 direction = (player.position - transform.position).normalized;
        _spriteRenderer.flipX = direction.x < 0;

        if (Vector2.Distance(transform.position, player.position) <= catchDistance)
        {
            PlayerCaught();
        }
    }

    private void PlayerCaught()
    {
        gameOverUI.SetActive(true);
        Destroy(playerObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector2 facingDirection = _spriteRenderer != null && _spriteRenderer.flipX ? Vector2.left: Vector2.right;
        Vector2 leftBoundary = Quaternion.Euler(0,0, visionAngle) * facingDirection;
        Vector2 rightBoundary = Quaternion.Euler(0,0, -visionAngle) * facingDirection;

        Gizmos.DrawLine(transform.position, transform.position + (Vector3)leftBoundary * visionRange);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)rightBoundary * visionRange);
    }
}
