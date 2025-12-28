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
    [SerializeField] private GameObject level2LockIconkilit; // Level 2 kilit ikonu (LevelSelectPanel altında)
    [SerializeField] private GameObject level3LockIconkilit; // Level 3 kilit ikonu (LevelSelectPanel altında)
    [SerializeField] private Sprite[] levelPreviewSprites; // UI görseli için

    [Header("Satın Alma Penceresi")]
    [SerializeField] private GameObject purchasePanel;
    [SerializeField] private GameObject purchaseCard; // Ortalanmış kart
    [SerializeField] private TextMeshProUGUI purchaseLevelNameText; // Level adı (görselin üstünde)
    [SerializeField] private Image purchaseLevelImage; // Level görseli (ortada)
    [SerializeField] private Image purchaseCoinIcon; // Coin icon (üstte solda)
    [SerializeField] private TextMeshProUGUI purchasePriceText; // Fiyat metni (coin icon yanında)
    [SerializeField] private Button purchaseBuyButton; // Satın Al butonu (alta sağda)
    [SerializeField] private Button purchaseBackButton; // Geri butonu (alta solda)
    [SerializeField] private Color buyButtonColor = new Color(0.2f, 0.8f, 0.2f);   // yeşil
    [SerializeField] private Color backButtonColor = new Color(0.8f, 0.2f, 0.2f);  // kırmızı

    [Header("Seviye Fiyatları ve Görselleri")]
    [SerializeField] private int level2Price = 50;
    [SerializeField] private int level3Price = 100;
    [SerializeField] private string[] levelDisplayNames = { "Level 1", "Level 2 Çöl", "Level 3 Yanar Dağ" };
    [SerializeField] private Sprite[] levelFrameSprites; // Level çerçeve görselleri (level2Cerceve, level3Cercevesi)

    [Header("Oyun Başlığı")]
    [SerializeField] private TextMeshProUGUI gameTitleText;

    [Header("Hazırlayanlar Metni")]
    [SerializeField] private TextMeshProUGUI creditsText; // Alt köşede gösterilecek hazırlayanlar metni

    private LevelManager levelManager;
    private int pendingPurchaseLevel = -1;

    private void Start()
    {
        // AudioManager'ı kontrol et ve müziği başlat
        InitializeAudioManager();

        // LevelManager'ı bul veya oluştur
        levelManager = LevelManager.Instance != null ? LevelManager.Instance : FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            GameObject levelManagerObj = new GameObject("LevelManager");
            levelManager = levelManagerObj.AddComponent<LevelManager>();
        }

        Debug.Log($"MainMenuUI Start - levelManager bulundu: {levelManager != null}");

        // Eğer referanslar atanmamışsa, otomatik bul
        if (level2LockIconkilit == null && levelSelectPanel != null)
        {
            // LevelSelectPanel altında Level2Locklconkilit veya Level2LockIconkilit ara
            level2LockIconkilit = FindChildInPanel("Level2Locklconkilit") ?? 
                                  FindChildInPanel("Level2LockIconkilit");
            
            // Eğer hala bulunamadıysa, tüm child'ları kontrol et
            if (level2LockIconkilit == null)
            {
                for (int i = 0; i < levelSelectPanel.transform.childCount; i++)
                {
                    Transform t = levelSelectPanel.transform.GetChild(i);
                    if (t.name.Contains("Level2") && t.name.Contains("Lock") && t.name.Contains("kilit"))
                    {
                        level2LockIconkilit = t.gameObject;
                        Debug.Log($"Level 2 Lock Icon bulundu (isim farklı): {t.name}");
                        break;
                    }
                }
            }
            
            if (level2LockIconkilit != null)
            {
                Debug.Log($"Level 2 Lock Icon otomatik bulundu: {level2LockIconkilit.name}");
            }
            else
            {
                Debug.LogError("Level 2 Lock Icon bulunamadı! LevelSelectPanel altında 'Level2Locklconkilit' veya 'Level2LockIconkilit' isimli GameObject olmalı!");
            }
        }

        if (level3LockIconkilit == null && levelSelectPanel != null)
        {
            // LevelSelectPanel altında Level3Lockiconkilit veya Level3LockIconkilit ara
            level3LockIconkilit = FindChildInPanel("Level3Lockiconkilit") ?? 
                                  FindChildInPanel("Level3LockIconkilit");
            
            // Eğer hala bulunamadıysa, tüm child'ları kontrol et
            if (level3LockIconkilit == null)
            {
                for (int i = 0; i < levelSelectPanel.transform.childCount; i++)
                {
                    Transform t = levelSelectPanel.transform.GetChild(i);
                    if (t.name.Contains("Level3") && t.name.Contains("Lock") && t.name.Contains("kilit"))
                    {
                        level3LockIconkilit = t.gameObject;
                        Debug.Log($"Level 3 Lock Icon bulundu (isim farklı): {t.name}");
                        break;
                    }
                }
            }
            
            if (level3LockIconkilit != null)
            {
                Debug.Log($"Level 3 Lock Icon otomatik bulundu: {level3LockIconkilit.name}");
            }
            else
            {
                Debug.LogError("Level 3 Lock Icon bulunamadı! LevelSelectPanel altında 'Level3Lockiconkilit' veya 'Level3LockIconkilit' isimli GameObject olmalı!");
            }
        }

        // Referans kontrolü (final)
        if (level2LockIconkilit == null)
        {
            Debug.LogError("MainMenuUI Start: level2LockIconkilit referansı HALA NULL! LevelSelectPanel altında GameObject bulunamadı!");
        }
        else
        {
            Debug.Log($"MainMenuUI Start: level2LockIconkilit bulundu - İsim: {level2LockIconkilit.name}, Aktif: {level2LockIconkilit.activeSelf}");
        }

        if (level3LockIconkilit == null)
        {
            Debug.LogError("MainMenuUI Start: level3LockIconkilit referansı HALA NULL! LevelSelectPanel altında GameObject bulunamadı!");
        }
        else
        {
            Debug.Log($"MainMenuUI Start: level3LockIconkilit bulundu - İsim: {level3LockIconkilit.name}, Aktif: {level3LockIconkilit.activeSelf}");
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

        // Hazırlayanlar metnini ayarla
        SetupCreditsText();
    }

    /// <summary>
    /// AudioManager'ı başlatır ve ana menü müziğini çalar
    /// </summary>
    private void InitializeAudioManager()
    {
        // AudioManager'ı bul veya oluştur
        if (AudioManager.Instance == null)
        {
            GameObject audioManagerObj = new GameObject("AudioManager");
            audioManagerObj.AddComponent<AudioManager>();
            Debug.Log("AudioManager oluşturuldu.");
        }

        // Ana menü müziğini başlat
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
            Debug.Log("Ana menü müziği başlatıldı.");
        }
        else
        {
            Debug.LogWarning("AudioManager bulunamadı veya oluşturulamadı!");
        }
    }

    // LevelSelectPanel altında child GameObject bul
    private GameObject FindChildInPanel(string childName)
    {
        if (levelSelectPanel == null) return null;

        // Direkt child olarak ara
        Transform child = levelSelectPanel.transform.Find(childName);
        if (child != null)
        {
            return child.gameObject;
        }

        return null;
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
        if (purchaseCard != null)
            purchaseCard.SetActive(false);

        // Buton renklerini uygula (sahnede atandıysa override etmez)
        if (purchaseBuyButton != null && purchaseBuyButton.image != null)
            purchaseBuyButton.image.color = buyButtonColor;
        if (purchaseBackButton != null && purchaseBackButton.image != null)
            purchaseBackButton.image.color = backButtonColor;

        // Kilit ikonları tıklamayı engellemesin
        DisableLockIconRaycast(level2LockIconkilit);
        DisableLockIconRaycast(level3LockIconkilit);

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

        // Kartı göster
        if (purchaseCard != null)
        {
            purchaseCard.SetActive(true);
        }

        // Panel'i de aktif et (eğer varsa)
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(true);
        }

        // Level adı (görselin üstünde)
        if (purchaseLevelNameText != null && levelNumber - 1 >= 0 && levelNumber - 1 < levelDisplayNames.Length)
        {
            purchaseLevelNameText.text = levelDisplayNames[levelNumber - 1];
        }

        // Level görseli (ortada) - levelFrameSprites kullan
        if (purchaseLevelImage != null)
        {
            // Level 2 için index 0, Level 3 için index 1
            int spriteIndex = levelNumber - 2; // Level 2 -> 0, Level 3 -> 1
            if (levelFrameSprites != null && spriteIndex >= 0 && spriteIndex < levelFrameSprites.Length)
            {
                purchaseLevelImage.sprite = levelFrameSprites[spriteIndex];
                purchaseLevelImage.preserveAspect = true;
            }
            else if (levelPreviewSprites != null && levelNumber - 1 >= 0 && levelNumber - 1 < levelPreviewSprites.Length)
            {
                // Fallback: eski sistem
                purchaseLevelImage.sprite = levelPreviewSprites[levelNumber - 1];
                purchaseLevelImage.preserveAspect = true;
            }
        }

        // Fiyat metni (coin icon yanında)
        if (purchasePriceText != null)
        {
            purchasePriceText.text = price.ToString();
        }

        // Coin icon'u göster (eğer atanmışsa)
        if (purchaseCoinIcon != null)
        {
            purchaseCoinIcon.gameObject.SetActive(true);
        }
    }

    private void ClosePurchasePanel()
    {
        pendingPurchaseLevel = -1;
        
        // Tüm purchase UI elementlerini gizle
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(false);
        }
        if (purchaseCard != null)
        {
            purchaseCard.SetActive(false);
        }
    }

    private void OnPurchaseBuyClicked()
    {
        if (pendingPurchaseLevel < 0)
        {
            Debug.LogWarning("OnPurchaseBuyClicked: pendingPurchaseLevel geçersiz!");
            return;
        }

        Debug.Log($"OnPurchaseBuyClicked çağrıldı - Level: {pendingPurchaseLevel}");

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
            Debug.Log($"Level {pendingPurchaseLevel} açılıyor...");
            levelManager.UnlockLevel(pendingPurchaseLevel);
            
            // Kontrol et
            bool isUnlocked = levelManager.IsLevelUnlocked(pendingPurchaseLevel);
            Debug.Log($"Level {pendingPurchaseLevel} açıldı mı? {isUnlocked}");
        }
        else
        {
            Debug.LogError("levelManager NULL! Level açılamıyor!");
        }

        Debug.Log("UpdateLevelButtons çağrılıyor...");
        UpdateLevelButtons();
        ClosePurchasePanel();
    }

    private void UpdatePlayerCoinsUI(int totalCoins)
    {
        // Bu metod artık kullanılmıyor ama event listener için gerekli
        // Coin bilgisi artık purchase panel'de gösterilmiyor
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
        if (levelManager == null)
        {
            Debug.LogWarning("UpdateLevelButtons: levelManager null!");
            return;
        }

        // Level 1 her zaman açık
        bool level1Unlocked = true;
        bool level2Unlocked = levelManager.IsLevelUnlocked(2);
        bool level3Unlocked = levelManager.IsLevelUnlocked(3);

        Debug.Log($"UpdateLevelButtons çağrıldı - Level 2: {(level2Unlocked ? "Açık" : "Kilitli")}, Level 3: {(level3Unlocked ? "Açık" : "Kilitli")}");

        // Level 1
        if (level1Button != null)
            level1Button.interactable = true; // Her zaman tıklanabilir

        if (level1Text != null)
            level1Text.text = level1Unlocked ? "Level 1" : "Kilitli";

        // Level 2
        if (level2Button != null)
            level2Button.interactable = true; // Kilitliyken de tıklanıp satın alma açılsın

        // Level 2 kilit ikonu (LevelSelectPanel altında direkt GameObject)
        if (level2LockIconkilit != null)
        {
            level2LockIconkilit.SetActive(!level2Unlocked); // Açıksa gizle, kilitliyse göster
            Debug.Log($"Level 2 Lock Icon ({level2LockIconkilit.name}): {(level2Unlocked ? "GİZLENDİ" : "GÖSTERİLDİ")} - Aktif: {level2LockIconkilit.activeSelf}");
        }
        else
        {
            Debug.LogError("level2LockIconkilit referansı NULL! Unity Editor'da atanmamış olabilir.");
        }

        if (level2Text != null)
            level2Text.text = level2Unlocked ? "Level 2" : "Kilitli";

        // Level 3
        if (level3Button != null)
            level3Button.interactable = true; // Kilitliyken de tıklanıp satın alma açılsın

        // Level 3 kilit ikonu (LevelSelectPanel altında direkt GameObject)
        if (level3LockIconkilit != null)
        {
            level3LockIconkilit.SetActive(!level3Unlocked); // Açıksa gizle, kilitliyse göster
            Debug.Log($"Level 3 Lock Icon ({level3LockIconkilit.name}): {(level3Unlocked ? "GİZLENDİ" : "GÖSTERİLDİ")} - Aktif: {level3LockIconkilit.activeSelf}");
        }
        else
        {
            Debug.LogError("level3LockIconkilit referansı NULL! Unity Editor'da atanmamış olabilir.");
        }

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

    /// <summary>
    /// Hazırlayanlar metnini ayarlar (alt köşede gösterilir)
    /// </summary>
    private void SetupCreditsText()
    {
        if (creditsText != null)
        {
            creditsText.text = "Hazırlayanlar: Enes Yenigün (1220505062), Ahmet Çalışkan (1220505025)";
        }
        else
        {
            Debug.LogWarning("MainMenuUI: creditsText atanmamış! Unity Editor'da alt köşeye bir TextMeshProUGUI ekleyip Inspector'dan atayın.");
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

