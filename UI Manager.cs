using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    bool isPaused = false;

    private void Start()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPaused == false)
            {
                isPaused = true;
                Pause();
            }
            else
            {
                isPaused = false;
                Resume();
            }
        }
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
}
