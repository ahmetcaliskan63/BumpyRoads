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
    [SerializeField] private Sprite[] levelPreviewSprites; // UI görseli için

    [Header("Satın Alma Penceresi")]
    [SerializeField] private GameObject purchasePanel;
    [SerializeField] private TextMeshProUGUI purchaseLevelNameText;
    [SerializeField] private Image purchaseLevelImage;
    [SerializeField] private TextMeshProUGUI purchasePriceText;
    [SerializeField] private TextMeshProUGUI purchasePlayerCoinsText;
    [SerializeField] private Button purchaseBuyButton;
    [SerializeField] private Button purchaseBackButton;
    [SerializeField] private Color buyButtonColor = new Color(0.2f, 0.8f, 0.2f);   // yeşil
    [SerializeField] private Color backButtonColor = new Color(0.8f, 0.2f, 0.2f);  // kırmızı

    [Header("Seviye Fiyatları")]
    [SerializeField] private int level2Price = 50;
    [SerializeField] private int level3Price = 100;
    [SerializeField] private string[] levelDisplayNames = { "Level 1", "Level 2", "Level 3" };

    [Header("Oyun Başlığı")]
    [SerializeField] private TextMeshProUGUI gameTitleText;

    private LevelManager levelManager;
    private int pendingPurchaseLevel = -1;

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
            level2Button.onClick.AddListener(() => HandleLevelButton(2));

        if (level3Button != null)
            level3Button.onClick.AddListener(() => HandleLevelButton(3));

        if (backToMainMenuButton != null)
            backToMainMenuButton.onClick.AddListener(OnBackToMainMenuClicked);

        if (purchaseBuyButton != null)
            purchaseBuyButton.onClick.AddListener(OnPurchaseBuyClicked);

        if (purchaseBackButton != null)
            purchaseBackButton.onClick.AddListener(ClosePurchasePanel);

        // Panel başlangıçta kapalı
        if (purchasePanel != null)
            purchasePanel.SetActive(false);

        // Buton renklerini uygula (sahnede atandıysa override etmez)
        if (purchaseBuyButton != null && purchaseBuyButton.image != null)
            purchaseBuyButton.image.color = buyButtonColor;
        if (purchaseBackButton != null && purchaseBackButton.image != null)
            purchaseBackButton.image.color = backButtonColor;

        // Kilit ikonları tıklamayı engellemesin
        DisableLockIconRaycast(level1LockIcon);
        DisableLockIconRaycast(level2LockIcon);
        DisableLockIconRaycast(level3LockIcon);

        // Kasa bilgisini dinle
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnTotalCoinsChanged += UpdatePlayerCoinsUI;
            UpdatePlayerCoinsUI(CoinManager.Instance.TotalCoins);
        }
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

    private void HandleLevelButton(int levelNumber)
    {
        if (levelManager == null)
        {
            LoadLevel(levelNumber);
            return;
        }

        bool unlocked = levelManager.IsLevelUnlocked(levelNumber);
        if (unlocked)
        {
            LoadLevel(levelNumber);
            return;
        }

        // Kilitliyse satın alma penceresini aç
        int price = GetLevelPrice(levelNumber);
        if (price > 0)
        {
            OpenPurchasePanel(levelNumber, price);
        }
        else
        {
            Debug.LogWarning($"Level {levelNumber} için fiyat tanımlı değil.");
        }
    }

    private int GetLevelPrice(int levelNumber)
    {
        return levelNumber switch
        {
            2 => level2Price,
            3 => level3Price,
            _ => 0
        };
    }

    private void OpenPurchasePanel(int levelNumber, int price)
    {
        pendingPurchaseLevel = levelNumber;

        if (purchasePriceText != null)
        {
            purchasePriceText.text = $"{price} Altın";
        }

        if (purchasePanel != null)
        {
            purchasePanel.SetActive(true);
        }

        // Level adı ve görseli
        if (purchaseLevelNameText != null && levelNumber - 1 >= 0 && levelNumber - 1 < levelDisplayNames.Length)
        {
            purchaseLevelNameText.text = levelDisplayNames[levelNumber - 1];
        }

        if (purchaseLevelImage != null && levelPreviewSprites != null &&
            levelNumber - 1 >= 0 && levelNumber - 1 < levelPreviewSprites.Length)
        {
            purchaseLevelImage.sprite = levelPreviewSprites[levelNumber - 1];
            purchaseLevelImage.preserveAspect = true;
        }

        // Oyuncu toplam coin bilgisi
        if (CoinManager.Instance != null && purchasePlayerCoinsText != null)
        {
            purchasePlayerCoinsText.text = CoinManager.Instance.TotalCoins.ToString();
        }
    }

    private void ClosePurchasePanel()
    {
        pendingPurchaseLevel = -1;
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(false);
        }
    }

    private void OnPurchaseBuyClicked()
    {
        if (pendingPurchaseLevel < 0) return;

        int price = GetLevelPrice(pendingPurchaseLevel);
        if (price <= 0)
        {
            Debug.LogWarning("Geçersiz fiyat, satın alma iptal.");
            ClosePurchasePanel();
            return;
        }

        if (CoinManager.Instance == null)
        {
            Debug.LogWarning("CoinManager bulunamadı, satın alma yapılamıyor.");
            ClosePurchasePanel();
            return;
        }

        bool paid = CoinManager.Instance.SpendCoins(price);
        if (!paid)
        {
            Debug.LogWarning("Yeterli altın yok!");
            return;
        }

        // Satın alma başarılı, level kilidini aç
        if (levelManager != null)
        {
            levelManager.UnlockLevel(pendingPurchaseLevel);
        }

        UpdateLevelButtons();
        ClosePurchasePanel();
    }

    private void UpdatePlayerCoinsUI(int totalCoins)
    {
        if (purchasePlayerCoinsText != null)
        {
            purchasePlayerCoinsText.text = totalCoins.ToString();
        }
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
            level1Button.interactable = true; // Her zaman tıklanabilir

        if (level1LockIcon != null)
            level1LockIcon.SetActive(true); // Her zaman açık kalsın, kaybolmasın

        if (level1Text != null)
            level1Text.text = level1Unlocked ? "Level 1" : "Kilitli";

        // Level 2
        if (level2Button != null)
            level2Button.interactable = true; // Kilitliyken de tıklanıp satın alma açılsın

        if (level2LockIcon != null)
            level2LockIcon.SetActive(!level2Unlocked); // Satın alındıysa gizle

        if (level2Text != null)
            level2Text.text = level2Unlocked ? "Level 2" : "Kilitli";

        // Level 3
        if (level3Button != null)
            level3Button.interactable = true; // Kilitliyken de tıklanıp satın alma açılsın

        if (level3LockIcon != null)
            level3LockIcon.SetActive(!level3Unlocked); // Satın alındıysa gizle

        if (level3Text != null)
            level3Text.text = level3Unlocked ? "Level 3" : "Kilitli";
    }

    private void DisableLockIconRaycast(GameObject icon)
    {
        if (icon == null) return;
        var img = icon.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.raycastTarget = false; // ikon tıklamayı bloklamasın
        }
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

        if (purchaseBuyButton != null)
            purchaseBuyButton.onClick.RemoveAllListeners();

        if (purchaseBackButton != null)
            purchaseBackButton.onClick.RemoveAllListeners();
    }
}

