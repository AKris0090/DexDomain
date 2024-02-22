using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterMovement : MonoBehaviour
{

    public float moveSpeed;
    private float moveX, moveY;
    private Rigidbody2D rb;
    private Vector2 mousePos;
    private GameObject sprite;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        rb.velocity = (new Vector2(moveX, moveY)).normalized * moveSpeed;

        // CODE FROM: https://discussions.unity.com/t/2d-look-at-mouse-position-z-rotation-c/117860

        // convert mouse position into world coordinates
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        Vector2 direction = (mousePos - (Vector2) transform.position).normalized;

        // set vector of transform directly
        transform.up = direction;
    }
}
