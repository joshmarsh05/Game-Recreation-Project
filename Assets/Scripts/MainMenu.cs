using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    private bool paused = false;
    public GameObject pauseScreen;
    [SerializeField] AudioManager audioManager;

    private void Start() {
        if(!pauseScreen)
            pauseScreen = GameObject.Find("PauseScreen");
        if(!audioManager)
            audioManager = GameObject.Find("HUD").GetComponent<AudioManager>();
    }

    private void Update() {
        if(pauseScreen){
            if(Input.GetKeyDown(KeyCode.Escape) && paused == false){
                Pause();
            }
            else if(Input.GetKeyDown(KeyCode.Escape) && paused == true){
                UnPause();
            }
        }
    }

    public void Pause(){
        Time.timeScale = 0;
        paused = true;
        pauseScreen.SetActive(true);
        audioManager.PauseMusic();
        audioManager.PlaySFX("Pause");
    }

    public void UnPause(){
        Time.timeScale = 1;
        paused = false;
        pauseScreen.SetActive(false);
        audioManager.ResumeMusic();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Level 1");
    }

    public void Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
