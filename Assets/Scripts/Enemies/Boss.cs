using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static RoomData;
using static Unity.VisualScripting.Member;

public class Boss : Enemy
{
    BossState state;
    BossState idle;
    BossState bulletHell;
    BossState dash;
    BossState shotgun;
    BossState deathRattle;
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
    public int numberOfShotgunShots = 5;
    public float shotGunForce = 500f;
    public float shotgunFireRate = 0.5f;
    public int numberOfShells = 5;
    public float phaseChangeTime = 2f;
    ParticleSystem bloodEmmiter;
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
        deathRattle = new Deathrattle();
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

        canLookAtPlayer = false;
        StartCoroutine(Spin());

        bloodEmmiter = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health < maxHeatlth / 2 && !phaseChanged)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            phase += 0.5f;
            phaseChanged = true;
        }
        else
        {
            state.Act(this);
        }
    }

    IEnumerator Spin()
    {
        while (true)
        {
            if(phase > 1)
            {
                transform.Rotate(Vector3.forward, 10);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    void SelectNewState()
    {
        // Don't selected a new state if the boss is dying
        if (state != deathRattle)
        {
            // This allows for the boss repeatedly selecting the same state. Should be okay? 
            int selector = Random.Range(0, states.Count);
            state = states[selector];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemyManager.DamageObject(collision.gameObject);
    }

    public override void Damage(int amount)
    {
        if (canTakeDamage)
        {
            bloodEmmiter.Emit(20);
            health -= amount;
            StartCoroutine(DamageFeedback());
            StartCoroutine(DamageCooldown());
            canTakeDamage = false;
            Debug.Log("Damage delt to boss");
            BossHealthBar.Instance.UpdateHealth(health);
            if (health <= 0)
            {
                state = deathRattle;
            }
        }
    }

    // Boss state is a state machine, that handles the bosses state
    abstract class BossState
    {
        abstract public void Act(Boss boss);
    }

    class Deathrattle : BossState
    {
        bool dying = false;
        public override void Act(Boss boss)
        {
            if (!dying)
            {
                boss.StopAllCoroutines();
                dying = true;
                boss.StartCoroutine(Die(boss));
            }
        }

        IEnumerator Die(Boss boss)
        {
            for (int i = 0; i < 10; i++)
            {
                boss.bloodEmmiter.Emit(50);
                yield return new WaitForSeconds(0.3f);
            }
            SceneManager.LoadScene("Victory"); 
            Destroy(boss.gameObject);
        }

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
                Debug.Log(boss);
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
                boss.state = boss.bulletHell;
                BossHealthBar.Instance.Activate(boss.health);
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
                    Bullet newBullet = EnemyManager.Instance.GetBullet(boss.transform.position, boss.transform.rotation);
                    float properAngle = ((2*Mathf.PI) / bulletsToSpawn) * j;
                    float y = Mathf.Cos(properAngle);
                    float x = Mathf.Sin(properAngle);
                    Vector3 dir = new Vector3(x * 10f, y * 10f, 0);
                    Debug.Log(boss.ringBulletForce);
                    newBullet.SetTarget(dir + boss.transform.position, boss.ringBulletForce * boss.phase, 5, boss.gameObject);
                }
                yield return new WaitForSeconds(boss.secondsBetweenRings);
                for (int j = 0; j < bulletsToSpawn; j++)
                {
                    Bullet newBullet = EnemyManager.Instance.GetBullet(boss.transform.position, boss.transform.rotation);
                    float properAngle = (((2 * Mathf.PI) / bulletsToSpawn) * j);
                    float x = Mathf.Cos(properAngle);
                    float y = Mathf.Sin(properAngle);
                    Vector3 dir = new Vector3(x * 10f, y * 10f, 0);
                    newBullet.SetTarget(dir + boss.transform.position, boss.ringBulletForce * boss.phase, 5, boss.gameObject);
                }
                yield return new WaitForSeconds(boss.secondsBetweenRings);
            }
            yield return new WaitForSeconds(boss.phaseChangeTime);
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
                agent.SetDestination(nextLocation);
                agent.speed = boss.dashSpeed * boss.phase;
                agent.acceleration = 200;
                // Wait until the boss is sutibly close to the end of the dash, firing bullets as it goes
                while ((boss.transform.position - nextLocation).magnitude >= 1)
                {
                    Vector2 line = (boss.transform.position - nextLocation);
                    Vector2 bossLoc = boss.transform.position;
                    Bullet newBullet = EnemyManager.Instance.GetBullet(boss.transform.position, boss.transform.rotation);
                    // Fire bullets perpendicuarly from the direction of travel
                    newBullet.SetTarget(Vector2.Perpendicular(line) + bossLoc, boss.dashForce, 3, boss.gameObject);
                    newBullet = EnemyManager.Instance.GetBullet(boss.transform.position, boss.transform.rotation);
                    newBullet.SetTarget(Vector2.Perpendicular(line) * -1 + bossLoc, boss.dashForce, 3, boss.gameObject);
                    yield return new WaitForSeconds(boss.dashFirerate / boss.phase);
                }
                // Update the current location
                currentLocation = nextLocation;
            }
            yield return new WaitForSeconds(boss.phaseChangeTime);
            // Change state
            boss.SelectNewState();
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

        IEnumerator Attack(Boss boss)
        {
            int phasedNumOfShells = (int)(boss.numberOfShells * boss.phase);
            if(phasedNumOfShells % 2 == 0)
            {
                phasedNumOfShells -= 1;
            }
            // If the enemy is ready to fire, do so
            for (int i = 0; i < boss.numberOfShotgunShots * boss.phase; i++)
            {
                Vector3 playerPos = EnemyManager.Instance.GetPlayerPosition();
                Vector2 line = -(boss.transform.position - playerPos);
                // OuterArc should be at a 45 degree angle from the boss to the player
                Vector3 outerArc = line + Vector2.Perpendicular(line);
                for (int j = 0; j < phasedNumOfShells; j++)
                {
                    // calculate the angle the bullet should be fired at
                    float intermediateAngle = ((Mathf.PI/4)/phasedNumOfShells) + ((Mathf.PI/2) / phasedNumOfShells) * j;
                    float angle = Mathf.Atan2(outerArc.y, outerArc.x) + intermediateAngle;
                    float x = Mathf.Cos(angle);
                    float y = Mathf.Sin(angle);
                    Vector3 dir = new Vector2(x, y);
                    Bullet newBullet = EnemyManager.Instance.GetBullet(boss.transform.position, boss.transform.rotation);
                    newBullet.SetTarget(dir + boss.transform.position, boss.shotGunForce, 5, boss.gameObject);
                }
                yield return new WaitForSeconds(boss.shotgunFireRate);
            }
            yield return new WaitForSeconds(boss.phaseChangeTime);
            // Change state
            boss.SelectNewState();
            attackStarted = false;
        }
    }
}


