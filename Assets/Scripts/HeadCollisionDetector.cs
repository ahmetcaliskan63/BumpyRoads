using UnityEngine;

public class HeadCollisionDetector : MonoBehaviour
{
    private CrashSystem crashSystem;

    public void Initialize(CrashSystem system)
    {
        crashSystem = system;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (crashSystem == null) return;

        if (crashSystem.IsGround(collision.gameObject))
        {
            if (crashSystem.CheckCrashSpeed())
            {
                crashSystem.TriggerCrash();
            }
        }
    }
}

