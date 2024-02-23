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
    public List<Canvas> AbilitySlots;
    private List<bool> AbilityActive = new()
    {
        false,
        false,
        false,
        false,
    };
    public List<Canvas> DeckSlots;
    private List<string> DeckSetup = new() {
        "FIREBALL",
        "SWORD",
        "SPEAR",
        "DAGGER",
        "BOOTS",
    };

    // get card manager obj!!

    // keybinds
    private KeyCode Ability1_K = KeyCode.Q;
    private KeyCode Ability2_K = KeyCode.W;
    private KeyCode Ability3_K = KeyCode.E;
    private KeyCode Ability4_K = KeyCode.R;

    // Start is called before the first frame update
    void Start()
    {
        // add event listeners for deck
        foreach (Canvas CardSlot in DeckSlots)
        {
            // Find the button within the canvas
            Button Card = CardSlot.GetComponentInChildren<Button>();

            if (Card != null)
            {
                // Add the onClick function "foo" to the button
                Card.onClick.AddListener(() => SwapToCard(DeckSetup[DeckSlots.IndexOf(CardSlot)]));

            }
        }

        // hide all abilities at start
        foreach (Canvas Ability in AbilitySlots)
        {
            Button Card = Ability.GetComponentInChildren<Button>();
            Card.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Ability1_K))
        {
            Ability1();
        }
        if (Input.GetKeyDown(Ability2_K))
        {
            Ability2();
        }
        if (Input.GetKeyDown(Ability3_K))
        {
            Ability3();
        }
        if (Input.GetKeyDown(Ability4_K))
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

        int A; // ability to swap to

        // set up abilities (hardcoded sorry)
        if (Card == "FIREBALL")
        {
            A = 0;
        }
        else if (Card == "SWORD" || Card == "SPEAR")
        {
            A = 1;
        }
        else if (Card == "DAGGER")
        {
            A = 2;
        }
        else
        {
            A = 3;
        }

        Canvas Ability = AbilitySlots[A];
        Button B = Ability.GetComponentInChildren<Button>(true);

        if (!AbilityActive[A])
        {
            AbilityActive[A] = true;
            
            Debug.Log(B);
            B.gameObject.SetActive(true);
        }

        // we are kinda only just using the text rn
        B.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = Card;

    }
}
