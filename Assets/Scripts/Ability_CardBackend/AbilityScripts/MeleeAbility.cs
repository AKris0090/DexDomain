using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MeleeAbility : MonoBehaviour
{
    public void spinSword(GameObject swordPrefab)
    {
        StartCoroutine(fullSpin(swordPrefab, .5f));
    }

    // https://stackoverflow.com/questions/66286142/unity-2d-how-to-rotate-an-object-smoothly-inside-a-coroutine
    IEnumerator fullSpin(GameObject p, float seconds)
    {
        Quaternion startRotation = p.transform.rotation;
        float endZRot = 360.0f;
        float duration = seconds;
        float t = 0;

        while (t < 1f)
        {
            t = Mathf.Min(1f, t + Time.deltaTime / duration);
            Vector3 newEulerOffset = Vector3.forward * (endZRot * t);
            p.transform.rotation = startRotation * Quaternion.Euler(-newEulerOffset);
            yield return null;
        }
        Destroy(p);
    }

    public void slashSword(GameObject swordPrefab, float duration)
    {
        StartCoroutine(cut(swordPrefab, duration));
    }

    IEnumerator cut(GameObject p, float seconds)
    {
        Quaternion startRotation = p.transform.rotation;
        float endZRot = 90;
        float duration = seconds;
        float t = 0;

        while (t < 1f)
        {
            t = Mathf.Min(1f, t + Time.deltaTime / duration);
            Vector3 newEulerOffset = Vector3.forward * (endZRot * t);
            p.transform.rotation = startRotation * Quaternion.Euler(newEulerOffset);
            yield return null;
        }
        Destroy(p);
    }
}
