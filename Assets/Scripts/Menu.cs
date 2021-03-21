using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public TextMeshProUGUI scoreText;
	public GameObject mapman;
	public Base playerBase;
	

	public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Start is called before the first frame update
    void Update()
    {
		
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
		if (!playerBase /*&& SceneManager.GetActiveScene().name == "MainScene"*/)
		{
			playerBase = GameObject.FindGameObjectWithTag("base")?.GetComponent<Base>();
		}
		//Needs to know how game is over
		else if (playerBase.gameOver)
        {
            gameOverMenu.SetActive(true);
            Time.timeScale = 0f;
			scoreText.text = "" + (mapman.GetComponent<MapManager>().currentWave - 2);
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Restart()
    {		
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
