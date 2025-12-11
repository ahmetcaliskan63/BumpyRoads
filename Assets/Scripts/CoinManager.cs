using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("Altın Ayarları")]
    [SerializeField] private int currentCoins = 0;
    [SerializeField] private int coinsPerCollectible = 5;

    private const string CoinsPrefKey = "TotalCoins";

    public int CurrentCoins => currentCoins;

    public System.Action<int> OnCoinsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoinsFromPrefs();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        OnCoinsChanged?.Invoke(currentCoins);
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        SaveCoinsToPrefs();
        OnCoinsChanged?.Invoke(currentCoins);
    }

    public void CollectCoin()
    {
        AddCoins(coinsPerCollectible);
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            SaveCoinsToPrefs();
            OnCoinsChanged?.Invoke(currentCoins);
            return true;
        }
        return false;
    }

    public void ResetCoins()
    {
        currentCoins = 0;
        SaveCoinsToPrefs();
        OnCoinsChanged?.Invoke(currentCoins);
    }

    private void LoadCoinsFromPrefs()
    {
        currentCoins = PlayerPrefs.GetInt(CoinsPrefKey, 0);
    }

    private void SaveCoinsToPrefs()
    {
        PlayerPrefs.SetInt(CoinsPrefKey, currentCoins);
        PlayerPrefs.Save();
    }
}

