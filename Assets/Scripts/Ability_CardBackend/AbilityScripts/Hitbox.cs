using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    // when hitbox is activated, fades out over duration
    public IEnumerator HitboxTrigger(float duration = 0.5f) 
    {
        gameObject.SetActive(true);
        float timePassed = 0f;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

        while (timePassed < duration)
        {
            Color oldColor = sr.material.color;
            float newAlpha = Mathf.Lerp(1, 0, timePassed / duration);
            sr.material.color = new Color(oldColor.r, oldColor.g, oldColor.b, newAlpha);
            timePassed += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    // access to get damage of a hit (maybe useful?)
    public int getDamage()
    {
        // gets damage value from parent component
        return transform.parent.gameObject.GetComponent<BaseMeleeAbility>().damage;
    }
}
