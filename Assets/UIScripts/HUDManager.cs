using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public Canvas HPContainer;
    List<GameObject> Hearts;
    public GameObject HeartPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // grab health data (max health)
        //Hearts.Add(Instantiate(HeartPrefab));
        UpdateHealthHUD();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHealthHUD()
    {
        // grab data from scriptable obj

        // I HAVE NO IDEA WHAT IM DOING HERE SORRY
        
    }
}
