using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(4);
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting!");
    }
    public void returnToMenu(){
        SceneManager.LoadScene(2);
    }
}
