using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/Teleport")]
public class Teleport : AbilityAbstract
{
    public GameObject player;
    private Vector3 toTP;
    public override async void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.UseAbility(playerPos, lookAt);
        CharacterMovement._cmInstance._rb.velocity = new Vector2(0, 0);
        CharacterMovement._cmInstance.enableMovement_ = false;
        CharacterMovement._cmInstance.invulnerable = true;
        toTP = CharacterMovement._cmInstance._mousePos;
        await Task.Delay(500);
        CharacterMovement._cmInstance.gameObject.transform.position = new Vector3(toTP.x, toTP.y, 0);
        CharacterMovement._cmInstance.enableMovement_ = true;
        CharacterMovement._cmInstance.invulnerable = false;
    }
}
