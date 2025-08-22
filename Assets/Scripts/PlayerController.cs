using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    TouchingDirections touchingDirections;
    CapsuleCollider2D capsuleCollider;
    public Damageable damageable;
    public Spellable spellable;
    public Rigidbody2D rb;
    Animator animator;
    AudioSource audioSource;
    Vector2 moveInput;
    Vector2 vecGravity;

    [Header("Audio")]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip doubleJumpSound;
    [SerializeField] AudioClip landingSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip healSound;
    [SerializeField] AudioClip rollSound;
    [SerializeField] AudioClip spellCastSound_1;
    [SerializeField] AudioClip spellCastSound_2;
    [SerializeField] AudioClip spellCastSound_3;
    [SerializeField] AudioClip spellCastSound_4;

    [Header("Walking Settings")]
    [SerializeField] private float moveSpeed = 9f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float decceleration = 12f;
    [SerializeField] private float velPower = 0.9f;

    //[Header("Running Settings")]
    //public float runSpeed = 10f;
    
    [Header("Gravity Settings")]
    [SerializeField] private float airSpeed = 9f;
    [SerializeField] private float fallMultiplier = 3.5f;
    [SerializeField] private float fallClampSpeed = -30f;

    [Header("Jumping Settings")]
    [SerializeField] private float jumpForce = 30f;
    [SerializeField] private float jumpHeight = 0.3f;
    [SerializeField] private float jumpBufferTime = 0.25f;
    [SerializeField] private float jumpMultiplier = 2.8f;
    [SerializeField] private float coyoteTime = 0.2f;
    private float jumpBufferCounter = 0f;
    private float coyoteTimeCounter = 0f;

    [Header("Double Jump Settings")]
    [SerializeField] private float maxAirJumps = 1f;
    private float airJumpCounter = 0f;

    [Header("Roll Settings")]
    [SerializeField] private float rollSpeed = 12f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollCooldown = 1f;
    [SerializeField] private float rollCost = 20f;
    private float rollTimer = 0f;
    private float rollCooldownTimer = 0f;
    
    [Header("Heal Settings")]
    [SerializeField] private float healLevel_1 = 20f;
    [SerializeField] private float healCost_1 = 20f;
    [SerializeField] private float healLevel_2 = 40f;
    [SerializeField] private float healCost_2 = 40f;
    [SerializeField] private float healLevel_3 = 60f;
    [SerializeField] private float healCost_3 = 60f;
    [SerializeField] public float timeCast_9 = 1f;

    [Header("Spell Settings")]
    [SerializeField] private float spellMana_1 = 5f;
    [SerializeField] private float spellMana_2 = 10f;
    [SerializeField] private float spellMana_3 = 15f;
    [SerializeField] private float spellMana_4 = 20f;
    [SerializeField] private float summonDistance = 5f;
    [SerializeField] public float timeCast_1 = 3f;
    [SerializeField] public float timeCast_2 = 5f;
    [SerializeField] public float timeCast_3 = 7f;
    [SerializeField] public float timeCast_4 = 9f;
    [SerializeField] Transform SpellPoint_1, SpellPoint_2, SpellPoint_3, SpellPoint_4;
    [SerializeField] GameObject Spell_1;
    [SerializeField] GameObject Spell_2;
    [SerializeField] GameObject Spell_3;
    [SerializeField] GameObject Spell_4;
    [SerializeField] public bool isCasting;

    [Header("Other Settings")]
    [SerializeField] private float damageSelf = 45f;
    [SerializeField] public bool cutscene = false;
    [SerializeField] public bool landingSoundPlayed;
    [SerializeField] public bool isCheatMode;

    [Header("Player Status")]
    public bool _isFacingRight = true;
    public bool isFacingRight {
        get
        {
            return _isFacingRight;
        }
        private set
        {
            // Flip only if value is new
            if (_isFacingRight != value)
            {
                // Flip the local scale to make the player face the opposite direction
                transform.localScale *= new Vector2(-1, 1);
            }

            _isFacingRight = value;
        }
    }

    public bool CanMove {
        get
        {
            return animator.GetBool(AnimationsString.canMove);
        }}

    public bool IsAlive {
        get
        {
            return animator.GetBool(AnimationsString.isAlive);
        }}

    [SerializeField] private bool _isMoving = false;
    public bool IsMoving {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationsString.isMoving, value);
        }}

    //[SerializeField] private bool _isRunning = false;
    //public bool IsRunning
    //{
    //    get
    //    {
    //        return _isRunning;
    //    }
    //    private set
    //    {
    //        _isRunning = value;
    //        animator.SetBool(AnimationsString.isRunning, value);
    //    }
    //}

    [SerializeField] private bool _isJumping;
    public bool IsJumping
    {
        get
        {
            return _isJumping;
        }
        private set
        {
            _isJumping = value;
        }
    }

    [SerializeField] private bool _isRoll = false;
    public bool IsRolling
    {
        get
        {
            return _isRoll;
        }
        set
        {
            _isRoll = value;
        }
    }

    public bool unlocked_DoubleJump;
    public bool unlocked_Healing;
    public bool unlocked_Rolling;

    public bool unlocked_FireBall;
    public bool unlocked_WindSlash;
    public bool unlocked_EarthBump;
    public bool unlocked_WaterTornado;

    public bool unlocked_Key_1;
    public bool unlocked_Key_2;
    public bool unlocked_Key_3;
    public bool unlocked_Key_4;
    public bool unlocked_Key_5;

    [Header("ReadOnly_Attribute " + " >> Do Not Change <<")]
    [SerializeField] private float jumpBufferCounter_ReadOnly;
    [SerializeField] private float currentJumpMultiplier_ReadOnly;
    [SerializeField] private float coyoteTimeCounter_ReadOnly;
    [SerializeField] private float airJumpCounter_ReadOnly;
    [SerializeField] private float rollCooldownTimer_ReadOnly;
    [SerializeField] private float rollTimer_ReadOnly;
    public float timeSinceCast_1;
    public float timeSinceCast_2;
    public float timeSinceCast_3;
    public float timeSinceCast_4;
    public float timeSinceCast_9;

    public float currentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {
                        // Ground State
                        return moveSpeed;

                        //if (IsRunning)
                        //{
                        //    return runSpeed;
                        //}
                        //else
                        //{
                        //    return moveSpeed;
                        //}
                    }
                    else
                    {
                        // Air State
                        return airSpeed;
                    }
                }
                else
                {
                    // Idle State
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        spellable = GetComponent<Spellable>();
        audioSource = GetComponent<AudioSource>();

        //// Singleton Player
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance != null && !GameManager.Instance.gameIsPaused)
        {
            SaveData.Instance.Load_PlayerData();
        }

        vecGravity = new Vector2(0, -Physics2D.gravity.y);

        if (damageable.Health == 0)
        {
            damageable.IsAlive = false;
            GameManager.Instance.RespawnPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;

        if (cutscene) return;

        if (Input.GetKeyDown(KeyCode.F2))
        {
            damageable.Health -= damageSelf;
        }
    }

    private void FixedUpdate()
    {
        if (cutscene) return;

        UpdateVariables();

        coyoteTimeCounter_ReadOnly = coyoteTimeCounter; // No Function Just For Testing

        if (!damageable.lockVelocity)
        {
            //rb.velocity = new Vector2(moveInput.x * currentMoveSpeed, rb.velocity.y);
            Movement();
        }

        #region Jump Buffering
        // Jump Input Buffering
        if (rb.velocity.y > 0 && IsJumping == true)
        {
            jumpBufferCounter += Time.deltaTime;
            jumpBufferCounter_ReadOnly = jumpBufferCounter; // No Function Just For Testing

            if (jumpBufferCounter > jumpBufferTime)
            {
                IsJumping = false;
            }

            float jumpBuffer = jumpBufferCounter / jumpBufferTime;
            float currentJumpMultipler = jumpMultiplier;

            if (jumpBuffer > 0.35f)
            {
                currentJumpMultipler = jumpMultiplier * (1 - jumpBuffer);
            }

            rb.velocity += vecGravity * currentJumpMultipler * Time.deltaTime;
            currentJumpMultiplier_ReadOnly = currentJumpMultipler; // No Function Just For Testing

            //if (jumpBufferCounter > 0 && jumpBufferCounter <= jumpBufferTime)
            //{
            //    GetComponent<SpriteRenderer>().color = Color.green; // Highlight character
            //}
            //else
            //{
            //    GetComponent<SpriteRenderer>().color = Color.white; // Default color
            //}
        }
        #endregion

        // Faster Fall
        if (rb.velocity.y < 0)
        {
            rb.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
            
            // Clamped Fall Speed
            if (rb.velocity.y < fallClampSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, fallClampSpeed);
            }

        }

        animator.SetFloat(AnimationsString.yVelocity, rb.velocity.y);

        // Roll Movement
        if (IsRolling)
        {
            rollTimer -= Time.deltaTime;
            rb.velocity = new Vector2(transform.localScale.x * rollSpeed, rb.velocity.y);
            damageable.IsInvincible = true;

            if (rollTimer <= 0f)
            {
                IsRolling = false;
            }
        }

        airJumpCounter_ReadOnly = airJumpCounter; // No Function Just For Testing
        rollCooldownTimer_ReadOnly = rollCooldownTimer; // No Function Just For Testing
        rollTimer_ReadOnly = rollTimer; // No Function Just For Testing

        // Cast Spell
        OnCastSpell();

        // Death State
        if (damageable.IsAlive == false)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    public void Movement()
    {

        // Calculate the direction we want to move in and our desired velocity
        float targetSpeed = moveInput.x * currentMoveSpeed;
        // Calculate the difference between the current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;
        // Change acceleration rate depending on situation
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        // Applies acceleration to speed difference, then raise to a set power so acceleration increase with higher speeds
        // Finally multiplies by sign to reapply direction
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        // Applies force to rigidbody, multiplying by Vector2.right so that is only affect the X axis
        rb.AddForce(movement * Vector2.right);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !isFacingRight)
        {
            // Face the right
            isFacingRight = true;
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            // Face the left
            isFacingRight = false;
        }
    }

    //public void OnRun(InputAction.CallbackContext context)
    //{
    //    if (context.started)
    //    {
    //        IsRunning = true;
    //    }
    //    else if (context.canceled)
    //    {
    //        IsRunning = false;
    //    }
    //}

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && CanMove && coyoteTimeCounter > 0f)
        {
            audioSource.PlayOneShot(jumpSound);
            animator.SetTrigger(AnimationsString.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            IsJumping = true;
            jumpBufferCounter = 0f;
        }

        // Double Jump
        if (context.started && !touchingDirections.IsGrounded && airJumpCounter < maxAirJumps && unlocked_DoubleJump)
        {
            audioSource.PlayOneShot(doubleJumpSound);
            animator.SetTrigger(AnimationsString.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            IsJumping = true;
            airJumpCounter++;
        }

        // Variable Jump Height
        else if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpHeight); // Value can be set from 0 -> velocity * 0.

            IsJumping = false;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    public void UpdateVariables()
    {
        if (touchingDirections.IsGrounded)
        {
            if (!landingSoundPlayed)
            {
                audioSource.PlayOneShot(landingSound);
                landingSoundPlayed = true;
            }

            IsJumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0f;

            if (rollCooldownTimer > 0)
            {
                rollCooldownTimer -= Time.deltaTime;
            }
        }
        else if (!touchingDirections.IsGrounded)
        {
            IsJumping = true;
            coyoteTimeCounter -= Time.deltaTime;
            landingSoundPlayed = false;
        }

        timeSinceCast_9 = Mathf.Max(0f, timeSinceCast_9 - Time.deltaTime); // Never let the value below more than 0
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && !touchingDirections.IsGrounded)
        {
            audioSource.PlayOneShot(attackSound);
            animator.SetTrigger(AnimationsString.airAttackTrigger);
        }
        else if (context.started && touchingDirections.IsGrounded)
        {
            audioSource.PlayOneShot(attackSound);
            animator.SetTrigger(AnimationsString.attackTrigger);
        }
        else
        {

        }
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        audioSource.PlayOneShot(hurtSound);

        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    public void OnHeal(InputAction.CallbackContext context)
    {
        if (unlocked_Healing && timeSinceCast_9 <= 0)
        {
            if (context.started && IsAlive && touchingDirections.IsGrounded)
            {
                audioSource.PlayOneShot(healSound);

                if (damageable.Health > 70 && damageable.Health <= 100 && spellable.Mana >= healCost_1)
                {
                    damageable.Heal(healLevel_1);
                    spellable.UseMana(healCost_1);
                }
                else if (damageable.Health > 40 && damageable.Health < 70 && spellable.Mana >= healCost_2)
                {
                    damageable.Heal(healLevel_2);
                    spellable.UseMana(healCost_2);
                }
                else if (damageable.Health < 30 && spellable.Mana >= healCost_3)
                {
                    damageable.Heal(healLevel_3);
                    spellable.UseMana(healCost_3);
                }
            }

            timeSinceCast_9 = timeCast_9;
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (unlocked_Rolling)
        {
            if (context.started && touchingDirections.IsGrounded && !IsRolling && rollCooldownTimer <= 0f && spellable.Stamina >= rollCost)
            {
                audioSource.PlayOneShot(rollSound);
                animator.SetTrigger(AnimationsString.rollTrigger);
                StartRoll();
                rollCooldownTimer = rollCooldown;
                spellable.UseStamina(rollCost);
            }
        }
    }

    public void StartRoll()
    {
        IsRolling = true;
        rollTimer = rollDuration;
    }
    
    public void OnCastSpell()
    {
        if (Input.GetKey(KeyCode.Alpha1) && spellable.Mana >= spellMana_1 && timeSinceCast_1 <= 0 && unlocked_FireBall)
        {
            audioSource.PlayOneShot(spellCastSound_1);
            spellable.UseMana(spellMana_1);
            animator.SetTrigger(AnimationsString.castTrigger_1);
            GameObject spell_1 = Instantiate(Spell_1, SpellPoint_1.position, Quaternion.identity);

            if (isFacingRight)
            {
                spell_1.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                spell_1.transform.eulerAngles = new Vector2(spell_1.transform.eulerAngles.x, 180);
            }

            #region
            //float angle = -45f;

            //if (isFacingRight)
            //{
            //    spell_1.transform.rotation = Quaternion.Euler(0, 0, angle);
            //}
            //else
            //{
            //    spell_1.transform.rotation = Quaternion.Euler(0, 0, 180 - angle);
            //}
            #endregion

            timeSinceCast_1 = timeCast_1;
            isCasting = true;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast_1 = Mathf.Max(0f, timeSinceCast_1 - Time.deltaTime); // Never let the value below more than 0
            
        }

        if (Input.GetKey(KeyCode.Alpha2) && spellable.Mana >= spellMana_2 && timeSinceCast_2 <= 0 && unlocked_WindSlash)
        {
            audioSource.PlayOneShot(spellCastSound_2);
            spellable.UseMana(spellMana_2);
            animator.SetTrigger(AnimationsString.castTrigger_2);
            GameObject spell_2 = Instantiate(Spell_2, SpellPoint_2.position, Quaternion.identity);

            if (isFacingRight)
            {
                spell_2.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                spell_2.transform.eulerAngles = new Vector2(spell_2.transform.eulerAngles.x, 180);
            }

            timeSinceCast_2 = timeCast_2;
            isCasting = true;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast_2 = Mathf.Max(0f, timeSinceCast_2 - Time.deltaTime); // Never let the value below more than 0
        }

        if (Input.GetKey(KeyCode.Alpha3) && spellable.Mana >= spellMana_3 && timeSinceCast_3 <= 0 && touchingDirections.IsGrounded && unlocked_EarthBump)
        {
            audioSource.PlayOneShot(spellCastSound_3);
            spellable.UseMana(spellMana_3);
            animator.SetTrigger(AnimationsString.castTrigger_3);
            
            Vector3 summonOffset = isFacingRight ? new Vector3(summonDistance, 0f, 0f) : new Vector3(-summonDistance, 0f, 0f);
            Vector3 summonPosition = SpellPoint_3.position + summonOffset;
            GameObject spell_3 = Instantiate(Spell_3, summonPosition, Quaternion.identity);

            if (isFacingRight)
            {
                spell_3.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                spell_3.transform.eulerAngles = new Vector2(spell_3.transform.eulerAngles.x, 180);
            }

            timeSinceCast_3 = timeCast_3;
            isCasting = true;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast_3 = Mathf.Max(0f, timeSinceCast_3 - Time.deltaTime); // Never let the value below more than 0
        }

        if (Input.GetKey(KeyCode.Alpha4) && spellable.Mana >= spellMana_4 && timeSinceCast_4 <= 0 && touchingDirections.IsGrounded && unlocked_WaterTornado)
        {
            audioSource.PlayOneShot(spellCastSound_4);
            spellable.UseMana(spellMana_4);
            animator.SetTrigger(AnimationsString.castTrigger_1);
            GameObject spell_4 = Instantiate(Spell_4, SpellPoint_4.position, Quaternion.identity);

            if (isFacingRight)
            {
                spell_4.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                spell_4.transform.eulerAngles = new Vector2(spell_4.transform.eulerAngles.x, 180);
            }

            timeSinceCast_4 = timeCast_4;
            isCasting = true;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast_4 = Mathf.Max(0f, timeSinceCast_4 - Time.deltaTime); // Never let the value below more than 0
        }
    }

    public IEnumerator CastCoroutine()
    {
        yield return new WaitForSeconds(0.35f);
        isCasting = false;
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        // If exit direction is upwards
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        // If exit direction requires horizontal movement
        if (_exitDir.x != 0)
        {
            moveInput = new Vector2(_exitDir.x > 0 ? 1 : -1, 0);
            Movement();
        }

        //SetFacingDirection(moveInput);
        yield return new WaitForSeconds(_delay);
        cutscene = false;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded)
        {

        }
    }

    public void Respawned()
    {
        if (!damageable.IsAlive)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            GetComponent<CapsuleCollider2D>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = true;

            damageable.IsAlive = true;
            damageable.Health = damageable.MaxHealth;
            animator.Play("Player_idle");
        }
    }

    public void OnCheatMode(InputAction.CallbackContext context)
    {
        if (context.started && IsAlive)
        {
            ToggleCheatMode();
        }
    }

    public void ToggleCheatMode()
    {
        if (!isCheatMode)
        {
            moveSpeed = 15f;
            airSpeed = 15f;
            jumpForce = 45f;
            rollSpeed = 18f;
            damageable.Health = 100f;
            spellable.Mana = 100f;
            spellable.Stamina = 100f;
            damageable.invincibilityTime = 999f;

            rollCost = 0f;
            healCost_1 = 0f;
            spellMana_1 = 0f;
            spellMana_2 = 0f;
            spellMana_3 = 0f;
            spellMana_4 = 0f;
            timeCast_1 = 0.5f;
            timeCast_2 = 0.5f;
            timeCast_3 = 0.5f;
            timeCast_4 = 0.5f;
            timeCast_9 = 0.5f;

            damageable.IsInvincible = true;
            unlocked_DoubleJump = true;
            unlocked_Healing = true;
            unlocked_Rolling = true;
            unlocked_FireBall = true;
            unlocked_WindSlash = true;
            unlocked_EarthBump = true;
            unlocked_WaterTornado = true;
            unlocked_Key_1 = true;
            unlocked_Key_2 = true;
            unlocked_Key_3 = true;
            unlocked_Key_4 = true;
            unlocked_Key_5 = true;

            GetComponent<SpriteRenderer>().color = Color.yellow;
            isCheatMode = true;
        }
        else if (isCheatMode)
        {
            moveSpeed = 9f;
            airSpeed = 9f;
            jumpForce = 30f;
            rollSpeed = 12f;
            damageable.invincibilityTime = 0.5f;
            damageable.IsInvincible = false;

            rollCost = 20f;
            healCost_1 = 20f;
            spellMana_1 = 5f;
            spellMana_2 = 10f;
            spellMana_3 = 15f;
            spellMana_4 = 20f;
            timeCast_1 = 3f;
            timeCast_2 = 5f;
            timeCast_3 = 7f;
            timeCast_4 = 9f;
            timeCast_9 = 1f;

            GetComponent<SpriteRenderer>().color = Color.white;
            isCheatMode = false;
        }
    }
}
