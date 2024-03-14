using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDealDamage : MonoBehaviour
{
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Bullet>() == null)
        {
            EnemyManager.Instance.DamageEnemy(collision.gameObject, (damage * CharacterMovement._cmInstance.dmgMod));
            return;
        }
    }
}
