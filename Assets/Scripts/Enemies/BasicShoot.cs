using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Redcode.Pools;

// This enemy needs to be given a target when it is created with set target
// It will then move towards that target until it reaches fire range, at which point it will stop and begin firing at the target
// Should the target move within run range units, it will run away

public class BasicShoot : Enemy
{
    NavMeshAgent agent;
    public float viewDistance = 30;
    public float runRange = 5;
    public float fireRange = 20;
    public float firerate = 0.5f;
    public float force = 100f;
    public float bulletLifespan = 5f;
    protected bool readyToFire;
    // Used to check if the enemy is currently trying to circle around the player
    protected bool circiling;
    protected int circelingDirection;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // Code from https://www.youtube.com/watch?v=HRX0pUSucW4
        agent = GetComponent<NavMeshAgent>();
        // Sets up agent to be in 2d
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        // external code ends
        readyToFire = true;
        circiling = false;
        StartCoroutine(UpdateTarget());
    }

    // Only change directions once every 0.1 seconds, just to save processing power
    // Realistically, this is a state machine, but with a whopping total of 3 (three) states, I couldn't be bothered to make a real fsm
    protected IEnumerator UpdateTarget()
    {
        while (true)
        {
            Vector2 circilingVector = transform.position;
            Vector2 pos = transform.position;
            Vector3 playerPos = enemyManager.GetPlayerPosition();
            // Draw a line between the target and this enemy
            Vector2 line = playerPos - transform.position;
            // Draw a line from here to the target
            RaycastHit2D hit = Physics2D.Raycast(transform.position, line, viewDistance);
            // If it intercepts the target, update the destination
            if(hit.collider && enemyManager.CheckIfPlayer(hit.collider.gameObject))
            {
                // If the target is outside the fireRange, move towards it
                if (line.magnitude > fireRange)
                {
                    agent.SetDestination(playerPos);
                    circiling = false;
                }
                // If it is inside the run range, run away and fire
                else if (line.magnitude < runRange)
                {
                    agent.SetDestination(pos-line);
                    Fire();
                    circiling = false;
                }else
                // If it is between the fire and run range, circle around the player
                {
                    // When the enemy starts ciricling the player, decide if it will circle clockwise or counterclockwise
                    // All of the other options will reset this bool to false
                    // This stops the enemy from just sort of getting stuck and wibbling side to side or always circiling the same direction, which feels bad
                    if (!circiling)
                    {
                        circiling = true;
                        circelingDirection = Random.Range(-1, 1) < 0 ? -1 : 1;
                    }
                    circilingVector = line.Perpendicular1() * circelingDirection;
                    float x = Random.Range(-1, 1);
                    float y = Random.Range(-1, 1);
                    Vector2 offset = new Vector2(x, y);
                    circilingVector += offset;
                    agent.SetDestination(pos + circilingVector);
                    Fire();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // wait for firerate seconds before being able to shoot again
    protected IEnumerator FireCooldown()
    {
        yield return new WaitForSeconds(firerate);
        readyToFire = true;
    }

    public virtual void Fire()
    {
        // If the enemy is ready to fire, do so
        if (readyToFire)
        {
            readyToFire = false;
            StartCoroutine(FireCooldown());
            Bullet newBullet = enemyManager.GetBullet(transform.position, transform.rotation);
            newBullet.SetTarget(enemyManager.GetPlayerPosition(), force, bulletLifespan, this.gameObject);
        }
        return;
    }
}
