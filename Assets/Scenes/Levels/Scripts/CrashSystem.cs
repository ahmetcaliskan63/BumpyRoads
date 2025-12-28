using UnityEngine;

public class CrashSystem : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private GameObject driverHead;
    [SerializeField] private GameObject driverBody;
    [SerializeField] private CarController carController;
    [SerializeField] private Rigidbody2D carRb;

    [Header("Çarpışma Ayarları")]
    [SerializeField] private float separationForce = 5f;
    [SerializeField] private float minCrashSpeed = 2f;

    private bool hasCrashed = false;
    private bool headTouchingGround = false;
    private bool bodyTouchingGround = false;
    private Rigidbody2D headRb;
    private Rigidbody2D bodyRb;

    public System.Action OnCrash;

    private void Start()
    {
        SetupHeadCollision();
        SetupBodyCollision();
    }

    private void SetupHeadCollision()
    {
        if (driverHead != null)
        {
            Collider2D collider = driverHead.GetComponent<Collider2D>();
            if (collider == null)
            {
                CircleCollider2D circleCollider = driverHead.AddComponent<CircleCollider2D>();
                circleCollider.radius = 0.5f;
                circleCollider.isTrigger = true;
            }
            else
            {
                collider.isTrigger = true;
            }

            HeadCollisionDetector detector = driverHead.GetComponent<HeadCollisionDetector>();
            if (detector == null)
            {
                detector = driverHead.AddComponent<HeadCollisionDetector>();
            }
            detector.Initialize(this, carRb);
        }
    }

    private void SetupBodyCollision()
    {
        if (driverBody != null)
        {
            Collider2D collider = driverBody.GetComponent<Collider2D>();
            if (collider == null)
            {
                BoxCollider2D boxCollider = driverBody.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = true;
            }
            else
            {
                collider.isTrigger = true;
            }

            BodyCollisionDetector detector = driverBody.GetComponent<BodyCollisionDetector>();
            if (detector == null)
            {
                detector = driverBody.AddComponent<BodyCollisionDetector>();
            }
            detector.Initialize(this, carRb);
        }
    }

    public bool IsGround(GameObject obj)
    {
        return obj.CompareTag("Ground") || obj.name.Contains("Ground");
    }

    public void SetHeadTouchingGround(bool touching)
    {
        headTouchingGround = touching;
        CheckForCrash();
    }

    public void SetBodyTouchingGround(bool touching)
    {
        bodyTouchingGround = touching;
        CheckForCrash();
    }

    private void CheckForCrash()
    {
        // Kafa VEYA gövde zemine değdiğinde çarpışma tetiklenir
        // (Her ikisi de değmesi gerekmez, biri yeterli)
        if ((headTouchingGround || bodyTouchingGround) && !hasCrashed)
        {
            TriggerCrash();
        }
    }

    public void TriggerCrash()
    {
        if (hasCrashed) return;
        Crash();
    }

    private void Crash()
    {
        if (hasCrashed) return;
        hasCrashed = true;

        Debug.Log("Crash detected! Separating head and body.");

        if (carController != null)
        {
            carController.enabled = false;
        }

        if (driverHead != null)
        {
            Vector3 headPosition = driverHead.transform.position;
            Quaternion headRotation = driverHead.transform.rotation;
            
            // Kafa üzerindeki tüm joint'leri kaldır
            Joint2D[] headJoints = driverHead.GetComponents<Joint2D>();
            foreach (Joint2D joint in headJoints)
            {
                DestroyImmediate(joint);
            }
            
            // Araba üzerindeki joint'leri de kontrol et (kafaya bağlı olanlar)
            if (carRb != null)
            {
                Joint2D[] carJoints = carRb.GetComponents<Joint2D>();
                foreach (Joint2D joint in carJoints)
                {
                    if (joint.connectedBody != null && joint.connectedBody.gameObject == driverHead)
                    {
                        DestroyImmediate(joint);
                    }
                }
            }
            
            // Parent'ı kaldır
            driverHead.transform.SetParent(null);
            driverHead.transform.position = headPosition;
            driverHead.transform.rotation = headRotation;
            
            // Collider'ı trigger'dan çıkar
            Collider2D headCollider = driverHead.GetComponent<Collider2D>();
            if (headCollider != null)
            {
                headCollider.isTrigger = false;
            }
            
            // Mevcut Rigidbody2D'yi kontrol et ve ayarla
            Rigidbody2D existingRb = driverHead.GetComponent<Rigidbody2D>();
            if (existingRb != null)
            {
                existingRb.isKinematic = false;
                existingRb.gravityScale = 1f;
                existingRb.linearDamping = 2f;
                existingRb.angularDamping = 5f;
                headRb = existingRb;
            }
            else
            {
                headRb = driverHead.AddComponent<Rigidbody2D>();
                headRb.isKinematic = false;
                headRb.gravityScale = 1f;
                headRb.linearDamping = 2f;
                headRb.angularDamping = 5f;
            }
            
            // Kuvvet uygula
            Vector2 headForce = new Vector2(
                Random.Range(-separationForce, separationForce),
                Random.Range(2f, 5f)
            );
            headRb.AddForce(headForce, ForceMode2D.Impulse);
            headRb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
        }

        if (driverBody != null)
        {
            Vector3 bodyPosition = driverBody.transform.position;
            Quaternion bodyRotation = driverBody.transform.rotation;
            
            // Gövde üzerindeki tüm joint'leri kaldır
            Joint2D[] bodyJoints = driverBody.GetComponents<Joint2D>();
            foreach (Joint2D joint in bodyJoints)
            {
                DestroyImmediate(joint);
            }
            
            // Araba üzerindeki joint'leri de kontrol et (gövdeye bağlı olanlar)
            if (carRb != null)
            {
                Joint2D[] carJoints = carRb.GetComponents<Joint2D>();
                foreach (Joint2D joint in carJoints)
                {
                    if (joint.connectedBody != null && joint.connectedBody.gameObject == driverBody)
                    {
                        DestroyImmediate(joint);
                    }
                }
            }
            
            // Parent'ı kaldır
            driverBody.transform.SetParent(null);
            driverBody.transform.position = bodyPosition;
            driverBody.transform.rotation = bodyRotation;
            
            // Collider'ı trigger'dan çıkar
            Collider2D bodyCollider = driverBody.GetComponent<Collider2D>();
            if (bodyCollider != null)
            {
                bodyCollider.isTrigger = false;
            }
            
            // Mevcut Rigidbody2D'yi kontrol et ve ayarla
            Rigidbody2D existingRb = driverBody.GetComponent<Rigidbody2D>();
            if (existingRb != null)
            {
                existingRb.isKinematic = false;
                existingRb.gravityScale = 1f;
                existingRb.linearDamping = 2f;
                existingRb.angularDamping = 5f;
                bodyRb = existingRb;
            }
            else
            {
                bodyRb = driverBody.AddComponent<Rigidbody2D>();
                bodyRb.isKinematic = false;
                bodyRb.gravityScale = 1f;
                bodyRb.linearDamping = 2f;
                bodyRb.angularDamping = 5f;
            }
            
            // Kuvvet uygula
            Vector2 bodyForce = new Vector2(
                Random.Range(-separationForce, separationForce),
                Random.Range(2f, 5f)
            );
            bodyRb.AddForce(bodyForce, ForceMode2D.Impulse);
            bodyRb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
        }

        if (carRb != null)
        {
            carRb.constraints = RigidbodyConstraints2D.None;
        }

        OnCrash?.Invoke();
    }
}

