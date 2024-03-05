using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public Canvas healthPointsContainer;
    public GameObject heartsPrefab;
    public RangedIntReference playerHealth;
    private int prevHealth;

    // image refactoring
    private List<GameObject> heartsPool;

    void Start()
    {
        prevHealth = playerHealth.MaxValue; // start at max health

        Debug.Log(playerHealth.MaxValue);
        for (int i=0; i<playerHealth.MaxValue; ++i)
        {
            GameObject heartSprite = Instantiate(heartsPrefab, healthPointsContainer.transform, false);

            heartsPool.Add(heartSprite);
        }

        UpdateHeartsHUD();
    }

    // Update is called once per frame
    void Update()
    {
        if (prevHealth != playerHealth.Value)
        {
            UpdateHeartsHUD();
        }
    }

    private void UpdateHeartsHUD()
    {

    }
}
