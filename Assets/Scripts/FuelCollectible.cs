using UnityEngine;

public class FuelCollectible : MonoBehaviour
{
    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        if (collision.CompareTag("Player") || 
            collision.CompareTag("Car") || 
            collision.GetComponent<CarController>() != null ||
            collision.GetComponentInParent<CarController>() != null)
        {
            CollectFuel();
        }
    }

    private void CollectFuel()
    {
        if (isCollected) return;

        isCollected = true;

        if (FuelManager.Instance != null)
        {
            FuelManager.Instance.CollectFuel();
        }

        Destroy(gameObject);
    }
}

