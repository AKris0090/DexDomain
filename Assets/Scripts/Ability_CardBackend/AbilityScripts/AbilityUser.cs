using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// generic AbilityUser class for potential future use?
// (initially made for testing)
public class AbilityUser : MonoBehaviour
{
    [SerializeField] private GameObject AbilityPrefab;
    private BaseAbility Ability;

    // Start is called before the first frame update
    void Start()
    {
        Ability = AbilityPrefab.GetComponent<BaseAbility>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("use ability test");
            Ability.UseAbility();
        }
    }
}
