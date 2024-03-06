using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMeleeAbility : BaseAbility
{
    private List<Collider2D> hitboxes = new List<Collider2D>();
    [SerializeField] private int numHitboxes;
    [SerializeField] private float hitTime = 1;
    [SerializeField] public int damage = 1; // will likely need to add invincibility frames to avoid multihits bc of overlapping hitboxes


    // scriptable object start()
    void Start()
    {
        for (int i = 0; i < numHitboxes; i++)
        {
            Transform hitbox = transform.Find("hitbox_" + i); // get hitbox step
            hitboxes.Add(hitbox.gameObject.GetComponent<Collider2D>()); // add to hitbox list
            // hitbox.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    public override void UseAbility()
    {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        float hitboxInterval = hitboxes.Count/numHitboxes;

        for (int i = 0;i < numHitboxes;i++) 
        {
            //hitboxes[i].gameObject.SetActive(true);
            // start hitbox fadeout sequence
            StartCoroutine(hitboxes[i].GetComponent<Hitbox>().HitboxTrigger());
            yield return new WaitForSeconds(hitboxInterval);
        }
        

    } 
}
