using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Redcode.Pools;
using static UnityEditor.FilePathAttribute;

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
    public GameObject basicShoot;

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
    }

    public Bullet GetBullet()
    {
        return bullets.Get();
    }

    public void ReturnBullet(Bullet bullet)
    {
        bullets.Take(bullet);
    }

    // Spawns a basic shoot enemy at location, targeting target
    // Basic shoot enemies move towards their target, until they're within a certain range, before firing a damaging projectile
    public void SpawnBasicShoot(Vector3 location)
    {
        Instantiate(basicShoot, location, basicShoot.transform.rotation);
    }

    // Hurts the passed in enemy
    // TODO: firgue out a better way to do this
    // the switch statement to determine which enemy has been damaged, then calling the respective damage function,
    // feels clunky and kind of bad. I'll have to ask cole about a better way of doing this
    public void DamageEnemy(GameObject enemy, int amount)
    {
        Debug.Log(enemy);
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
            // TODO: make this actually damage the player 
            // Whoever does this should make sure the player doesn't take damage too much as part of the player,
            // in case we have other managers that can damage the player.
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
