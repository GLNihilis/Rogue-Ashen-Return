using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Skeleton_NPC : MonoBehaviour
{
    TouchingDirections touchingDirections;
    Rigidbody2D rb;
    Animator animator;
    Damageable damageable;
    AudioSource audioSource;
    EnemySpawner enemySpawner;
    [SerializeField] AudioClip hurtSound;
    public DetectionZone cliffDetectionZone;
    public DetectionZone attackZone;

    [Header("Walk Settings")]
    [SerializeField] private float walkAcceleration = 30f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float walkStopRate = 0.5f;

    [Header("Patrol Settings")]
    //[SerializeField] protected float patrolRange = 5f;
    //protected Vector2 startPoint;
    [SerializeField] private float waitTimer = 2f;
    private float timer;

    [Header("Chase Settings")]
    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float chaseSpeed = 3f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDistance = 1.5f;

    [Header("Status Settings")]
    [SerializeField] private bool _isIdle = false;
    public bool IsIdle
    {
        get
        {
            return _isIdle;
        }
        set
        {
            _isIdle = value;
            animator.SetBool(AnimationsString.isIdle, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationsString.canMove);
        }
    }

    [SerializeField] private bool _hasTarget = false;
    public bool HasTarget
    {
        get
        {
            return _hasTarget;
        }
        set
        {
            _hasTarget = value;
            animator.SetBool(AnimationsString.hasTarget, value);
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationsString.attackCooldown);
        }
        set
        {
            animator.SetFloat(AnimationsString.attackCooldown, Mathf.Max(value, 0));
        }
    }

    [Header("ReadOnly_Field")]
    [SerializeField] private float time_ReadOnly;

    private enum EnemyStates
    {
        Idle_State,
        Flip_State,
        Chase_State,
        Attack_State,
    }

    private EnemyStates currentEnemyStates;

    private void ChangeState(EnemyStates _newState)
    {
        currentEnemyStates = _newState;
    }

    private void UpdateEnemyStates()
    {
        float distance = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        switch (currentEnemyStates)
        {
            case EnemyStates.Idle_State:
                if (!touchingDirections.IsOnCeiling)
                {
                    if (touchingDirections.IsGrounded && touchingDirections.IsOnWall /* || cliffDetectionZone.dectectedColliders.Count == 0 */)
                    {
                        ChangeState(EnemyStates.Flip_State);
                    }
                }

                if (touchingDirections.IsGrounded && !damageable.lockVelocity)
                {
                    //if (Vector2.Distance(transform.position, startPoint) >= patrolRange)
                    //{
                    //    ChangeState(EnemyStates.Melee_Flip_State);
                    //}

                    if (distance < chaseDistance)
                    {
                        ChangeState(EnemyStates.Chase_State);
                    }
                    else
                    {
                        Movement();
                    }
                }

                break;

            case EnemyStates.Flip_State:
                timer += Time.deltaTime;
                IsIdle = true;

                if (timer > waitTimer)
                {
                    timer = 0;
                    IsIdle = false;
                    FlipDirection();
                    ChangeState(EnemyStates.Idle_State);
                }

                time_ReadOnly = timer;
                break;

            case EnemyStates.Chase_State:
                if (touchingDirections.IsGrounded)
                {
                    if (!damageable.lockVelocity)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, chaseSpeed * Time.deltaTime);
                    }

                    if (distance < attackDistance)
                    {
                        ChangeState(EnemyStates.Attack_State);
                    }
                    else if (distance > chaseDistance)
                    {
                        ChangeState(EnemyStates.Idle_State);
                    }

                    if (PlayerController.Instance.transform.position.x < transform.position.x)
                    {
                        WalkDirection = WalkableDirection.Left;
                    }
                    else
                    {
                        WalkDirection = WalkableDirection.Right;
                    }
                }

                break;

            case EnemyStates.Attack_State:
                if (touchingDirections.IsGrounded)
                {
                    if (distance < attackDistance)
                    {
                        rb.velocity = Vector2.zero;
                        HasTarget = attackZone.dectectedColliders.Count > 0;

                        if (AttackCooldown > 0)
                        {
                            AttackCooldown -= Time.deltaTime;
                        }

                        if (PlayerController.Instance.transform.position.x < transform.position.x)
                        {
                            WalkDirection = WalkableDirection.Left;
                        }
                        else
                        {
                            WalkDirection = WalkableDirection.Right;
                        }

                    }
                    else
                    {
                        if (distance > attackDistance && distance < chaseDistance)
                        {
                            ChangeState(EnemyStates.Chase_State);
                        }
                    }
                }

                break;
        }
    }

    private enum WalkableDirection { Right, Left }
    private Vector2 walkDirectionVector = Vector2.right;

    private WalkableDirection _walkDirection;
    private WalkableDirection WalkDirection
    {
        get
        {
            return _walkDirection;
        }
        set
        {
            if (_walkDirection != value)
            {
                // Direction flipped
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;
        }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        audioSource = GetComponent<AudioSource>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        //startPoint = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;
    }

    private void FixedUpdate()
    {
        //if (!touchingDirections.IsOnCeiling)
        //{
        //    if (touchingDirections.IsGrounded && touchingDirections.IsOnWall /* || cliffDetectionZone.dectectedColliders.Count == 0 */)
        //    {
        //        FlipDirection();
        //    }
        //}

        //if (!damageable.lockVelocity)
        //{
        //    Movement();
        //}

        UpdateEnemyStates();
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("Current walkable direction is not set to legal value of right or left");
        }
    }

    private void Movement()
    {
        if (CanMove)
        {
            float xVelocity = Mathf.Clamp(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed);

            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
        }
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        audioSource.PlayOneShot(hurtSound);

        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);

        if (!damageable.IsAlive)
        {
            StartCoroutine(Alive());
            enemySpawner.ReturnPool(gameObject);
        }
    }

    public void OnCliffDetected()
    {
        if (touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }

    public IEnumerator Alive()
    {
        damageable.IsAlive = true;
        damageable.Health = damageable.MaxHealth;
        ChangeState(EnemyStates.Idle_State);
        yield return null;
    }
}
