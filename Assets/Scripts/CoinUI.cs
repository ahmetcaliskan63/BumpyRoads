using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject coinIcon;

    private void Start()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged += UpdateCoinUI;
            UpdateCoinUI(CoinManager.Instance.CurrentCoins);
        }
    }

    private void OnDestroy()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged -= UpdateCoinUI;
        }
    }

    private void UpdateCoinUI(int currentCoins)
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }
}

