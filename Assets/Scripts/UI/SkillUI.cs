using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    public Slider slider_1;
    public Slider slider_2;
    public Slider slider_3;
    public Slider slider_4;

    public Image image_1;
    public Image image_2;
    public Image image_3;
    public Image image_4;

    [SerializeField] private Image imageCooldown_1;
    [SerializeField] private Image imageCooldown_2;
    [SerializeField] private Image imageCooldown_3;
    [SerializeField] private Image imageCooldown_4;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("No player found ! ##ErrorTagPlayer");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.Instance.unlocked_FireBall)
        {
            image_1.enabled = true;
        }
        else
        {
            image_1.enabled = false;
        }

        if (PlayerController.Instance.unlocked_WindSlash)
        {
            image_2.enabled = true;
        }
        else
        {
            image_2.enabled = false;
        }

        if (PlayerController.Instance.unlocked_EarthBump)
        {
            image_3.enabled = true;
        }
        else
        {
            image_3.enabled = false;
        }

        if (PlayerController.Instance.unlocked_WaterTornado)
        {
            image_4.enabled = true;
        }
        else
        {
            image_4.enabled = false;
        }

        if (PlayerController.Instance.timeSinceCast_1 < PlayerController.Instance.timeCast_1)
        {

            float cooldownProgress_1 = PlayerController.Instance.timeSinceCast_1 / PlayerController.Instance.timeCast_1;
            slider_1.value = cooldownProgress_1;
        }
        else
        {
            slider_1.value = 0f;   
        }

        if (PlayerController.Instance.timeSinceCast_2 < PlayerController.Instance.timeCast_2)
        {

            float cooldownProgress_2 = PlayerController.Instance.timeSinceCast_2 / PlayerController.Instance.timeCast_2;
            slider_2.value = cooldownProgress_2;
        }
        else
        {
            slider_2.value = 0f;
        }

        if (PlayerController.Instance.timeSinceCast_3 < PlayerController.Instance.timeCast_3)
        {

            float cooldownProgress_3 = PlayerController.Instance.timeSinceCast_3 / PlayerController.Instance.timeCast_3;
            slider_3.value = cooldownProgress_3;
        }
        else
        {
            slider_3.value = 0f;
        }

        if (PlayerController.Instance.timeSinceCast_4 < PlayerController.Instance.timeCast_4)
        {

            float cooldownProgress_4 = PlayerController.Instance.timeSinceCast_4 / PlayerController.Instance.timeCast_4;
            slider_4.value = cooldownProgress_4;
        }
        else
        {
            slider_4.value = 0f;
        }
    }
}
