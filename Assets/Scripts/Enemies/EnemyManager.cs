using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

// Singlton manager for managing enemies
// To get it to work, add the EnemyManager prefab to a scene
// Connect the player to the player variable in the scene manager
// Make sure the player has a collider (box or circle) attachec to it (please add this arjun)
// Add the property Navigation Modifier to everything the enemies will walk over or need to avoid
// Check Override Area, then set Area to Not Walkable for obsticles 
// Double check all obsticals also have a collider on them as well
// Put all walkable areas on the ignoreRaycasts layer
// Then, add an empty object, with the modifiers Navigation Surface and Navigation CollectSources2d
// Click Rotatate Surface to xy on the collectSources, then bake on the surface
// With any luck at all, provided i haven't forgotten anything, that should build the navmesh, and enemies should be able to walk around

public class EnemyManager : MonoBehaviour
{
    // Variables for all of the enemy types
    public Enemy basicShoot;
    public Enemy rapidFire;
    public Enemy ram;
    List<Enemy> mainEnemyList;

    // Variable to keep track of the player
    public GameObject player;

    // Bullet used by all enemies
    public Bullet enemyBullet;
    Pool<Bullet> bullets;

    // Singletonize it
    private static EnemyManager _instance;
    public static EnemyManager Instance { get { return _instance; } }
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        // Create a pool for enemy bullets
        bullets = Pool.Create(enemyBullet);
        // add all the enemy types to the enemy list
        mainEnemyList = new List<Enemy>
        {
            basicShoot,
            ram,
            rapidFire
        };
    }

    public Bullet GetBullet(Vector3 spawnPosition, Quaternion rotation)
    {
        Bullet bullet = bullets.Get();
        bullet.GetComponent<TrailRenderer>().enabled = false;
        bullet.GetComponent<TrailRenderer>().Clear();
        bullet.transform.position = spawnPosition;
        bullet.transform.rotation = rotation;
        bullet.GetComponent<TrailRenderer>().enabled = true;
        return bullet;
    }

    public void ReturnBullet(Bullet bullet)
    {
        if (bullet.gameObject.activeInHierarchy)
        {
            Instantiate(bullet.BulletDeath, bullet.transform.position, bullet.transform.rotation);
            bullets.Take(bullet);
        }
    }

    // Gets count random enemies
    public void GetRandomEnemies(int count, out List<Enemy> enemies)
    {
        enemies = new List<Enemy>();
        Enemy enemy;
        for(int i = 0; i < count; i++)
        {
            GetRandomEnemy(out enemy);
            enemies.Add(enemy);
        }
    }

    // Gets one random enemy
    public void GetRandomEnemy(out Enemy enemy)
    {
        int selector = UnityEngine.Random.Range(0, mainEnemyList.Count);
        enemy = mainEnemyList[selector];
    }

    // Hurts the passed in enemy
    public void DamageEnemy(GameObject enemy, int amount)
    {
        Enemy damagedEnemy = enemy.GetComponent<Enemy>();
        if(damagedEnemy != null)
        {
            damagedEnemy.Damage(amount);
        }
    }

    // Return the player's position, mostly used by enemies
    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    // This function will be called whenever an enemy tries to damage an object
    // So the enemies don't also need to know the player, they will pass all objects they interact with here,
    // at which point the manager will decide if that makes sense
    public void DamageObject(GameObject damaged)
    {
        if(damaged == player)
        {
            player.GetComponent<PlayerHealth>().DealDamage(1);
            return;
        }
        return;
    }

    // Check if possible is player
    public bool CheckIfPlayer(GameObject possible)
    {
        return player == possible;
    }
}
