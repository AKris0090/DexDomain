using CardOperations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardSelector : MonoBehaviour
{
    public int cardsPerCatagory = 2;
    int numPicked = 0;
    int slots = 5;
    public List<Canvas> cardSlots;
    public CardTransferer transferer;
    public List<BaseCardClass> primaries;
    public List<BaseCardClass> secondaries;
    public List<BaseCardClass> specials;
    public List<BaseCardClass> movements;
    List<List<BaseCardClass>> lists;
    int currentSlot;
    // Start is called before the first frame update
    void Start()
    {
        transferer.Clear();
        lists = new List<List<BaseCardClass>>();
        lists.Add(primaries);
        lists.Add(secondaries);
        lists.Add(specials);
        lists.Add(movements);

        StartCoroutine(SelectCards());
    }

    IEnumerator SelectCards()
    {
        // For every type of card
        foreach (List<BaseCardClass> list in lists)
        {
            currentSlot = 0;
            numPicked = 0;
            // Pick upto slots random cards
            for (int i = 0; i < slots; i++)
            {
                int selector = Random.Range(0, list.Count);
                BaseCardClass card = list[selector];
                GameObject newCard = Instantiate(card.cardUIPrefab, cardSlots[currentSlot].transform, false);
                Button cardButton = newCard.GetComponentInChildren<Button>();
                cardButton.onClick.AddListener(() => AddCardToTransfer(card, cardButton));
                currentSlot++;
                list.Remove(card);
                if (list.Count == 0)
                {
                    break;
                }
            }
            // Wait until the player has picked their cards
            while (numPicked < cardsPerCatagory)
            {
                yield return new WaitForSeconds(0.1f);
            }
            // Remove leftover cards from slots
            for (int i = 0; i < slots; i++)
            {
                Button cardButton = cardSlots[i].GetComponentInChildren<Button>();
                if (cardButton != null)
                {
                    Destroy(cardButton.gameObject);
                }
            }
        }
        SceneManager.LoadScene("FinalScene");
    }


    void AddCardToTransfer(BaseCardClass card, Button UICard)
    {
        transferer.selectedCards.Add(card);
        Destroy(UICard.gameObject);
        numPicked++;
    }

}
