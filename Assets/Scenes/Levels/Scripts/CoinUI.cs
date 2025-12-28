using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject coinIcon;

    [Header("Gösterim")]
    [SerializeField] private bool showTotalCoins = false; // true: kasa, false: run içi

    private bool isSubscribed = false;

    private void OnEnable()
    {
        TrySubscribe();
        UpdateIconMode();
    }

    private void Start()
    {
        TrySubscribe(); // Start anında da dene
        UpdateIconMode();
    }

    private void OnDisable()
    {
        if (CoinManager.Instance != null)
        {
            if (showTotalCoins)
            {
                CoinManager.Instance.OnTotalCoinsChanged -= UpdateCoinUIWithTotal;
            }
            else
            {
                CoinManager.Instance.OnCoinsChanged -= UpdateCoinUI;
            }
        }

        isSubscribed = false;
    }

    private void UpdateCoinUI(int currentCoins)
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }

    private void UpdateCoinUIWithTotal(int totalCoins)
    {
        if (coinText != null)
        {
            coinText.text = totalCoins.ToString();
        }
    }

    private void UpdateIconMode()
    {
        if (coinIcon == null) return;

        var image = coinIcon.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            // Kasa görünümünde nötr, run içi görünümde hafif sıcak ton
            image.color = showTotalCoins ? Color.white : new Color(1f, 0.95f, 0.3f);
        }

        coinIcon.SetActive(true); // İkonu kullan, gizli kalmasın
    }

    private void TrySubscribe()
    {
        if (isSubscribed) return;
        // CoinManager sahnede yoksa bulmaya çalış, yoksa otomatik oluştur
        if (CoinManager.Instance == null)
        {
            var found = FindObjectOfType<CoinManager>();
            if (found == null)
            {
                var go = new GameObject("CoinManager (Auto)");
                go.AddComponent<CoinManager>();
            }
        }
        if (CoinManager.Instance == null) return;

        if (showTotalCoins)
        {
            CoinManager.Instance.OnTotalCoinsChanged += UpdateCoinUIWithTotal;
            UpdateCoinUIWithTotal(CoinManager.Instance.TotalCoins);
        }
        else
        {
            CoinManager.Instance.OnCoinsChanged += UpdateCoinUI;
            UpdateCoinUI(CoinManager.Instance.CurrentCoins);
        }

        isSubscribed = true;
    }
}

