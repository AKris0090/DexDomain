using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public static CharacterMovement _cmInstance;

    public float moveSpeed;
    private float _moveX, _moveY;
    public Rigidbody2D _rb;
    public Vector2 _mousePos;
    public Vector2 mouseDirection;
    public bool enableMovement_ = true;
    public int dmgMod = 1;
    public bool invulnerable = false;
    public Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Cursor.lockState = CursorLockMode.Confined;
        enableMovement_ = true;
        _cmInstance = this;
        invulnerable = false;
    }

    private void Update()
    {
        if (_cmInstance.enableMovement_)
        {
            // Update velocity based on the input, adjust with normalization and multiply by movement speed
            _moveX = Input.GetAxisRaw("Horizontal");
            _moveY = Input.GetAxisRaw("Vertical");
            _rb.velocity = (new Vector2(_moveX, _moveY)).normalized * moveSpeed;
        }

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

    public IEnumerator incMS(float time)
    {
        this.moveSpeed *= 2;
        yield return new WaitForSeconds(time);
        this.moveSpeed /= 2;
    }

    public IEnumerator incDmg(float time)
    {
        this.dmgMod = 2;
        yield return new WaitForSeconds(time);
        this.dmgMod = 1;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Door"))
        {
            GameObject door = col.gameObject;
            Room fromRoom = door.GetComponentInParent<Room>();
            RoomData.Dir doorDir = fromRoom.GetDoorDirection(door);

            if (fromRoom.IsDoorLocked(doorDir))
                return;

            Vector2 telePos = fromRoom.TeleportLocationToNextRoom(door);
            Room toRoom = fromRoom.GetRoomFromDirection(doorDir);
            mainCam.GetComponent<CameraStandard>().MoveTo(toRoom.CameraPosition);
            transform.position = new (telePos.x, telePos.y, transform.position.z);

            if (toRoom.Started == false && toRoom.Ended == false)
                toRoom.StartRoom();
            else
                toRoom.SetVisibility(true);

            if (fromRoom.Started == true && fromRoom.Ended == false)
                fromRoom.EndRoom();

            fromRoom.SetVisibility(false);
        }
    }
}
