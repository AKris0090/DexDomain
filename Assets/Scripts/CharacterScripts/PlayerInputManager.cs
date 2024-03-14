using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{

    private CharacterMovement _characterMovement;
    private Vector2 _mousePos;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("PrimaryAttack")) {
            // Fire the primary weapon/card towards the mouse position
            Debug.Log("player input primary attack");
            CardManager.Instance.UsePrimary(transform.position, CharacterMovement._cmInstance.mouseDirection);
        }
        if (Input.GetButton("SecondaryAttack"))
        {
            // Fire the secondary weapon/card towards the mouse position
            CardManager.Instance.UseSecondary(transform.position, CharacterMovement._cmInstance.mouseDirection);
        }
        if (Input.GetButton("SpecialAbility"))
        {
            // Fire the special weapon/card towards the mouse position
            CardManager.Instance.UseSpecial(transform.position, CharacterMovement._cmInstance.mouseDirection);
        }
        if (Input.GetButton("MovementAbility"))
        {
            // Use the movement weapon/card towards the mouse position
            CardManager.Instance.UseMovement(transform.position, CharacterMovement._cmInstance.mouseDirection);
        }
    }
}
