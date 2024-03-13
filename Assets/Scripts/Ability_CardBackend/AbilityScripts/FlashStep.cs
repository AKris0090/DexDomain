using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/FlashStep")]
public class FlashStep : AbilityAbstract
{
    public float dashTime = 0.005f;
    private float dashPower = 100000000f;
    public GameObject player;

    public override async void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.UseAbility(playerPos, lookAt);
        CharacterMovement._cmInstance.enableMovement_ = false;
        CharacterMovement._cmInstance._rb.velocity = lookAt.normalized * dashPower;
        CharacterMovement._cmInstance.invulnerable = true;
        await Task.Delay((int)(dashTime * 1000));
        CharacterMovement._cmInstance._rb.velocity = new Vector2(0, 0);
        CharacterMovement._cmInstance.enableMovement_ = true;
        CharacterMovement._cmInstance.invulnerable = false;
    }
}
