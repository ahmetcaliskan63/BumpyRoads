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
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private GameObject fuelBarContainer;

    [Header("Sistem Referansları")]
    [SerializeField] private CrashSystem crashSystem;

    [Header("Ayarlar")]
    [SerializeField] private float showDelay = 1f;
    [SerializeField] private float autoReturnToMenuDelay = 5f; // Otomatik ana menüye dönme süresi (saniye)
    [SerializeField] private bool autoReturnToMenu = true; // Otomatik dönüş açık/kapalı

    private bool isGameOver = false;

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Buton referanslarını kontrol et ve ayarla
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            Debug.Log("GameOverUI: Restart butonu listener eklendi.");
        }
        else
        {
            Debug.LogError("GameOverUI: restartButton referansı NULL! Unity Editor'da atanmamış!");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
            Debug.Log("GameOverUI: Main Menu butonu listener eklendi.");
        }
        else
        {
            Debug.LogError("GameOverUI: mainMenuButton referansı NULL! Unity Editor'da atanmamış!");
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

        ShowGameOver();
    }

    private void OnFuelEmpty()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameOver(true);
        }

        ShowGameOver();
    }

    private void ShowGameOver()
    {
        if (gameOverText != null)
        {
            gameOverText.text = "OYUN BİTTİ!";
        }

        if (coinsText != null)
        {
            int coins = CoinManager.Instance != null ? CoinManager.Instance.CurrentCoins : 0;
            coinsText.text = coins.ToString(); // Sadece sayı (icon yanında)
        }

        if (distanceText != null)
        {
            float distance = DistanceManager.Instance != null ? DistanceManager.Instance.CurrentDistance : 0f;
            distanceText.text = $"{distance:F1} m"; // Sadece sayı ve birim (icon yanında)
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

        if (fuelBarContainer != null)
        {
            fuelBarContainer.SetActive(true);
        }

        // Otomatik olarak ana menüye dön
        if (autoReturnToMenu)
        {
            Invoke(nameof(GoToMainMenu), autoReturnToMenuDelay);
        }
    }

    private void RestartGame()
    {
        Debug.Log("RestartGame() çağrıldı!");
        
        // Otomatik ana menüye dönme işlemini iptal et
        CancelInvoke(nameof(GoToMainMenu));
        
        // Tüm Invoke'ları iptal et
        CancelInvoke();
        
        // Game over durumunu sıfırla
        isGameOver = false;
        
        // Sistemleri sıfırla
        if (FuelManager.Instance != null)
        {
            FuelManager.Instance.ResetFuel();
            Debug.Log("FuelManager sıfırlandı.");
        }
        
        if (DistanceManager.Instance != null)
        {
            DistanceManager.Instance.ResetDistance();
            Debug.Log("DistanceManager sıfırlandı.");
        }
        
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.ResetRunCoins();
            Debug.Log("CoinManager sıfırlandı.");
        }
        
        // Game over panelini kapat
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("Game Over paneli kapatıldı.");
        }
        
        // GameManager ile restart yap
        if (GameManager.Instance != null)
        {
            Debug.Log("GameManager.Instance bulundu, RestartLevel() çağrılıyor...");
            GameManager.Instance.RestartLevel();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance NULL! Fallback: Direkt sahne yeniden yükleniyor...");
            // Fallback: Direkt sahne yeniden yükle
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    private void GoToMainMenu()
    {
        Debug.Log("GoToMainMenu() çağrıldı!");
        
        // Otomatik ana menüye dönme işlemini iptal et
        CancelInvoke(nameof(GoToMainMenu));
        
        if (GameManager.Instance != null)
        {
            Debug.Log("GameManager.Instance bulundu, LoadMainMenu() çağrılıyor...");
            GameManager.Instance.LoadMainMenu();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance NULL! Fallback: MainMenu sahnesi yükleniyor...");
            // Fallback: Direkt MainMenu sahnesini yükle
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}

