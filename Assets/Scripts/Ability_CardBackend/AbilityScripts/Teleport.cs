using System;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
        toTP.z = 0;
        Vector2 twoDToTP = new Vector2(toTP.x, toTP.y);
        CharacterMovement._cmInstance.StartCoroutine(CharacterMovement._cmInstance.TPParticleTurnOn(0.5f));
        await Task.Delay(500);
        Vector3 pos = CharacterMovement._cmInstance.gameObject.transform.position;
        Vector2 twoDpos = new Vector2(pos.x, pos.y);
        RaycastHit2D[] theThingIHit = Physics2D.RaycastAll(twoDpos, (twoDToTP - twoDpos).normalized, (twoDToTP - twoDpos).magnitude);
        if (theThingIHit.Length <= 1)
        {
            CharacterMovement._cmInstance.gameObject.transform.position = toTP;
        }
        CharacterMovement._cmInstance.enableMovement_ = true;
        CharacterMovement._cmInstance.invulnerable = false;
    }
}
