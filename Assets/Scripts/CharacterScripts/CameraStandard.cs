using System.Collections;
using UnityEngine;

public class CameraStandard : MonoBehaviour
{
    [SerializeField]
    private float _easeDuration = 0.5f;
    // TESTER CODE
    public Vector2 testPosition;
    public void MoveTo(Vector2 position)
    {
        StartCoroutine(LerpToPosition(position));
    }

    public void Test()
    {
        MoveTo(testPosition);
    }

    private IEnumerator LerpToPosition(Vector2 endPosition)
    {
        float time = 0;

        Vector2 startPosition = transform.position;
        while (time <= _easeDuration)
        {
            time += Time.deltaTime;
            var progress = time / _easeDuration;
            transform.position = EaseOutQuintVec(startPosition, endPosition, progress);
            yield return new WaitForEndOfFrame();
        }
    }

    private Vector3 EaseOutQuintVec(Vector2 start, Vector2 end, float progress)
    {
        var vec = Vector2.Lerp(start, end, EaseOutQuint(progress));
        return new(vec.x, vec.y, -10f);
    }

    private float EaseOutQuint(float x) 
    {
            return 1 - Mathf.Pow(1 - x, 5);
    }
}
