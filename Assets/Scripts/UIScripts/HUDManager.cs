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
    private List<GameObject> heartsPool = new();

    void Start()
    {
        prevHealth = playerHealth.MaxValue; // start at max health
        for (int i=0; i<playerHealth.MaxValue; ++i)
        {
            GameObject heartSprite = Instantiate(heartsPrefab, healthPointsContainer.transform, false);
            RectTransform t = heartSprite.GetComponent<RectTransform>();
            Vector3 t2 = t.anchoredPosition;
            t2.x += 125*i;
            t.anchoredPosition = t2;

            heartsPool.Add(heartSprite);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (prevHealth != playerHealth.Value)
        {
            prevHealth = playerHealth.Value;
            for (int i=0; i<playerHealth.Value; ++i)
            {
                heartsPool[i].SetActive(true);
            }
            for (int j=playerHealth.Value; j<heartsPool.Count; ++j)
            {
                heartsPool[j].SetActive(false);
            }
        }
    }
}
