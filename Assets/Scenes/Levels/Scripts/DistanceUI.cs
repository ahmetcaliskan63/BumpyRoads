using UnityEngine;
using TMPro;

public class DistanceUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private GameObject distanceIcon;

    [Header("Gösterim Ayarları")]
    [SerializeField] private bool showInMeters = true;
    [SerializeField] private int decimalPlaces = 1;

    private void Start()
    {
        if (DistanceManager.Instance != null)
        {
            DistanceManager.Instance.OnDistanceChanged += UpdateDistanceUI;
            UpdateDistanceUI(DistanceManager.Instance.CurrentDistance);
        }
    }

    private void OnDestroy()
    {
        if (DistanceManager.Instance != null)
        {
            DistanceManager.Instance.OnDistanceChanged -= UpdateDistanceUI;
        }
    }

    private void UpdateDistanceUI(float currentDistance)
    {
        if (distanceText != null)
        {
            if (showInMeters)
            {
                distanceText.text = currentDistance.ToString("F" + decimalPlaces) + " m";
            }
            else
            {
                distanceText.text = currentDistance.ToString("F" + decimalPlaces);
            }
        }
    }
}

