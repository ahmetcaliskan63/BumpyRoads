using UnityEngine;

/// <summary>
/// Level içi geçici coinler için manager. Her level başında 0'dan başlar.
/// </summary>
public class LevelCoinManager : MonoBehaviour
{
    public static LevelCoinManager Instance { get; private set; }

    [Header("Altın Ayarları")]
    [SerializeField] private int currentCoins = 0;
    [SerializeField] private int coinsPerCollectible = 5;

    public int CurrentCoins => currentCoins;

    public System.Action<int> OnCoinsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad YOK - her level'de yeni instance
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Level başında coinleri sıfırla
        ResetCoins();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
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
            OnCoinsChanged?.Invoke(currentCoins);
            return true;
        }
        return false;
    }

    public void ResetCoins()
    {
        currentCoins = 0;
        OnCoinsChanged?.Invoke(currentCoins);
    }
}

