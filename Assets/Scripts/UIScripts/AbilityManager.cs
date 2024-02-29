using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum ABILITIES
{
    Attack = 0,
    Weapon1 = 1,
    Weapon2 = 2,
    Equipment = 3,
}

public class AbilityManager : MonoBehaviour
{
    public List<Canvas> abilitySlots;
    private List<bool> abilityActive = new()
    {
        false,
        false,
        false,
        false,
    };
    public List<Canvas> deckSlots;
    private List<string> deckSetup = new() {
        "FIREBALL",
        "SWORD",
        "SPEAR",
        "DAGGER",
        "BOOTS",
    };

    // get card manager obj!!

    // keybinds
    private KeyCode ABILITY1_K = KeyCode.Q;
    private KeyCode ABILITY2_K = KeyCode.W;
    private KeyCode ABILITY3_K = KeyCode.E;
    private KeyCode ABILITY4_K = KeyCode.R;

    // Start is called before the first frame update
    void Start()
    {
        // add event listeners for deck
        foreach (Canvas cardSlot in deckSlots)
        {
            // Find the button within the canvas
            Button card = cardSlot.GetComponentInChildren<Button>();

            if (card != null)
            {
                // Add the onClick function "foo" to the button
                card.onClick.AddListener(() => SwapToCard(deckSetup[deckSlots.IndexOf(cardSlot)]));

            }
        }

        // hide all abilities at start
        foreach (Canvas ability in abilitySlots)
        {
            Button Card = ability.GetComponentInChildren<Button>();
            Card.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(ABILITY1_K))
        {
            Ability1();
        }
        if (Input.GetKeyDown(ABILITY2_K))
        {
            Ability2();
        }
        if (Input.GetKeyDown(ABILITY3_K))
        {
            Ability3();
        }
        if (Input.GetKeyDown(ABILITY4_K))
        {
            Ability4();
        }
    }

    // wrapper function for casting abilities
    public void Ability1()
    {
        Debug.Log("do the ability 1 thing");
    }
    public void Ability2()
    {
        Debug.Log("do the ability 2 thing");
    }
    public void Ability3()
    {
        Debug.Log("do the ability 3 thing");
    }
    public void Ability4()
    {
        Debug.Log("do the ability 4 thing");
    }

    // wrapper to contact card manager
    void SwapToCard(string Card)
    {
        Debug.Log("swaping to card " + Card);

        int abilityIndex; // ability to swap to

        // set up abilities (hardcoded sorry)
        if (Card == "FIREBALL")
        {
            abilityIndex = 0;
        }
        else if (Card == "SWORD" || Card == "SPEAR")
        {
            abilityIndex = 1;
        }
        else if (Card == "DAGGER")
        {
            abilityIndex = 2;
        }
        else
        {
            abilityIndex = 3;
        }

        Canvas _ability = abilitySlots[abilityIndex];
        Button _button = _ability.GetComponentInChildren<Button>(true);

        if (!abilityActive[abilityIndex])
        {
            abilityActive[abilityIndex] = true;
            
            Debug.Log(_button);
            _button.gameObject.SetActive(true);
        }

        // we are kinda only just using the text rn
        _button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = Card;

    }
}
