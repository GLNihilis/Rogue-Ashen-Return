using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIAudio : MonoBehaviour
{
    [SerializeField] AudioClip hover, click;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SoundOnHover()
    {
        audioSource.PlayOneShot(hover);
    }

    public void SoundOnClick()
    {
        audioSource?.PlayOneShot(click);
    }
}
