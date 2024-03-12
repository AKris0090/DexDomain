using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    BossState state;
    BossState idle;
    BossState bulletHell;
    BossState dash;
    BossState shotgun;
    public int numOfBulletHellRings = 4;
    public int bulletsInRings = 20;
    public float ringBulletForce = 100;
    public float bulletLifespan = 5;
    int phase;
    int maxHeatlth;
    NavMeshAgent agent;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        state = idle;
        phase = 1;
        maxHeatlth = health;
        state = new Idle();
        idle = state;
        bulletHell = new BulletHell();
        dash = new Dash();
        shotgun = new Shotgun();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(health < maxHeatlth / 2)
        {
            phase++;
        }
        state.Act(this);
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
            Vector2 pos = boss.transform.position;
            Vector3 playerPos = boss.enemyManager.GetPlayerPosition();

            // Draw a line between the boss and the player
            Vector2 line = playerPos - boss.transform.position;
            // Draw a line from here to the player
            RaycastHit2D hit = Physics2D.Raycast(boss.transform.position, line);
            // If this line hit the player
            Debug.Log(hit.collider.gameObject);
            if (boss.enemyManager.CheckIfPlayer(hit.collider.gameObject))
            {
                boss.state = boss.bulletHell;
            }
            yield return new WaitForSeconds(0.1f);
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
            int ringsToSpawn = boss.numOfBulletHellRings * boss.phase;
            int bulletsToSpawn = boss.bulletsInRings * boss.phase;
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
                    newBullet.SetTarget(dir + boss.transform.position, boss.ringBulletForce, boss.ringBulletForce, boss.gameObject, boss.transform.position, boss.transform.rotation);
                }
                yield return new WaitForSeconds(2f);
                for (int j = 0; j < bulletsToSpawn; j++)
                {
                    Bullet newBullet = EnemyManager.Instance.GetBullet();
                    float properAngle = (((2 * Mathf.PI) / bulletsToSpawn) * j) + (((2 * Mathf.PI) / bulletsToSpawn)/2);
                    float x = Mathf.Cos(properAngle);
                    float y = Mathf.Sin(properAngle);
                    Debug.Log("Angle: " + properAngle + " x: " + x + " y: " + y);
                    Vector3 dir = new Vector3(x * 10f, y * 10f, 0);
                    newBullet.SetTarget(dir + boss.transform.position, boss.ringBulletForce, boss.ringBulletForce, boss.gameObject, boss.transform.position, boss.transform.rotation);
                }
                yield return new WaitForSeconds(2f);
            }
            yield return new WaitForSeconds(0.1f);
            attackStarted = true;
        }
    }

    // In the dash state, the boss will dash towards predetermined parts of the room, and produce bullets as it does so
    class Dash : BossState
    {
        public override void Act(Boss boss)
        {
            
        }
    }
    // In the shotgun state, the boss will move towards the player, and fire bullets in a shotgun like arc
    class Shotgun : BossState
    {
        public override void Act(Boss boss)
        {

        }
    }
}


