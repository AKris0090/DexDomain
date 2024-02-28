using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    public Transform playerPos;
    public float _distance = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Set camera position to the position of the player. subtract from z to bring the camera up a couple z-layers
        this.gameObject.transform.position = new Vector3(playerPos.position.x, playerPos.position.y, playerPos.position.z - _distance);
    }
}
