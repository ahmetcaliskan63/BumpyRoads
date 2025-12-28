using UnityEngine;
using UnityEngine.InputSystem;

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
    private CarController carController;

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
        
        if (carController == null)
        {
            carController = FindObjectOfType<CarController>();
        }
    }

    private void Update()
    {
        if (!isConsumingFuel || currentFuel <= 0f) return;

        bool isAccelerating = IsCarAccelerating();
        
        if (isAccelerating)
        {
            ConsumeFuel(constantConsumptionRate * Time.deltaTime);
        }
    }

    private bool IsCarAccelerating()
    {
        if (carController == null)
        {
            carController = FindObjectOfType<CarController>();
        }

        if (carController == null) return false;

        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.dKey.isPressed || 
                keyboard.leftArrowKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                return true;
            }
        }

        if (gamepad != null)
        {
            float stickInput = gamepad.leftStick.ReadValue().x;
            if (Mathf.Abs(stickInput) > 0.1f)
            {
                return true;
            }
        }

        float input = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(input) > 0.1f)
        {
            return true;
        }

        return false;
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

