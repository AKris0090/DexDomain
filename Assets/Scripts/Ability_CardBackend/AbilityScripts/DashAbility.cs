using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/DashAbility")]
public class DashAbility : AbilityAbstract
{
    private float dashTime = 0.35f;
    private float dashPower = 27f;
    public GameObject player;

    public override async void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        if (CharacterMovement._cmInstance._rb != null)
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
}
