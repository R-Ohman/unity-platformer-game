using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS }
    public GameState currentGameState = GameState.GS_PAUSEMENU;

    public const string keyHighScore = "HighScoreLevel1";

    public static GameManager instance;

    public Canvas inGameCanvas;
    public Canvas pauseMenuCanvas;
    public Canvas levelCompletedCanvas;
    public Canvas optionsCanvas;

    public Slider VolumeSlider;


    public TMP_Text scoreText;
    public TMP_Text scoreText2;
    public TMP_Text highScoreText;
    public TMP_Text enemyText;
    public TMP_Text timeText;
    public TMP_Text qualityText;
    private int score = 0;
    private int enemiesSlayed = 0;

    public Image[] keysTab;
    public Image[] heartsTab;


    private float timer = 0;

    void Awake()
    {
        InGame();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        keysTab[0].color = Color.gray;
        keysTab[1].color = Color.gray;
        keysTab[2].color = Color.gray;

        heartsTab[0].color = Color.white;
        heartsTab[1].color = Color.white;
        heartsTab[2].color = Color.white;
        heartsTab[3].color = Color.clear;

        if (!PlayerPrefs.HasKey(keyHighScore))
        {
            PlayerPrefs.SetInt(keyHighScore, 0);
        }

    }

    void Start()
    {
        SetGameState(GameState.GS_GAME);
        UpdateScoreUI();
    }

    void Update()
    {
        timer += Time.deltaTime;
        UpdateTime();
        //Debug.Log(timer);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameState.GS_GAME)
            {
                Debug.Log("The game is off");
                PauseMenu();
            }
            else if (currentGameState == GameState.GS_PAUSEMENU)
            {
                Debug.Log("The game is on");
                InGame();
            }
        }
    }

    public void SetGameState(GameState newGameState)
    {


        currentGameState = newGameState;

        if (currentGameState == GameState.GS_LEVELCOMPLETED)
        {
            levelCompletedCanvas.enabled = true;
            Scene currentScene = SceneManager.GetActiveScene();

            if (currentScene.name == "Level1")
            {
                int highScore = PlayerPrefs.GetInt(keyHighScore);

                if (highScore < score)
                {
                    highScore = score;
                    PlayerPrefs.SetInt(keyHighScore, highScore);
                }
                scoreText2.text = "Score: " + score;
                highScoreText.text = "High Score: " + highScore;
            }

        }
        else { levelCompletedCanvas.enabled = false; }

        if (currentGameState == GameState.GS_PAUSEMENU)
        {
            pauseMenuCanvas.enabled = true;
        }
        else
        {
            pauseMenuCanvas.enabled = false;
        }
        if (newGameState == GameState.GS_GAME)
        {
            inGameCanvas.enabled = true;
        }
        else
        {
            inGameCanvas.enabled = false;
        }


        if (newGameState == GameState.GS_OPTIONS)
        {
            optionsCanvas.enabled = true;
        }
        else
        {
            optionsCanvas.enabled = false;
        }

        //optionsCanvas.enabled = (newGameState == GameState.GS_OPTIONS);

        //levelCompletedCanvas.enabled = (currentGameState == GameState.GS_LEVELCOMPLETED);
    }

    public void PauseMenu()
    {
        Time.timeScale = 0;
        SetGameState(GameState.GS_PAUSEMENU);
    }

    public void InGame()
    {
        Time.timeScale = 1;
        SetGameState(GameState.GS_GAME);

    }

    public void LevelCompleted()
    {
        SetGameState(GameState.GS_LEVELCOMPLETED);
    }

    public void GameOver()
    {
        SetGameState(GameState.GS_GAME_OVER);
    }

    public void Options()
    {
        qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
        //Debug.Log("Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
        Time.timeScale = 0;
        SetGameState(GameState.GS_OPTIONS);
    } 

    public void AddPoints(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    public void AddKeys(int i)
    {
        keysTab[i].color = Color.white;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void UpdateEnemiesUI()
    {
        enemiesSlayed++;
        if (enemyText != null)
        {
            enemyText.text = enemiesSlayed.ToString();
        }
    }
    private void UpdateTime()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        if (timeText != null)
        {
            // timeText.text = timer.ToString();
            // string timeText = string.Format("{0:00}:{1:00}", minutes, seconds);
            Debug.Log(timeText);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    public void AddHeart(int i)
    {
        heartsTab[i].color = Color.white;
    }
    public void RemoveHeart(int i)
    {
        heartsTab[i].color = Color.clear;
    }

    public void OnResumeButtonClicked()
    {
        InGame();
        //SimulateMouseClick();
    }
    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnReturnToMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void IncreaseGraphics()
    {
        QualitySettings.IncreaseLevel();
        qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
    }
    public void DecreaseGraphics()
    {
        QualitySettings.DecreaseLevel();
        qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
    }
    void SimulateMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the ray hit an object
            GameObject hitObject = hit.collider.gameObject;

            // You can perform actions based on the hit object here
            Debug.Log("Mouse clicked on: " + hitObject.name);
        }
    }

    public void SetVolume()
    {
        float volume = VolumeSlider.value;
        AudioListener.volume = volume;
        Debug.Log("VOLUME SET: " + volume);
    }
}