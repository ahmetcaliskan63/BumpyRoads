using UnityEngine;

public class BodyCollisionDetector : MonoBehaviour
{
    private CrashSystem crashSystem;

    public void Initialize(CrashSystem system, Rigidbody2D carRigidbody)
    {
        crashSystem = system;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (crashSystem == null) return;

        if (crashSystem.IsGround(collision.gameObject))
        {
            crashSystem.SetBodyTouchingGround(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (crashSystem == null) return;

        if (crashSystem.IsGround(collision.gameObject))
        {
            crashSystem.SetBodyTouchingGround(false);
        }
    }
}

