using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Redcode.Pools;
using UnityEditor.Rendering;

//This enemy will approach the player, almost stop and flash blue for a moment, then ram them
public class RammingEnemy : Enemy
{
    NavMeshAgent agent;
    public float viewDistance = 30;
    public float runRange = 5;
    public float fireRange = 20;
    public float firerate = 0.5f;
    public float windup = 0.5f;
    public float windupSpeed = 1;
    public float ramSpeed = 20;
    public float ramAceleration = 100;
    public float ramDistance = 10;
    public float numberOfBlinks = 4;
    protected bool readyToFire;
    bool currentlyRamming;
    Color oldColor;

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
        // I do this here just so if a windup starts when the enemy is taking damage, it won't overwrite the enemy's color with red
        oldColor = gameObject.GetComponent<SpriteRenderer>().color;
        currentlyRamming = false;
        StartCoroutine(UpdateTarget());
    }

    // Only change directions once every 0.1 seconds, just to save processing power
    // Realistically, this is a state machine, but with a whopping total of 3 (three) states, I couldn't be bothered to make a real fsm
    protected IEnumerator UpdateTarget()
    {
        while (true)
        {
            Vector3 playerPos = enemyManager.GetPlayerPosition();
            // Draw a line between the target and this enemy
            Vector2 line = playerPos - transform.position;
            // Draw a line from here to the target
            RaycastHit2D hit = Physics2D.Raycast(transform.position, line, viewDistance);
            // If it intercepts the target, update the destination
            // Don't update the target if we're currently ramming, as that will cause the ram destination to be overwritten
            if (hit.collider && enemyManager.CheckIfPlayer(hit.collider.gameObject) && !currentlyRamming)
            {
                // Move towards the player
                agent.SetDestination(playerPos);
                // If the target is insaide the fireRange, ram em
                if (line.magnitude < fireRange)
                {
                    StartCoroutine(Fire());
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

    public IEnumerator Fire()
    {
        // If the enemy is ready to ram, do so
        if (readyToFire)
        {
            readyToFire = false;
            currentlyRamming = false;
            // To begin, have the enemy start winding up its ram
            float oldSpeed = agent.speed;
            // slow the enemy way down
            agent.speed = windupSpeed;
            // Blink the sprite blue as it winds up
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            for (int i = 0; i < numberOfBlinks - 1; i++)
            {
                renderer.color = Color.blue;
                yield return new WaitForSeconds(windup / numberOfBlinks);
                renderer.color = oldColor;
                yield return new WaitForSeconds(windup / numberOfBlinks);
            }
            // Set the target to a bit past where the player is right now, and stop it from changing further
            Vector2 playerPos = enemyManager.GetPlayerPosition();
            Vector2 loc = transform.position;
            // Draw a line between the target and this enemy
            Vector2 line = playerPos - loc;
            // Normalize that line, and use it to set the real target to past the player
            currentlyRamming = true;
            agent.SetDestination(playerPos + (line.normalized * ramDistance));
            // Give another quick blink
            renderer.color = Color.blue;
            yield return new WaitForSeconds(windup / numberOfBlinks);
            renderer.color = oldColor;
            yield return new WaitForSeconds(windup / numberOfBlinks);

            float oldAccel = agent.acceleration;
            // Test if the player is still in sight, ram if they are
            RaycastHit2D hit = Physics2D.Raycast(transform.position, enemyManager.GetPlayerPosition() - transform.position);
            if (hit.collider && enemyManager.CheckIfPlayer(hit.collider.gameObject))
            {
                agent.speed = ramSpeed;
                agent.acceleration = ramAceleration;
                // Wait until the ram is complete
                while (agent.remainingDistance >= 0.2f)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            // Then reset the values
            agent.speed = oldSpeed;
            agent.acceleration = oldAccel;
            currentlyRamming = false;
            StartCoroutine(FireCooldown());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemyManager.DamageObject(collision.gameObject);
    }
}
