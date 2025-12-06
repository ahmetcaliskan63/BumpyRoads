using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [Header("Sistem Referansları")]
    [SerializeField] private CrashSystem crashSystem;

    [Header("Ayarlar")]
    [SerializeField] private string crashText = "KAZA!";
    [SerializeField] private string fuelEmptyText = "BENZİN BİTTİ!";
    [SerializeField] private float showDelay = 1f;

    private bool isGameOver = false;

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        if (crashSystem == null)
        {
            crashSystem = FindObjectOfType<CrashSystem>();
        }

        if (crashSystem != null)
        {
            crashSystem.OnCrash += OnCrashDetected;
        }

        if (FuelManager.Instance != null)
        {
            FuelManager.Instance.OnFuelEmpty += OnFuelEmpty;
        }
    }

    private void OnDestroy()
    {
        if (crashSystem != null)
        {
            crashSystem.OnCrash -= OnCrashDetected;
        }

        if (FuelManager.Instance != null)
        {
            FuelManager.Instance.OnFuelEmpty -= OnFuelEmpty;
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }
    }

    private void OnCrashDetected()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameOver(true);
        }

        ShowGameOver(crashText);
    }

    private void OnFuelEmpty()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameOver(true);
        }

        ShowGameOver(fuelEmptyText);
    }

    private void ShowGameOver(string text)
    {
        if (gameOverText != null)
        {
            gameOverText.text = text;
        }

        if (gameOverPanel != null)
        {
            Invoke(nameof(ActivatePanel), showDelay);
        }
    }

    private void ActivatePanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
    }

    private void GoToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
    }
}

