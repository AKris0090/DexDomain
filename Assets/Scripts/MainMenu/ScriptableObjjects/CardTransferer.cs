using CardOperations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardTransferer", menuName = "CardSelection/CardTransferer", order = 1)]
[Serializable]
public class CardTransferer : ScriptableObject
{
    public List<BaseCardClass> selectedCards;
    public void Clear()
    {
        selectedCards = new List<BaseCardClass>();
    }
}
