using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private Image fuelBarFill;
    [SerializeField] private TextMeshProUGUI fuelText;
    [SerializeField] private Image fuelIcon;
    [SerializeField] private GameObject lowFuelWarning;

    [Header("Uyarı Ayarları")]
    [SerializeField] private float lowFuelThreshold = 25f;
    [SerializeField] private float warningBlinkSpeed = 1f;

    private void Start()
    {
        if (FuelManager.Instance != null)
        {
            FuelManager.Instance.OnFuelChanged += UpdateFuelUI;
            FuelManager.Instance.OnFuelEmpty += ShowEmptyFuel;
            UpdateFuelUI(FuelManager.Instance.CurrentFuel);
        }

        if (lowFuelWarning != null)
        {
            lowFuelWarning.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (FuelManager.Instance != null)
        {
            FuelManager.Instance.OnFuelChanged -= UpdateFuelUI;
            FuelManager.Instance.OnFuelEmpty -= ShowEmptyFuel;
        }
    }

    private void Update()
    {
        if (lowFuelWarning != null && FuelManager.Instance != null)
        {
            bool shouldShowWarning = FuelManager.Instance.CurrentFuel <= lowFuelThreshold && 
                                     FuelManager.Instance.CurrentFuel > 0f;
            
            lowFuelWarning.SetActive(shouldShowWarning);

            if (shouldShowWarning)
            {
                float alpha = Mathf.PingPong(Time.time * warningBlinkSpeed, 1f);
                Color color = lowFuelWarning.GetComponent<Image>().color;
                color.a = alpha;
                if (lowFuelWarning.GetComponent<Image>() != null)
                {
                    lowFuelWarning.GetComponent<Image>().color = color;
                }
            }
        }
    }

    private void UpdateFuelUI(float currentFuel)
    {
        if (FuelManager.Instance == null) return;

        float fuelPercentage = FuelManager.Instance.FuelPercentage;

        if (fuelBarFill != null)
        {
            RectTransform fillRect = fuelBarFill.GetComponent<RectTransform>();
            if (fillRect != null)
            {
                float maxWidth = 200f;
                fillRect.sizeDelta = new Vector2(maxWidth * fuelPercentage, fillRect.sizeDelta.y);
            }

            if (fuelBarFill.type == Image.Type.Filled)
            {
                fuelBarFill.fillAmount = fuelPercentage;
            }
        }

        if (fuelText != null)
        {
            fuelText.text = Mathf.RoundToInt(currentFuel).ToString() + " / " + 
                           Mathf.RoundToInt(FuelManager.Instance.MaxFuel).ToString();
        }

        if (fuelIcon != null)
        {
            Color iconColor = fuelPercentage > 0.3f ? Color.white : 
                             (fuelPercentage > 0f ? Color.yellow : Color.red);
            fuelIcon.color = iconColor;
        }
    }

    private void ShowEmptyFuel()
    {
        if (lowFuelWarning != null)
        {
            lowFuelWarning.SetActive(true);
        }

        Debug.LogWarning("Benzin bitti!");
    }
}

