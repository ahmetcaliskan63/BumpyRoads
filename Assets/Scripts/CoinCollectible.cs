using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [Header("Altın Objesi Ayarları")]
    [SerializeField] private float rotationSpeed = 180f;

    private bool isCollected = false;

    private void Update()
    {
        if (isCollected) return;

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        if (collision.CompareTag("Player") || 
            collision.CompareTag("Car") || 
            collision.GetComponent<CarController>() != null ||
            collision.GetComponentInParent<CarController>() != null)
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        if (isCollected) return;

        isCollected = true;

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CollectCoin();
        }

        Destroy(gameObject);
    }
}

