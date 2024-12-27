using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private float damage;
    public Vector2 knockBack = Vector2.zero;

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
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null && collision.tag == "Player")
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

            Vector2 deliveredKnockback = new Vector2(knockbackDirection.x * knockBack.x, knockBack.y);

            // Hit the target
            bool gotHit = damageable.Hit(damage, deliveredKnockback);

            if (gotHit)
            {
                Debug.Log(collision.name + " got hit for " + damage + " by " + gameObject);
            }
        }
    }
}
