using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBump : MonoBehaviour
{
    [Header("Spell Settings")]
    [SerializeField] private float damage = 12f;
    [SerializeField] private float lifeTime = 1.2f;
    [SerializeField] private Vector2 knockBack = Vector2.zero;
    

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);

            bool spellHit = damageable.Hit(damage, knockBack);

            if (spellHit)
            {
                Debug.Log(collision.name + " got hit for " + damage);
            }
        }
    }
}
