using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class The_Guider : MonoBehaviour
{
    public Dialogue dialogue;
    public bool playerDetected;
    Animator animator;

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
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDetected && Input.GetKeyDown(KeyCode.E))
        {
            dialogue.StartDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetected = true;
            dialogue.ToggleInteract(playerDetected);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetected = false;
            dialogue.ToggleInteract(playerDetected);
            dialogue.EndDialogue();
        }
    }
}
