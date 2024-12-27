using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image gradientImageBar;

    public const float MAX_FADE = 1f;
    public Image damagedBarImage;
    public Color damagedColor;
    private float fadeTimer;

    Damageable damageable;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        damageable = player.GetComponent<Damageable>();

        if (player == null)
        {
            Debug.Log("No player found ! ##ErrorTagPlayer");
        }

        damagedColor = damagedBarImage.color;
        damagedColor.a = 0;
        damagedBarImage.color = damagedColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (damagedColor.a > 0)
        {
            fadeTimer -= Time.deltaTime;
            if (fadeTimer < 0)
            {
                float fadeAmount = 5f;
                damagedColor.a -= fadeAmount * Time.deltaTime;
                damagedBarImage.color = damagedColor;
            }
        }
    }

    private float CalculateSliderPercentage(float currentHealth, float maxHealth)
    {
        return currentHealth / maxHealth;
    }

    private void OnEnable()
    {
        damageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnDisable()
    {
        damageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    private void OnPlayerHealthChanged(float newHealth, float maxHealth)
    {
        slider.value = CalculateSliderPercentage(newHealth, maxHealth);

        gradientImageBar.color = gradient.Evaluate(1f);
        gradientImageBar.color = gradient.Evaluate(slider.normalizedValue);

        if (damagedColor.a < 0)
        {
            damagedBarImage.fillAmount = gradientImageBar.fillAmount;
        }
        damagedColor.a = 1;
        damagedBarImage.color = damagedColor;
        fadeTimer = MAX_FADE;

    }
}
