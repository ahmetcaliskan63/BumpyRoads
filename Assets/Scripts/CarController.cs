using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D frontTireRb;
    [SerializeField] private Rigidbody2D backTireRb;
    [SerializeField] private Rigidbody2D carRb;

    [Header("Motor Ayarları")]
    [SerializeField] private float wheelTorque = 3000f;
    [SerializeField] private float forwardForce = 800f;
    [SerializeField] private float maxWheelAngularVelocity = 100f;

    [Header("Kontrol Ayarları")]
    [SerializeField] private float inputSmoothing = 8f;

    [Header("Benzin Tüketimi")]
    [SerializeField] private bool consumeFuelOnMovement = false;
    [SerializeField] private float fuelConsumptionMultiplier = 1f;

    private float rawInput;
    private float smoothedInput;
    private Keyboard keyboard;
    private Gamepad gamepad;

    private void Start()
    {
        keyboard = Keyboard.current;
        gamepad = Gamepad.current;

        if (frontTireRb == null || backTireRb == null || carRb == null)
        {
            Debug.LogError("CarController: Rigidbody2D referansları atanmamış! Inspector'dan atayın.");
        }

        // Motor sesini başlat (eğer AudioManager varsa)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartEngineSound();
        }
    }

    private void Update()
    {
        rawInput = 0f;

        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                rawInput = -1f;
            else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                rawInput = 1f;
        }

        if (gamepad != null)
        {
            rawInput = gamepad.leftStick.ReadValue().x;
        }

        if (rawInput == 0f)
        {
            rawInput = Input.GetAxisRaw("Horizontal");
        }

        smoothedInput = Mathf.Lerp(smoothedInput, rawInput, inputSmoothing * Time.deltaTime);

        // Motor sesinin pitch'ini güncelle (gaz verildiğinde ses değişsin)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateEnginePitch(smoothedInput);
        }
    }

    private void FixedUpdate()
    {
        if (frontTireRb == null || backTireRb == null || carRb == null)
            return;

        if (FuelManager.Instance != null && FuelManager.Instance.CurrentFuel <= 0f)
        {
            return;
        }

        float wheelForce = -smoothedInput * wheelTorque * Time.fixedDeltaTime;
        
        backTireRb.AddTorque(wheelForce);
        if (Mathf.Abs(backTireRb.angularVelocity) > maxWheelAngularVelocity)
        {
            backTireRb.angularVelocity = Mathf.Sign(backTireRb.angularVelocity) * maxWheelAngularVelocity;
        }

        frontTireRb.AddTorque(wheelForce);
        if (Mathf.Abs(frontTireRb.angularVelocity) > maxWheelAngularVelocity)
        {
            frontTireRb.angularVelocity = Mathf.Sign(frontTireRb.angularVelocity) * maxWheelAngularVelocity;
        }
    }
}
