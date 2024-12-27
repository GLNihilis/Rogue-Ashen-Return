using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEffector : MonoBehaviour
{
    [SerializeField] private bool IsOnPlatform = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.S) && IsOnPlatform == true)
        {
            transform.GetComponent<PlatformEffector2D>().rotationalOffset = 180;
        }
        else
        {
            transform.GetComponent<PlatformEffector2D>().rotationalOffset = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            IsOnPlatform = true;
        }
    }
}
