using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class ToScene : MonoBehaviour
{
    public string scene;
    public void goToScene()
    {
        SceneManager.LoadScene(scene);
    }
}
