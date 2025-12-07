using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Ayarları")]
    [SerializeField] private string[] levelSceneNames = { "Level1", "Level2", "Level3" };
    [SerializeField] private bool unlockAllLevels = false; // Test için

    private const string LEVEL_UNLOCK_KEY = "LevelUnlocked_";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Level 1'i her zaman açık yap
        UnlockLevel(1);
    }

    private void Start()
    {
        if (unlockAllLevels)
        {
            UnlockAllLevels();
        }
    }

    public void LoadLevel(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > levelSceneNames.Length)
        {
            Debug.LogWarning($"Level {levelNumber} bulunamadı!");
            return;
        }

        if (!IsLevelUnlocked(levelNumber) && !unlockAllLevels)
        {
            Debug.LogWarning($"Level {levelNumber} kilitli!");
            return;
        }

        string sceneName = levelSceneNames[levelNumber - 1];
        
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"Level {levelNumber} için sahne adı tanımlanmamış!");
            return;
        }

        Debug.Log($"Level {levelNumber} yükleniyor: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        if (unlockAllLevels) return true;
        if (levelNumber == 1) return true; // Level 1 her zaman açık

        string key = LEVEL_UNLOCK_KEY + levelNumber;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    public void UnlockLevel(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > levelSceneNames.Length)
        {
            Debug.LogWarning($"Level {levelNumber} geçersiz!");
            return;
        }

        string key = LEVEL_UNLOCK_KEY + levelNumber;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        
        Debug.Log($"Level {levelNumber} açıldı!");
    }

    public void UnlockAllLevels()
    {
        for (int i = 1; i <= levelSceneNames.Length; i++)
        {
            UnlockLevel(i);
        }
    }

    public void LockLevel(int levelNumber)
    {
        string key = LEVEL_UNLOCK_KEY + levelNumber;
        PlayerPrefs.SetInt(key, 0);
        PlayerPrefs.Save();
    }

    public void ResetAllProgress()
    {
        for (int i = 2; i <= levelSceneNames.Length; i++)
        {
            LockLevel(i);
        }
        // Level 1 her zaman açık kalır
        PlayerPrefs.Save();
        Debug.Log("Tüm ilerleme sıfırlandı!");
    }

    public int GetHighestUnlockedLevel()
    {
        for (int i = levelSceneNames.Length; i >= 1; i--)
        {
            if (IsLevelUnlocked(i))
            {
                return i;
            }
        }
        return 1;
    }

    // Level tamamlandığında bir sonraki level'i açmak için
    public void CompleteLevel(int levelNumber)
    {
        if (levelNumber < levelSceneNames.Length)
        {
            UnlockLevel(levelNumber + 1);
        }
    }
}

