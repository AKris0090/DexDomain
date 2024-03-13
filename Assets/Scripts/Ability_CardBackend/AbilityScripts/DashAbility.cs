using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/DashAbility")]
public class DashAbility : AbilityAbstract
{
    public float force;
    public float lifespan;
    public float dashTime = 1.5f;
    public float dashPower = 100f;
    public GameObject player;

    public override async void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.UseAbility(playerPos, lookAt);
        CharacterMovement._cmInstance.enableMovement_ = false;
        CharacterMovement._cmInstance._rb.velocity = lookAt.normalized * dashPower;
        await Task.Delay((int) (dashTime * 1000));
        CharacterMovement._cmInstance._rb.velocity = new Vector2(0, 0);
        CharacterMovement._cmInstance.enableMovement_ = true;
    }
}
