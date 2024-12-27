using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    public Slider slider;

    //public const float MAX_FADE = 1f;
    //public Image castBarImage;
    //public Color castColor;
    //private float fadeTimer;

    Spellable spellable;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        spellable = player.GetComponent<Spellable>();

        if (player == null)
        {
            Debug.Log("No player found ! ##ErrorTagPlayer");
        }

        //castColor = castBarImage.color;
        //castColor.a = 0;
        //castBarImage.color = castColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (castColor.a > 0)
        //{
        //    fadeTimer -= Time.deltaTime;
        //    if (fadeTimer < 0)
        //    {
        //        float fadeAmount = 5f;
        //        castColor.a -= fadeAmount * Time.deltaTime;
        //        castBarImage.color = castColor;
        //    }
        //}
    }

    private float CalculateSliderPercentage(float currentMana, float maxMana)
    {
        return currentMana / maxMana;
    }

    private void OnEnable()
    {
        spellable.manaChanged.AddListener(OnPlayerManaChanged);
    }

    private void OnDisable()
    {
        spellable.manaChanged.RemoveListener(OnPlayerManaChanged);
    }

    private void OnPlayerManaChanged(float newMana, float maxMana)
    {
        slider.value = CalculateSliderPercentage(newMana, maxMana);

        //if (castColor.a < 0)
        //{
        //    castBarImage.fillAmount = slider.value;
        //}
        //castColor.a = 1;
        //castBarImage.color = castColor;
        //fadeTimer = MAX_FADE;
    }
}
