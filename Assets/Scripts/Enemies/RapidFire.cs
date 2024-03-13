using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BasicShoot that fires numOfShots times every timeBetweenShots before it goes on cooldown
public class RapidFire : BasicShoot
{
    public float timeBetweenShots;
    public float numOfShots;

    public override void Fire()
    {
        // If the enemy is ready to fire, do so
        if (readyToFire)
        {
            readyToFire = false;
            StartCoroutine(RapidFireShoot());
        }
    }

    IEnumerator RapidFireShoot()
    {
        for (int i = 0; i < numOfShots; i++)
        {
            Bullet newBullet = enemyManager.GetBullet(transform.position, transform.rotation);
            newBullet.SetTarget(enemyManager.GetPlayerPosition(), force, bulletLifespan, this.gameObject);
            yield return new WaitForSeconds(timeBetweenShots);
        }
        StartCoroutine(FireCooldown());
    }
}
