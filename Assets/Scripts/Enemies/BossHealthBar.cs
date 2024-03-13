using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Image bar;
    public GameObject holder;
    float maxHealth;
    // The allegations that I have created Yet Another Singleton Manager are naught but malice and lies (please ignore the below code)
    private static BossHealthBar _instance;
    public static BossHealthBar Instance { get { return _instance; } }
    void Awake()
    {
        Debug.Log("menu manager exists (UIManager component)");
        if (_instance == null) // If there is no instance already
        {
            _instance = this;
        }
        else if (_instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    public void Activate(int initialHealth)
    {
        Debug.Log("hm");
        holder.SetActive(true);
        bar.fillAmount = 1;
        maxHealth = initialHealth;
    }

    public void Deactivate()
    {
        holder.SetActive(false);
    }

    public void UpdateHealth(float currentHealth)
    {
        bar.fillAmount = currentHealth / maxHealth;
    }
}
