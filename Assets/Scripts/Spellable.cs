using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spellable : MonoBehaviour
{
    [Header("Mana Settings")]
    [SerializeField] private float mana_RegenRate = 5f;
    [SerializeField] private float mana_RegenDelay = 2f;
    private Coroutine mana_RegenCoroutine;
    public UnityEvent<float, float> manaChanged;

    [Header("Stamina Settings")]
    [SerializeField] private float stamina_RegenRate = 7f;
    [SerializeField] private float stamina_RegenDelay = 2f;
    private Coroutine stamina_RegenCoroutine;
    public UnityEvent<float, float> staminaChanged;

    [SerializeField] private float _maxMana = 100;
    public float MaxMana
    {
        get
        {
            return _maxMana;
        }
        set
        {
            _maxMana = value;
        }
    }

    [SerializeField] private float _mana = 100;
    public float Mana
    {
        get
        {
            return _mana;
        }
        set
        {
            _mana = value;
            
        }
    }

    [SerializeField] private float _maxStamina = 100;
    public float MaxStamina
    {
        get
        {
            return _maxStamina;
        }
        set
        {
            _maxStamina = value;
        }
    }

    [SerializeField] private float _stamina = 100;
    public float Stamina
    {
        get
        {
            return _stamina;
        }
        set
        {
            _stamina = value;

        }
    }

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool UseMana(float mana)
    {
        if (_mana >= mana)
        {
            _mana -= mana;
            manaChanged?.Invoke(_mana, MaxMana);
            StopManaRegen();
            Invoke(nameof(StartManaRegen), mana_RegenDelay);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartManaRegen()
    {
        if (mana_RegenCoroutine == null)
        {
            mana_RegenCoroutine = StartCoroutine(RegenMana());
        }
    }

    public void StopManaRegen()
    {
        if (mana_RegenCoroutine != null)
        {
            StopCoroutine(mana_RegenCoroutine);
            mana_RegenCoroutine = null;
        }
    }

    public IEnumerator RegenMana()
    {
        while (_mana < MaxMana)
        {
            _mana = Mathf.Min(_mana + mana_RegenRate * Time.deltaTime, MaxMana);
            manaChanged?.Invoke(_mana, MaxMana);
            yield return null;
        }

        mana_RegenCoroutine = null;
    }

    public bool UseStamina(float stamina)
    {
        if (_stamina >= stamina)
        {
            _stamina -= stamina;
            staminaChanged?.Invoke(_stamina, MaxStamina);
            StopStaminaRegen();
            Invoke(nameof(StartStaminaRegen), stamina_RegenDelay);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartStaminaRegen()
    {
        if (stamina_RegenCoroutine == null)
        {
            stamina_RegenCoroutine = StartCoroutine(RegenStamina());
        }
    }

    public void StopStaminaRegen()
    {
        if (stamina_RegenCoroutine != null)
        {
            StopCoroutine(stamina_RegenCoroutine);
            stamina_RegenCoroutine = null;
        }
    }

    public IEnumerator RegenStamina()
    {
        while (_stamina < MaxStamina)
        {
            _stamina = Mathf.Min(_stamina + stamina_RegenRate * Time.deltaTime, MaxStamina);
            staminaChanged?.Invoke(_stamina, MaxStamina);
            yield return null;
        }

        stamina_RegenCoroutine = null;
    }
}
