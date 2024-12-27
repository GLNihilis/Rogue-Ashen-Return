using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent noCollidersRemain;

    [Header("Settings")]
    public List<Collider2D> dectectedColliders = new List<Collider2D>();
    public List<Collider2D> dectectedEnemyColliders = new List<Collider2D>();
    Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            dectectedEnemyColliders.Add(collision);
        }

        dectectedColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            dectectedEnemyColliders.Remove(collision);
        }

        dectectedColliders.Remove(collision);

        if (dectectedColliders.Count <= 0)
        {
            noCollidersRemain.Invoke();
        }
    }
}
