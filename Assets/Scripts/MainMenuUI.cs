using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panelleri")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;

    [Header("Ana Menü Butonları")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Button quitButton;

    [Header("Level Seçim Butonları")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button backToMainMenuButton;

    [Header("Level Butonları UI")]
    [SerializeField] private TextMeshProUGUI level1Text;
    [SerializeField] private TextMeshProUGUI level2Text;
    [SerializeField] private TextMeshProUGUI level3Text;
    [SerializeField] private GameObject level1LockIcon;
    [SerializeField] private GameObject level2LockIcon;
    [SerializeField] private GameObject level3LockIcon;

    [Header("Oyun Başlığı")]
    [SerializeField] private TextMeshProUGUI gameTitleText;

    private LevelManager levelManager;

    private void Start()
    {
        // LevelManager'ı bul veya oluştur
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            GameObject levelManagerObj = new GameObject("LevelManager");
            levelManager = levelManagerObj.AddComponent<LevelManager>();
        }

        // Panelleri başlangıç durumuna getir
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);

        // Buton event'lerini ayarla
        SetupButtons();

        // Level durumlarını güncelle
        UpdateLevelButtons();
    }

    private void SetupButtons()
    {
        // Ana menü butonları
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);

        if (levelSelectButton != null)
            levelSelectButton.onClick.AddListener(OnLevelSelectButtonClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);

        // Level seçim butonları
        if (level1Button != null)
            level1Button.onClick.AddListener(() => LoadLevel(1));

        if (level2Button != null)
            level2Button.onClick.AddListener(() => LoadLevel(2));

        if (level3Button != null)
            level3Button.onClick.AddListener(() => LoadLevel(3));

        if (backToMainMenuButton != null)
            backToMainMenuButton.onClick.AddListener(OnBackToMainMenuClicked);
    }

    private void OnPlayButtonClicked()
    {
        // İlk açık level'i yükle (Level 1)
        LoadLevel(1);
    }

    private void OnLevelSelectButtonClicked()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
            UpdateLevelButtons();
        }
    }

    private void OnBackToMainMenuClicked()
    {
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    private void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void LoadLevel(int levelNumber)
    {
        if (levelManager != null)
        {
            levelManager.LoadLevel(levelNumber);
        }
        else
        {
            // Fallback: Direkt sahne adıyla yükle
            string sceneName = "Level" + levelNumber;
            SceneManager.LoadScene(sceneName);
        }
    }

    private void UpdateLevelButtons()
    {
        if (levelManager == null) return;

        // Level 1 her zaman açık
        bool level1Unlocked = true;
        bool level2Unlocked = levelManager.IsLevelUnlocked(2);
        bool level3Unlocked = levelManager.IsLevelUnlocked(3);

        // Level 1
        if (level1Button != null)
            level1Button.interactable = level1Unlocked;

        if (level1LockIcon != null)
            level1LockIcon.SetActive(!level1Unlocked);

        if (level1Text != null)
            level1Text.text = level1Unlocked ? "Level 1" : "Kilitli";

        // Level 2
        if (level2Button != null)
            level2Button.interactable = level2Unlocked;

        if (level2LockIcon != null)
            level2LockIcon.SetActive(!level2Unlocked);

        if (level2Text != null)
            level2Text.text = level2Unlocked ? "Level 2" : "Kilitli";

        // Level 3
        if (level3Button != null)
            level3Button.interactable = level3Unlocked;

        if (level3LockIcon != null)
            level3LockIcon.SetActive(!level3Unlocked);

        if (level3Text != null)
            level3Text.text = level3Unlocked ? "Level 3" : "Kilitli";
    }

    private void OnDestroy()
    {
        // Event listener'ları temizle
        if (playButton != null)
            playButton.onClick.RemoveAllListeners();

        if (levelSelectButton != null)
            levelSelectButton.onClick.RemoveAllListeners();

        if (quitButton != null)
            quitButton.onClick.RemoveAllListeners();

        if (level1Button != null)
            level1Button.onClick.RemoveAllListeners();

        if (level2Button != null)
            level2Button.onClick.RemoveAllListeners();

        if (level3Button != null)
            level3Button.onClick.RemoveAllListeners();

        if (backToMainMenuButton != null)
            backToMainMenuButton.onClick.RemoveAllListeners();
    }
}

