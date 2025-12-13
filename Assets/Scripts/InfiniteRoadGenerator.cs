using UnityEngine;
using UnityEngine.U2D;

public class InfiniteRoadGenerator : MonoBehaviour
{
    [Header("Yol Ayarları")]
    [SerializeField] private float roadGenerationDistance = 20f;
    [SerializeField] private float segmentLength = 4f;
    [SerializeField] private float minHeight = -3f;
    [SerializeField] private float maxHeight = 10f;
    [SerializeField] private float heightVariation = 6f;
    [SerializeField] private float bottomDepth = 15f;
    [SerializeField] private float smoothness = 0.3f;
    [SerializeField] private float extremeDifficultyChance = 0.2f;

    // --- YENİ EKLENEN ÖZELLİK ---
    [Header("Başlangıç Ayarı")]
    [SerializeField] private float flatStartDistance = 20f; // İlk 20 metre dümdüz olsun

    [Header("Eşya ve Görsel Ayarlar")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject fuelCanisterPrefab;
    [SerializeField] private LayerMask roadLayer; 

    [SerializeField] private float commonHeightOffset = 1.5f; 
    [SerializeField] private float fuelVisualCorrection = 0f; 

    [Header("Mesafe ve Temizlik Ayarları")]
    [SerializeField] private float coinClusterDistance = 100f; 
    [SerializeField] private int coinsPerCluster = 5;
    [SerializeField] private float coinSpacing = 1.5f;
    [SerializeField] private float destroyDistance = 100f; 

    [Header("Benzin Artış Matematiği")]
    [SerializeField] private float initialFuelGap = 100f; 
    [SerializeField] private float fuelGapIncrement = 20f; 

    [Header("Referanslar")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private SpriteShapeController spriteShapeController;

    private float lastGeneratedX = 0f;
    private float lastHeight = 0f;
    private float nextCoinSpawnX = 50f;  
    private float nextFuelSpawnX = 0f; 
    private float currentFuelGap = 0f;
    
    // Başlangıçtaki sabit yüksekliği hafızada tutmak için
    private float initialGroundY = -2f; 

    private Transform pickupsParentTransform;
    private GameObject invisibleWall;
    private BoxCollider2D wallCollider;

    private void Start()
    {
        if (playerTransform == null) 
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (spriteShapeController == null) spriteShapeController = GetComponent<SpriteShapeController>();

        if (roadLayer == 0) roadLayer = LayerMask.GetMask("Default"); 

        currentFuelGap = initialFuelGap; 
        nextFuelSpawnX = initialFuelGap; 

        GameObject pickupsObj = GameObject.Find("Pickups");
        if (pickupsObj == null) pickupsObj = new GameObject("Pickups");
        pickupsParentTransform = pickupsObj.transform;

        CreateInvisibleWall();

        spriteShapeController.spline.isOpenEnded = false;
        InitializeRoad();
    }

    private void CreateInvisibleWall()
    {
        invisibleWall = new GameObject("InvisibleStartWall");
        wallCollider = invisibleWall.AddComponent<BoxCollider2D>();
        wallCollider.size = new Vector2(1f, 50f); 
        invisibleWall.layer = gameObject.layer; 
    }

    private void InitializeRoad()
    {
        Spline spline = spriteShapeController.spline;
        spline.Clear();

        float startX = playerTransform.position.x - 15f; 
        initialGroundY = -2f; // Başlangıç yüksekliği sabittir

        // Nokta 0: Sol Alt
        spline.InsertPointAt(0, new Vector3(startX, initialGroundY - bottomDepth, 0)); 
        spline.SetTangentMode(0, ShapeTangentMode.Linear);

        // Nokta 1: Sol Üst
        spline.InsertPointAt(1, new Vector3(startX, initialGroundY, 0)); 
        spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        spline.SetSpriteIndex(1, 0);

        spline.InsertPointAt(2, new Vector3(startX + segmentLength, initialGroundY, 0)); 
        spline.SetTangentMode(2, ShapeTangentMode.Continuous);
        spline.SetSpriteIndex(2, 0);

        spline.InsertPointAt(3, new Vector3(startX + segmentLength, initialGroundY - bottomDepth, 0)); 
        spline.SetTangentMode(3, ShapeTangentMode.Linear);

        lastGeneratedX = startX + segmentLength;
        lastHeight = initialGroundY;

        spriteShapeController.BakeMesh();
        spriteShapeController.BakeCollider();
        
        UpdateBackWall();
    }

    private void Update()
    {
        if (playerTransform == null) return;

        if (playerTransform.position.x > lastGeneratedX - roadGenerationDistance)
        {
            GenerateRoadSegment();
        }

        CheckAndSpawnItems();
        CleanupOldRoad();
        CleanupOldItems();
        UpdateBackWall();
    }
    
    private void LateUpdate()
    {
        if (spriteShapeController != null)
        {
            spriteShapeController.BakeCollider();
        }
    }

    private void UpdateBackWall()
    {
        if (invisibleWall == null || spriteShapeController == null) return;

        Spline spline = spriteShapeController.spline;
        if (spline.GetPointCount() < 2) return;

        Vector3 roadStartPoint = spline.GetPosition(1);
        invisibleWall.transform.position = new Vector3(roadStartPoint.x, roadStartPoint.y + 10f, 0f);
    }

    private void CleanupOldRoad()
    {
        Spline spline = spriteShapeController.spline;
        if (spline.GetPointCount() < 10) return;

        Vector3 point1Pos = spline.GetPosition(1);

        if (playerTransform.position.x - point1Pos.x > destroyDistance)
        {
            spline.RemovePointAt(1);

            Vector3 newFirstTopPoint = spline.GetPosition(1);
            Vector3 newBottomLeft = new Vector3(newFirstTopPoint.x, newFirstTopPoint.y - bottomDepth, 0);

            spline.SetPosition(0, newBottomLeft);
            spriteShapeController.BakeMesh();
        }
    }

    private void CleanupOldItems()
    {
        foreach (Transform child in pickupsParentTransform)
        {
            if (playerTransform.position.x - child.position.x > destroyDistance + 10f)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void GenerateRoadSegment()
    {
        Spline spline = spriteShapeController.spline;
        int insertIndex = spline.GetPointCount() - 1; 

        float newX = lastGeneratedX + segmentLength;
        float targetHeight;

        // --- DÜZ ZEMİN MANTIĞI BURADA ---
        // Eğer şu an oluşturduğumuz yol, başlangıçtan (player'ın ilk konumundan) 
        // 'flatStartDistance' kadar uzakta değilse, DÜZ YAP.
        
        // Basit hesap: Eğer yeni X konumu, belirlenen düzlük sınırının altındaysa:
        if (newX < (playerTransform.position.x + flatStartDistance)) 
        {
            // Yüksekliği değiştirme, başlangıçtaki sabit yükseklikte kal
            targetHeight = initialGroundY;
        }
        else
        {
            // ARTIK NORMAL ENGEBELİ YOL YAPABİLİRSİN
            if (Random.value < extremeDifficultyChance)
                targetHeight = lastHeight + Random.Range(-(heightVariation * 1.5f), (heightVariation * 1.5f));
            else
                targetHeight = lastHeight + Random.Range(-heightVariation, heightVariation);
            
            targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
        }

        // Geçişi yumuşat
        float nextHeight = Mathf.Lerp(lastHeight, targetHeight, smoothness);

        spline.InsertPointAt(insertIndex, new Vector3(newX, nextHeight, 0));
        spline.SetTangentMode(insertIndex, ShapeTangentMode.Continuous);
        spline.SetSpriteIndex(insertIndex, 0); 
        
        float tangentSize = segmentLength * 0.4f;
        spline.SetLeftTangent(insertIndex, new Vector3(-tangentSize, 0, 0));
        spline.SetRightTangent(insertIndex, new Vector3(tangentSize, 0, 0));

        int bottomCornerIndex = insertIndex + 1;
        spline.SetPosition(bottomCornerIndex, new Vector3(newX, nextHeight - bottomDepth, 0));
        spline.SetTangentMode(bottomCornerIndex, ShapeTangentMode.Linear);

        spriteShapeController.BakeMesh();
        lastGeneratedX = newX;
        lastHeight = nextHeight;
    }

    private void CheckAndSpawnItems()
    {
        float safetyBuffer = 18f; 

        while (nextFuelSpawnX < lastGeneratedX - safetyBuffer) 
        {
            bool success = PlaceObjectOnRoad(fuelCanisterPrefab, nextFuelSpawnX, fuelVisualCorrection);
            
            if (success)
            {
                currentFuelGap += fuelGapIncrement;
                nextFuelSpawnX += currentFuelGap; 
            }
            else
            {
                break; 
            }
        }

        while (nextCoinSpawnX < lastGeneratedX - (coinsPerCluster * coinSpacing + safetyBuffer))
        {
            SpawnCoinCluster(nextCoinSpawnX);
            nextCoinSpawnX += coinClusterDistance;
        }
    }

    private void SpawnCoinCluster(float startX)
    {
        for (int i = 0; i < coinsPerCluster; i++)
        {
            float currentCoinX = startX + (i * coinSpacing);
            PlaceObjectOnRoad(coinPrefab, currentCoinX, 0f);
        }
    }

    private bool PlaceObjectOnRoad(GameObject prefab, float xPos, float extraOffset)
    {
        if (prefab == null) return false;

        Vector2 rayOrigin = new Vector2(xPos, 50f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 100f, roadLayer);

        if (hit.collider != null)
        {
            float finalY = hit.point.y + commonHeightOffset + extraOffset;
            Vector3 spawnPos = new Vector3(xPos, finalY, -1f); 

            GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);
            item.transform.SetParent(pickupsParentTransform); 
            
            return true;
        }
        else
        {
            return false;
        }
    }
}