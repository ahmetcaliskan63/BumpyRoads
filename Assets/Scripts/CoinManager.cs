using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("Altın Ayarları")]
    [SerializeField] private int currentCoins = 0; // Anlık (run içi) altın
    [SerializeField] private int totalCoins = 0;   // Kalıcı kasa altını
    [SerializeField] private int coinsPerCollectible = 5;

    private const string CoinsPrefKey = "TotalCoins";

    public int CurrentCoins => currentCoins; // run içi
    public int TotalCoins => totalCoins;     // kasa

    public System.Action<int> OnCoinsChanged;        // run içi güncelleme
    public System.Action<int> OnTotalCoinsChanged;   // kasa güncelleme

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoinsFromPrefs(); // totalCoins yüklenir
            currentCoins = 0;     // run başlarken sıfır
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Başlangıçta her iki değeri de yayınla
        OnCoinsChanged?.Invoke(currentCoins);
        OnTotalCoinsChanged?.Invoke(totalCoins);
    }

    // Run içi altın ekle (UI için)
    private void AddRunCoins(int amount)
    {
        currentCoins += amount;
        OnCoinsChanged?.Invoke(currentCoins);
    }

    // Kasa altın ekle/çıkar (kalıcı)
    private void AddTotalCoins(int amount)
    {
        totalCoins += amount;
        SaveCoinsToPrefs();
        OnTotalCoinsChanged?.Invoke(totalCoins);
    }

    public void CollectCoin()
    {
        AddRunCoins(coinsPerCollectible);
        AddTotalCoins(coinsPerCollectible);
    }

    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            SaveCoinsToPrefs();
            OnTotalCoinsChanged?.Invoke(totalCoins);
            return true;
        }
        return false;
    }

    public void ResetCoins()
    {
        currentCoins = 0; // run içi
        OnCoinsChanged?.Invoke(currentCoins);

        totalCoins = 0;   // kasa
        SaveCoinsToPrefs();
        OnTotalCoinsChanged?.Invoke(totalCoins);
    }

    public void ResetRunCoins()
    {
        currentCoins = 0;
        OnCoinsChanged?.Invoke(currentCoins);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Oyun sahnelerine girerken run içi altını sıfırla (MainMenu hariç)
        if (!scene.name.Equals("MainMenu"))
        {
            ResetRunCoins();
        }

        // Sahne değişiminde kasayı UI'lara yeniden bildir (MainMenu dahil)
        OnTotalCoinsChanged?.Invoke(totalCoins);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void LoadCoinsFromPrefs()
    {
        totalCoins = PlayerPrefs.GetInt(CoinsPrefKey, 0);
    }

    private void SaveCoinsToPrefs()
    {
        PlayerPrefs.SetInt(CoinsPrefKey, totalCoins);
        PlayerPrefs.Save();
    }
}

