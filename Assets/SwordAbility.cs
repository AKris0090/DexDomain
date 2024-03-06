using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAbility : MonoBehaviour
{
    private List<Collider2D> hitboxes = new List<Collider2D>();
    [SerializeField] private int numHitboxes;

    // scriptable object start()
    void Start()
    {
        for (int i = 0; i < numHitboxes; i++)
        {
            Transform child = transform.Find("hitbox_" + i);

        }
    }

    // Start is called before the first frame update
    public void use()
    {

    }
}
