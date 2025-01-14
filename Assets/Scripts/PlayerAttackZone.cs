using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackZone : MonoBehaviour
{
    public float attackDamage = 10f;
    public Vector2 knockBack = Vector2.zero;
    public float attackDamageCheatMode = 100f;
    public Vector2 knockBackCheatMode = Vector2.zero;

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

        if (damageable != null && !PlayerController.Instance.isCheatMode)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);

            // Hit the target
            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);

            if (gotHit)
            {
                Debug.Log(collision.name + " got hit for " + attackDamage);
            }
        }
        else if (damageable != null && PlayerController.Instance.isCheatMode)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockBackCheatMode : new Vector2(-knockBackCheatMode.x, knockBackCheatMode.y);

            // Hit the target
            bool gotHit = damageable.Hit(attackDamageCheatMode, deliveredKnockback);

            if (gotHit)
            {
                Debug.Log(collision.name + " got hit for " + attackDamage);
            }
        }
    }
}
