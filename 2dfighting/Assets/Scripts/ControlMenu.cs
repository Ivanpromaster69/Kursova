using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlMenu : MonoBehaviour
{
    public GameObject settings;
    public Button[] buttonsToDisable;
    void Start()
    {
        settings.SetActive(false);   
    }
    public void Settings(){
        if (settings.activeSelf == false) {
            settings.SetActive(true);
        }
        else if (settings.activeSelf == true) {
            settings.SetActive(false);
        }
    }
    public void Fight(){
        SceneManager.LoadScene("Fight");
    }
    public void Exit(){
        Debug.Log("Exit Game");
        Application.Quit();
    }
}
