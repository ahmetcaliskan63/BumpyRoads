using UnityEngine;

public class DistanceManager : MonoBehaviour
{
    public static DistanceManager Instance { get; private set; }

    [Header("Mesafe Ayarları")]
    [SerializeField] private float currentDistance = 0f;
    [SerializeField] private float distanceMultiplier = 1f;

    private Vector3 lastPosition;
    private Transform playerTransform;

    public float CurrentDistance => currentDistance;

    public System.Action<float> OnDistanceChanged;

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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        if (player != null)
        {
            playerTransform = player.transform;
            lastPosition = playerTransform.position;
        }

        currentDistance = 0f;
        OnDistanceChanged?.Invoke(currentDistance);
    }

    private void Update()
    {
        if (playerTransform == null) return;

        Vector3 currentPosition = playerTransform.position;
        float distanceDelta = Mathf.Abs(currentPosition.x - lastPosition.x);
        
        if (distanceDelta > 0.01f)
        {
            currentDistance += distanceDelta * distanceMultiplier;
            lastPosition = currentPosition;
            OnDistanceChanged?.Invoke(currentDistance);
        }
    }

    public void ResetDistance()
    {
        currentDistance = 0f;
        if (playerTransform != null)
        {
            lastPosition = playerTransform.position;
        }
        OnDistanceChanged?.Invoke(currentDistance);
    }
}

