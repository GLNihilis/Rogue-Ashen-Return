using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    public bool playerDetected;
    public GameObject interact;

    // Start is called before the first frame update
    void Start()
    {
        ToggleInteract(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetected = true;
            ToggleInteract(playerDetected);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetected = false;
            ToggleInteract(playerDetected);
        }
    }

    public void ToggleInteract(bool show)
    {
        interact.SetActive(show);
    }
}
