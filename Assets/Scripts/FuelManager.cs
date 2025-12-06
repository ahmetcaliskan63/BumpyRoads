using UnityEngine;

public class FuelManager : MonoBehaviour
{
    public static FuelManager Instance { get; private set; }

    [Header("Benzin Ayarları")]
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float fuelConsumptionRate = 0.5f;
    [SerializeField] private float fuelPerCollectible = 25f;
    [SerializeField] private float constantConsumptionRate = 0.2f;

    private float currentFuel;
    private bool isConsumingFuel = true;

    public float CurrentFuel => currentFuel;
    public float MaxFuel => maxFuel;
    public float FuelPercentage => currentFuel / maxFuel;
    public float FuelConsumptionRate => fuelConsumptionRate;

    public System.Action<float> OnFuelChanged;
    public System.Action OnFuelEmpty;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        currentFuel = maxFuel;
        OnFuelChanged?.Invoke(currentFuel);
    }

    private void Update()
    {
        if (isConsumingFuel && currentFuel > 0f)
        {
            ConsumeFuel(constantConsumptionRate * Time.deltaTime);
        }
    }

    public void ConsumeFuel(float amount)
    {
        if (!isConsumingFuel) return;

        currentFuel -= amount;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        OnFuelChanged?.Invoke(currentFuel);

        if (currentFuel <= 0f)
        {
            currentFuel = 0f;
            OnFuelEmpty?.Invoke();
        }
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        OnFuelChanged?.Invoke(currentFuel);
    }

    public void CollectFuel()
    {
        currentFuel = maxFuel;
        OnFuelChanged?.Invoke(currentFuel);
    }

    public void SetFuelConsumption(bool consume)
    {
        isConsumingFuel = consume;
    }

    public void ResetFuel()
    {
        currentFuel = maxFuel;
        OnFuelChanged?.Invoke(currentFuel);
    }
}

