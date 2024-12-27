using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Slider slider;

    Damageable damageable;

    private void Awake()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        damageable = boss.GetComponent<Damageable>();

        if (boss == null)
        {
            Debug.Log("No player found ! ##ErrorTagPlayer");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private float CalculateSliderPercentage(float currentHealth, float maxHealth)
    {
        return currentHealth / maxHealth;
    }

    private void OnEnable()
    {
        damageable.healthChanged.AddListener(OnBossHealthChanged);
    }

    private void OnDisable()
    {
        damageable.healthChanged.RemoveListener(OnBossHealthChanged);
    }

    private void OnBossHealthChanged(float newHealth, float maxHealth)
    {
        slider.value = CalculateSliderPercentage(newHealth, maxHealth);
    }
}
