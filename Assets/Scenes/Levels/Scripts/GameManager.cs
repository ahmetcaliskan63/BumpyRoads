using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Sahne Ayarları")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float gameOverDelay = 1f;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void RestartLevel()
    {
        // Game over olsa bile restart yapılabilir
        isGameOver = false; // Restart yaparken game over durumunu sıfırla
        
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void LoadMainMenu()
    {
        // Game over olsa bile ana menüye dönebilir
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void SetGameOver(bool gameOver)
    {
        isGameOver = gameOver;
    }
}

