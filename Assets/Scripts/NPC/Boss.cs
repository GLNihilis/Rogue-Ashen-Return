using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Boss : MonoBehaviour
{
    TouchingDirections touchingDirections;
    Rigidbody2D rb;
    Animator animator;
    public Damageable damageable;
    SpriteRenderer sr;
    public static Boss Instance { get; private set; }
    public DetectionZone detectionZone;

    [Header("Walk Settings")]
    //[SerializeField] private float walkAcceleration = 30f;
    //[SerializeField] private float maxSpeed = 3f;
    //[SerializeField] private float walkStopRate = 0.5f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseDistance = 25f;
    [SerializeField] private float chaseSpeed = 3f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDistance = 3f;

    [Header("Enrage Settings")]
    [SerializeField] private float enragedHealth = 250f;
    [SerializeField] private float enragedSpeed = 6f;
    [SerializeField] private float enragedAttackDistance = 5.5f;

    [Header("Spell Settings")]
    [SerializeField] GameObject bossHand;
    [SerializeField] private float hand_CastDistance = 17f;
    [SerializeField] private float hand_SummonDelay = 1f;
    [SerializeField] private float hand_SummonHeight = 3f;
    [SerializeField] private float hand_CastCooldown = 20f;
    private float hand_CurrentCastTimer;
    [SerializeField] GameObject bossCurse;
    [SerializeField] private float curse_CastDistance = 10f;
    [SerializeField] private float curse_SummonDelay = 1f;
    [SerializeField] private float curse_SummonHeight = -0.5f;
    [SerializeField] private float curse_CastCooldown = 17f;
    private float curse_CurrentCastTimer;
    [SerializeField] GameObject shadowBall;
    [SerializeField] Transform attackPoint;
    [SerializeField] private float shadow_CastDistance = 15f;
    [SerializeField] private float shadow_SummonDelay = 1f;
    [SerializeField] private float shadow_CastCooldown = 13f;
    private float shadow_CurrentCastTimer;
    
    [Header("ReadOnly_Attribute")]
    [SerializeField] private float hand_CurrentCastTimer_ReadOnly;
    [SerializeField] private float curse_CurrentCastTimer_ReadOnly;
    [SerializeField] private float shadow_CurrentCastTimer_ReadOnly;

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

    [SerializeField] private bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationsString.isMoving, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationsString.canMove);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationsString.isAlive);
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

    [SerializeField] private bool _isCast = false;
    public bool IsCast
    {
        get
        {
            return _isCast;
        }
        set
        {
            _isCast = value;
            animator.SetBool(AnimationsString.isCast, value);
        }
    }

    [Header("ReadOnly_Field")]
    [SerializeField] private float time_ReadOnly;

    private enum EnemyStates
    {
        Bringer_Idle_State,
        Bringer_Chase_State,
        Bringer_Attack_State,
        Bringer_Enraged_State,
        Bringer_Hand_Spell_State,
    }

    private EnemyStates currentEnemyStates;

    private void ChangeState(EnemyStates _newState)
    {
        currentEnemyStates = _newState;
    }

    private void UpdateEnemyStates()
    {
        if (PlayerController.Instance != null)
        {
            float distance = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

            switch (currentEnemyStates)
            {
                case EnemyStates.Bringer_Idle_State:
                    if (distance < chaseDistance)
                    {
                        ChangeState(EnemyStates.Bringer_Chase_State);
                        IsIdle = false;
                        IsMoving = true;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                        IsIdle = true;
                        IsMoving = false;
                    }

                    break;

                case EnemyStates.Bringer_Chase_State:
                    if (touchingDirections.IsGrounded)
                    {
                        if (!damageable.lockVelocity)
                        {
                            transform.position = Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, chaseSpeed * Time.deltaTime);
                        }

                        if (distance < attackDistance)
                        {
                            ChangeState(EnemyStates.Bringer_Attack_State);
                        }
                        else if (distance > chaseDistance)
                        {
                            ChangeState(EnemyStates.Bringer_Idle_State);
                        }

                        if (PlayerController.Instance.transform.position.x < transform.position.x)
                        {
                            WalkDirection = WalkableDirection.Right;
                        }
                        else
                        {
                            WalkDirection = WalkableDirection.Left;
                        }

                        if (damageable.Health <= enragedHealth)
                        {
                            ChangeState(EnemyStates.Bringer_Enraged_State);
                        }

                        if (hand_CurrentCastTimer <= 0 && !IsCast && distance < hand_CastDistance)
                        {
                            Hand_CastSpell();
                            hand_CurrentCastTimer = hand_CastCooldown;
                        }

                        if (curse_CurrentCastTimer <= 0 && !IsCast && distance < curse_CastDistance)
                        {
                            Curse_CastSpell();
                            curse_CurrentCastTimer = curse_CastCooldown;
                        }

                        if (shadow_CurrentCastTimer <= 0 && !IsCast && distance < shadow_CastDistance)
                        {
                            ShadowBall_CastSpell();
                            shadow_CurrentCastTimer = shadow_CastCooldown;
                        }
                    }

                    break;

                case EnemyStates.Bringer_Attack_State:
                    if (touchingDirections.IsGrounded)
                    {
                        if (distance < attackDistance)
                        {
                            rb.velocity = Vector2.zero;
                            HasTarget = detectionZone.dectectedColliders.Count > 0;

                            if (AttackCooldown > 0)
                            {
                                AttackCooldown -= Time.deltaTime;
                            }

                            if (PlayerController.Instance.transform.position.x < transform.position.x)
                            {
                                WalkDirection = WalkableDirection.Right;
                            }
                            else
                            {
                                WalkDirection = WalkableDirection.Left;
                            }
                        }
                        else
                        {
                            if (distance > attackDistance && distance < chaseDistance)
                            {
                                ChangeState(EnemyStates.Bringer_Chase_State);
                            }
                        }

                        if (damageable.Health <= enragedHealth)
                        {
                            ChangeState(EnemyStates.Bringer_Enraged_State);
                        }

                        if (hand_CurrentCastTimer <= 0 && !IsCast && distance < hand_CastDistance)
                        {
                            Hand_CastSpell();
                            hand_CurrentCastTimer = hand_CastCooldown;
                        }

                        if (curse_CurrentCastTimer <= 0 && !IsCast && distance < curse_CastDistance)
                        {
                            Curse_CastSpell();
                            curse_CurrentCastTimer = curse_CastCooldown;
                        }

                        if (shadow_CurrentCastTimer <= 0 && !IsCast && distance < shadow_CastDistance)
                        {
                            ShadowBall_CastSpell();
                            shadow_CurrentCastTimer = shadow_CastCooldown;
                        }
                    }

                    break;

                case EnemyStates.Bringer_Enraged_State:
                    if (touchingDirections.IsGrounded)
                    {
                        sr.color = new Color(1, 0.39f, 0.39f, 1);

                        if (!damageable.lockVelocity)
                        {
                            transform.position = Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, enragedSpeed * Time.deltaTime);
                        }

                        if (distance < enragedAttackDistance)
                        {
                            rb.velocity = Vector2.zero;
                            HasTarget = detectionZone.dectectedColliders.Count > 0;

                            if (AttackCooldown > 0)
                            {
                                AttackCooldown -= 0.2f;
                            }
                        }

                        if (PlayerController.Instance.transform.position.x < transform.position.x)
                        {
                            WalkDirection = WalkableDirection.Right;
                        }
                        else
                        {
                            WalkDirection = WalkableDirection.Left;
                        }

                        if (hand_CurrentCastTimer <= 0 && !IsCast && distance < hand_CastDistance)
                        {
                            Hand_CastSpell();
                            hand_CurrentCastTimer = hand_CastCooldown;
                        }

                        if (curse_CurrentCastTimer <= 0 && !IsCast && distance < curse_CastDistance)
                        {
                            Curse_CastSpell();
                            curse_CurrentCastTimer = curse_CastCooldown;
                        }

                        if (shadow_CurrentCastTimer <= 0 && !IsCast && distance < shadow_CastDistance)
                        {
                            ShadowBall_CastSpell();
                            shadow_CurrentCastTimer = shadow_CastCooldown;
                        }
                    }
                    break;
            }
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
        sr = GetComponent<SpriteRenderer>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(WaitOnSummon());

        hand_CurrentCastTimer = hand_CastCooldown;
        curse_CurrentCastTimer = curse_CastCooldown;
        shadow_CurrentCastTimer = shadow_CastCooldown;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;
    }

    private void FixedUpdate()
    {
        UpdateEnemyStates();

        if (hand_CurrentCastTimer > 0)
        {
            hand_CurrentCastTimer -= Time.deltaTime;
        }

        if (curse_CurrentCastTimer > 0)
        {
            curse_CurrentCastTimer -= Time.deltaTime;
        }

        if (shadow_CurrentCastTimer > 0)
        {
            shadow_CurrentCastTimer -= Time.deltaTime;
        }

        if (!IsAlive)
        {
            //SpawnBoss.Instance.IsNotTriggered();
        }

        hand_CurrentCastTimer_ReadOnly = hand_CurrentCastTimer;
        curse_CurrentCastTimer_ReadOnly = curse_CurrentCastTimer;
        shadow_CurrentCastTimer_ReadOnly = shadow_CurrentCastTimer;
    }

    //private void FlipDirection()
    //{
    //    if (WalkDirection == WalkableDirection.Right)
    //    {
    //        WalkDirection = WalkableDirection.Left;
    //    }
    //    else if (WalkDirection == WalkableDirection.Left)
    //    {
    //        WalkDirection = WalkableDirection.Right;
    //    }
    //    else
    //    {
    //        Debug.LogError("Current walkable direction is not set to legal value of right or left");
    //    }
    //}

    //private void Movement()
    //{
    //    if (CanMove)
    //    {
    //        float xVelocity = Mathf.Clamp(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed);

    //        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    //    }
    //    else
    //    {
    //        rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
    //    }
    //}

    public void OnHit(float damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);

        if (!Boss.Instance.IsAlive)
        {
            SpawnBoss.Instance.bossUI.SetActive(false);
            StartCoroutine(GameManager.Instance.ActiveVictoryScreen());
        }
    }

    private IEnumerator WaitOnSummon()
    {
        damageable.IsInvincible = true;
        yield return new WaitForSeconds(2f);

        SpawnBoss.Instance.bossUI.SetActive(true);
        ChangeState(EnemyStates.Bringer_Idle_State);
    }

    private IEnumerator SummonHandAtPosition(Vector2 playerPosition)
    {
        Vector2 summonPosition = playerPosition + Vector2.up * hand_SummonHeight;

        yield return new WaitForSeconds(hand_SummonDelay);

        Instantiate(bossHand, summonPosition, Quaternion.identity);

        IsIdle = false;
        IsCast = false;
        IsMoving = true;
    }

    private void Hand_CastSpell()
    {
        if (PlayerController.Instance != null)
        {
            rb.velocity = Vector2.zero;
            Vector2 playerPosition = PlayerController.Instance.transform.position;
            IsIdle = true;
            IsCast = true;
            IsMoving = false;

            StartCoroutine(SummonHandAtPosition(playerPosition));

            Debug.Log("Boss cast Hand_Spell");
        }
    }

    private IEnumerator SummonCurseAtPosition(Vector2 playerPosition)
    {
        Vector2 summonPosition = playerPosition + Vector2.up * curse_SummonHeight;

        yield return new WaitForSeconds(curse_SummonDelay);

        Instantiate(bossCurse, summonPosition, Quaternion.identity);

        IsIdle = false;
        IsCast = false;
        IsMoving = true;
    }

    private void Curse_CastSpell()
    {
        if (PlayerController.Instance != null)
        {
            rb.velocity = Vector2.zero;
            Vector2 playerPosition = PlayerController.Instance.transform.position;
            IsIdle = true;
            IsCast = true;
            IsMoving = false;

            StartCoroutine(SummonCurseAtPosition(playerPosition));

            Debug.Log("Boss cast Curse_Spell");
        }
    }

    private IEnumerator SummonShadowAtPosition()
    {
        yield return new WaitForSeconds(shadow_SummonDelay);

        GameObject projectile = Instantiate(shadowBall, attackPoint.position, Quaternion.identity);

        if (WalkDirection == WalkableDirection.Left)
        {
            projectile.transform.eulerAngles = Vector3.zero;
        }
        else
        {
            projectile.transform.eulerAngles = new Vector2(projectile.transform.eulerAngles.x, 180);
        }

        IsIdle = false;
        IsCast = false;
        IsMoving = true;
    }

    private void ShadowBall_CastSpell()
    {
        if (PlayerController.Instance != null)
        {
            rb.velocity = Vector2.zero;
            IsIdle = true;
            IsCast = true;
            IsMoving = false;

            StartCoroutine(SummonShadowAtPosition());

            Debug.Log("Boss cast Shadow_Spell");
        }
    }

    
}
