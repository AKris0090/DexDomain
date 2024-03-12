using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static RoomData;

public class Boss : Enemy
{
    BossState state;
    BossState idle;
    BossState bulletHell;
    BossState dash;
    BossState shotgun;
    bool phaseChanged;
    public Transform dashLocation1;
    public Transform dashLocation2;
    public Transform dashLocation3;
    public Transform dashLocation4;
    public Transform dashLocation5;
    List<Transform> dashLocations;
    public int dashSpeed = 30;
    public int numberOfDashes = 6;
    public float dashFirerate = 0.3f;
    public float dashForce = 100f;
    List<BossState> states;
    public int numOfBulletHellRings = 4;
    public int bulletsInRings = 20;
    public float ringBulletForce = 100;
    public float bulletLifespan = 5;
    public float secondsBetweenRings = 1;
    float phase;
    int maxHeatlth;
    NavMeshAgent agent;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        state = idle;
        phase = 1f;
        phaseChanged = false;
        maxHeatlth = health;
        state = new Idle();
        idle = state;
        bulletHell = new BulletHell();
        dash = new Dash();
        shotgun = new Shotgun();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        states = new List<BossState>();
        states.Add(bulletHell);
        states.Add(dash);
        states.Add(shotgun);

        dashLocations = new List<Transform>();
        dashLocations.Add(dashLocation1);
        dashLocations.Add(dashLocation2);
        dashLocations.Add(dashLocation3);
        dashLocations.Add(dashLocation4);
        dashLocations.Add(dashLocation5);
    }

    // Update is called once per frame
    void Update()
    {
        if (health < maxHeatlth / 2 && !phaseChanged)
        {
            phase += 0.5f;
            phaseChanged = true;
        }
        else
        {
            state.Act(this);
        }
    }

    void SelectNewState()
    {
        return; // TODO: CHANGE THIS WHEN ALL STATES WORK
        // This allows for the boss repeatedly selecting the same state. Should be okay? 
        int selector = Random.Range(0, states.Count);
        state = states[selector];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemyManager.DamageObject(collision.gameObject);
    }

    // Boss state is a state machine, that handles the bosses state
    abstract class BossState
    {
        abstract public void Act(Boss boss);
    }

    // When the boss is idle, it does nothing except wait for the player 
    class Idle : BossState
    {
        bool lookedRecently = false;
        public override void Act(Boss boss)
        {
            if (!lookedRecently)
            {
                lookedRecently = true;
                boss.StartCoroutine(CheckForPlayer(boss));
            }
        }

        IEnumerator CheckForPlayer(Boss boss)
        {
            Vector3 playerPos = boss.enemyManager.GetPlayerPosition();
            // Draw a line between the boss and the player
            Vector2 line = playerPos - boss.transform.position;
            // Draw a line from here to the player
            RaycastHit2D hit = Physics2D.Raycast(boss.transform.position, line);
            // If this line hit the player
            if (boss.enemyManager.CheckIfPlayer(hit.collider.gameObject))
            {
                boss.state = boss.dash;
            }
            yield return new WaitForSeconds(1f);
            lookedRecently = false;
        }
    }

    // In the bullet hell state, the boss will emit rings of bullets, slighly offset, to create something like a bullet hell
    class BulletHell : BossState
    {
        bool attackStarted = false;
        public override void Act(Boss boss)
        {
            if (!attackStarted)
            {
                attackStarted = true;
                boss.StartCoroutine(Attack(boss));
            }
        }

        IEnumerator Attack(Boss boss)
        {
            float ringsToSpawn = boss.numOfBulletHellRings * boss.phase;
            float bulletsToSpawn = boss.bulletsInRings * (boss.phase);
            for (int i = 0; i < ringsToSpawn / 2; i++)
            {
                for(int j = 0; j < bulletsToSpawn; j++)
                {
                    Bullet newBullet = EnemyManager.Instance.GetBullet();
                    float properAngle = ((2*Mathf.PI) / bulletsToSpawn) * j;
                    float x = Mathf.Cos(properAngle);
                    float y = Mathf.Sin(properAngle);
                    Debug.Log("Angle: " + properAngle + " x: " + x + " y: " + y);
                    Vector3 dir = new Vector3(x * 10f, y * 10f, 0);
                    newBullet.SetTarget(dir + boss.transform.position, 5, boss.ringBulletForce, boss.gameObject, boss.transform.position, boss.transform.rotation);
                }
                yield return new WaitForSeconds(boss.secondsBetweenRings);
                for (int j = 0; j < bulletsToSpawn; j++)
                {
                    Bullet newBullet = EnemyManager.Instance.GetBullet();
                    float properAngle = (((2 * Mathf.PI) / bulletsToSpawn) * j) + (((2 * Mathf.PI) / bulletsToSpawn)/2);
                    float x = Mathf.Cos(properAngle);
                    float y = Mathf.Sin(properAngle);
                    Debug.Log("Angle: " + properAngle + " x: " + x + " y: " + y);
                    Vector3 dir = new Vector3(x * 10f, y * 10f, 0);
                    newBullet.SetTarget(dir + boss.transform.position, 5, boss.ringBulletForce * boss.phase, boss.gameObject, boss.transform.position, boss.transform.rotation);
                }
                yield return new WaitForSeconds(boss.secondsBetweenRings);
            }
            yield return new WaitForSeconds(1f);
            attackStarted = false;
            boss.SelectNewState();
        }
    }

    // In the dash state, the boss will dash towards the closest predetermined part of the the room to the player, and produce bullets as it does so
    class Dash : BossState
    {
        bool attackStarted = false;
        public override void Act(Boss boss)
        {
            if (!attackStarted)
            {
                attackStarted = true;
                boss.StartCoroutine(Attack(boss));
            }
        }
        
        IEnumerator Attack(Boss boss)
        {
            NavMeshAgent agent = boss.agent;
            Vector3 currentLocation = Vector3.zero;
            for(int i = 0; i < boss.numberOfDashes; i++)
            {
                float smallestDistanceToPlayer = float.MaxValue;
                Vector3 nextLocation = Vector3.zero;
                // Because of ^, if a point is placed at (0, 0, 0), the boss will never go to it. Hopefully this never becomes relevent
                // Find the position closest to the player
                foreach (Transform loc in boss.dashLocations)
                {
                    Vector3 playerPos = boss.enemyManager.GetPlayerPosition();
                    // Draw a line between the dash location and the player
                    Vector2 line = playerPos - loc.position;
                    // Update the closest position if closer to the player, ignoring it if the boss is currently there
                    if(line.magnitude < smallestDistanceToPlayer && currentLocation != loc.position)
                    {
                        smallestDistanceToPlayer = line.magnitude;
                        nextLocation = loc.position;
                    }
                }
                // Now that the closest point has been found, fire the boss at it
                Debug.Log(nextLocation);
                agent.SetDestination(nextLocation);
                agent.speed = boss.dashSpeed * boss.phase;
                agent.acceleration = 200;
                // Wait until the boss is sutibly close to the end of the dash, firing bullets as it goes
                while ((boss.transform.position - nextLocation).magnitude >= 1)
                {
                    Vector2 line = (boss.transform.position - nextLocation);
                    Vector2 bossLoc = boss.transform.position;
                    Bullet newBullet = EnemyManager.Instance.GetBullet();
                    newBullet.SetTarget(line.Perpendicular1() + bossLoc, boss.dashForce, 3, boss.gameObject, boss.transform.position, boss.transform.rotation);
                    newBullet = EnemyManager.Instance.GetBullet();
                    newBullet.SetTarget(line.Perpendicular1() * -1 + bossLoc, boss.dashForce, 3, boss.gameObject, boss.transform.position, boss.transform.rotation);
                    yield return new WaitForSeconds(boss.dashFirerate / boss.phase);
                }
                // Update the current location
                currentLocation = nextLocation;
                // Change state
                boss.SelectNewState();
            }
            attackStarted = false;
        }

    }
    // In the shotgun state, the boss will move towards the player, and fire bullets in a shotgun like arc
    class Shotgun : BossState
    {
        bool attackStarted = false;
        public override void Act(Boss boss)
        {
            if (!attackStarted)
            {
                attackStarted = true;
                boss.StartCoroutine(Attack(boss));
            }
        }
    }
}


