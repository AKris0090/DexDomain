using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{

    private CharacterMovement _characterMovement;
    private Vector2 _mousePos;

    // Start is called before the first frame update
    void Start()
    {
        _characterMovement = this.gameObject.GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) {
            // Fire the primary weapon/card towards the mouse position
            CardManager.Instance.UsePrimary(transform.position, _characterMovement.mouseDirection);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Fire the primary weapon/card towards the mouse position
            CardManager.Instance.UseSecondary(transform.position, _characterMovement.mouseDirection);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Fire the primary weapon/card towards the mouse position
            CardManager.Instance.UseMovement(transform.position, _characterMovement.mouseDirection);
        }
    }
}
