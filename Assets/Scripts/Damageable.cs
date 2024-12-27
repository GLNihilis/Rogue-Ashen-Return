using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<float, Vector2> damageableHit;
    public UnityEvent<float, float> healthChanged;

    Animator animator;

    [SerializeField] private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    [SerializeField] private bool _isInvincible = false;
    public bool IsInvincible
    {
        get
        {
            return _isInvincible;
        }
        set
        {
            _isInvincible = value;
        }
    }

    [SerializeField] private float _maxHealth = 100;
    public float MaxHealth {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
            
        }}

    [SerializeField] private float _health = 100;
    public float Health {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);

            // If health drops below 0, character is no longer alive
            if (_health <= 0)
            {
                _health = 0;
                IsAlive = false;

                if (PlayerController.Instance.IsAlive == false)
                {
                    StartCoroutine(Death());
                }
            }
        }}

    [SerializeField] private bool _isAlive = true;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationsString.isAlive, value);
            Debug.Log("IsAlive set " + value);
        }
    }

    // The velocity should not be change while this is true but needs to be respected by the others physics components like
    // The Player Controller
    public bool lockVelocity
    {
        get
        {
            return animator.GetBool(AnimationsString.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationsString.lockVelocity, value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInvincible)
        {
            if (timeSinceHit > invincibilityTime) 
            {
                // Remove Invincibility
                IsInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }
    
    // Returns whether the damageable took damage or not
    public bool Hit(float damage, Vector2 knockBack)
    {
        if (IsAlive && !IsInvincible)
        {
            // Able to be Hit
            Health -= damage;
            IsInvincible = true;

            // Notify other subscribed components that the damageable was hit to handle the knockback
            animator.SetTrigger(AnimationsString.hitTrigger);
            lockVelocity = true;
            damageableHit?.Invoke(damage, knockBack);
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);

            return true;
        }
        else
        {
            // Unable to be hit
            return false;
        }
    }

    public bool Heal(float healthRestored)
    {
        if (IsAlive)
        {
            // Able to be heal
            float currentHealth = MaxHealth - Health; // Otherwise: Mathf.Max(MaxHealth - Health, 0);
            float actualHeal = Mathf.Min(currentHealth, healthRestored);
            Health += actualHeal;
            

            animator.SetTrigger(AnimationsString.healTrigger);
            CharacterEvents.characterHealed.Invoke(gameObject, healthRestored);
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator Death()
    {
        yield return new WaitForSeconds(0.9f);
        StartCoroutine(CanvasController.Instance.ActiveDeathScreen());
    }
}
