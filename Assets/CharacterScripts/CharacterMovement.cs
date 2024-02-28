using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterMovement : MonoBehaviour
{

    public float moveSpeed;
    private float _moveX, _moveY;
    private Rigidbody2D _rb;
    private Vector2 _mousePos;
    public Vector2 mouseDirection;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        // Update velocity based on the input, adjust with normalization and multiply by movement speed
        _moveX = Input.GetAxisRaw("Horizontal");
        _moveY = Input.GetAxisRaw("Vertical");
        _rb.velocity = (new Vector2(_moveX, _moveY)).normalized * moveSpeed;

        // CODE FROM: https://discussions.unity.com/t/2d-look-at-mouse-position-z-rotation-c/117860

        // set vector of transform directly
        transform.up = mouseDirection;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // CODE FROM: https://discussions.unity.com/t/2d-look-at-mouse-position-z-rotation-c/117860

        // convert mouse position into world coordinates
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        mouseDirection = (_mousePos - (Vector2)transform.position).normalized;
    }
}
