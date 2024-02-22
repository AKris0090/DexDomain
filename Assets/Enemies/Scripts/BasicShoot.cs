using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicShoot : MonoBehaviour
{
    [SerializeField] Transform target;
    NavMeshAgent agent;
    public GameObject bullet;
    public float viewDistance = 30;
    public float runRange = 5;
    public float fireRange = 20;
    public int health = 1;
    public float firerate = 0.5f;
    public float force = 100f;
    public float bulletLifespan = 5f;
    bool readyToFire;
    // Start is called before the first frame update
    void Start()
    {
        // Code from https://www.youtube.com/watch?v=HRX0pUSucW4
        agent = GetComponent<NavMeshAgent>();
        // Sets up agent to be in 2d
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        // external code ends
        StartCoroutine(UpdateTarget());
        readyToFire = true;
    }

    // Only change directions once every 0.1 seconds, just to save processing power
    IEnumerator UpdateTarget()
    {
        while (true)
        {
            // Draw a line between the target and this enemy
            Vector2 line = target.position - transform.position;
            // Draw a line from here to the target
            RaycastHit2D hit = Physics2D.Raycast(transform.position, line, viewDistance);
            // If it intercepts the target, update the destination
            if(hit.collider && hit.collider.gameObject.transform == target)
            {
                // If the target is outside the fireRange, move towards it
                if (line.magnitude > fireRange)
                {
                    agent.SetDestination(target.position); 
                    // If it is inside the run range, run away and fire
                }
                else if (line.magnitude < runRange)
                {
                    agent.SetDestination(-line);
                    Fire();
                }else
                // If it is between the fire and run range, stay in place
                // TODO: make it move around randomly a bit, to make things more interesting for the player
                {
                    agent.SetDestination(transform.position);
                    Fire();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // wait for firerate seconds before being able to shoot again
    IEnumerator FireCooldown()
    {
        yield return new WaitForSeconds(firerate);
        readyToFire = true;
    }

    public void Fire()
    {
        // If the enemy is ready to fire, do so
        if (readyToFire)
        {
            readyToFire = false;
            StartCoroutine(FireCooldown());
            GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);
            newBullet.GetComponent<Bullet>().SetTarget(target, force, bulletLifespan);
        }
        return;
    }

    // Change the basic shooter's target to loc
    public void setTarget(Transform loc)
    {
        target = loc;
    }

    // Take damage
    public void Damage()
    {
        health--;
        Debug.Log("Damage delt to enemy");
        if(health == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
