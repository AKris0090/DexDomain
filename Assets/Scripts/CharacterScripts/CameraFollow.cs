using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerPos;
    public float _distance = 5.0f;
    [SerializeField]
    private bool _isFollowing = false;

    // Update is called once per frame
    void Update()
    {
        // Set camera position to the position of the player. subtract from z to bring the camera up a couple z-layers
        if (_isFollowing)
            gameObject.transform.position.Set
            (
                playerPos.position.x,
                playerPos.position.y,
                playerPos.position.z - _distance
            );
    }

    public void SetPlayer(Transform player)
    {
        if (!player)
        {
            Debug.LogError("Player is null");
            Debug.Break();
        }
        else
            playerPos = player;
    }

    public void SetFollowing(bool isFollowing)
    {
        _isFollowing = isFollowing;
    }
}
