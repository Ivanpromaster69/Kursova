using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseFight : MonoBehaviour
{
    public GameObject pause;
    public GameObject keybind;
    void Start() {
        pause.SetActive(false);
        keybind.SetActive(false);   
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }
    public void Keybind() {
        if (keybind.activeSelf == false) {
            keybind.SetActive(true);
        }
        else if (keybind.activeSelf == true) {
            keybind.SetActive(false);
        }
    }
    private void TogglePause() {
        pause.SetActive(!pause.activeSelf);
        if (pause.activeSelf) {
            Time.timeScale = 0f;
        }
        else {
            Time.timeScale = 1f;
        }
    }
    public void Menu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}