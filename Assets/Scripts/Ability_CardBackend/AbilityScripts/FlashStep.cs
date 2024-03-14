using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/FlashStep")]
public class FlashStep : AbilityAbstract
{
    private float dashTime = 0.15f;
    private float dashPower = 150f;
    public GameObject player;

    public override async void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.UseAbility(playerPos, lookAt);
        CharacterMovement._cmInstance.enableMovement_ = false;
        CharacterMovement._cmInstance._rb.velocity = lookAt.normalized * dashPower;
        await Task.Delay((int)(dashTime * 1000));
        CharacterMovement._cmInstance._rb.velocity = new Vector2(0, 0);
        CharacterMovement._cmInstance.enableMovement_ = true;
    }
}
