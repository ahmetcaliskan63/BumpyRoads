using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Oyun genelinde müzik ve ses efektleri yönetimi için singleton manager.
/// Ana menü müziği ve oyun içi sesler için kullanılır.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Müzik Ayarları")]
    [SerializeField] private AudioSource musicSource; // Müzik için AudioSource
    [SerializeField] private AudioClip menuMusicClip; // Ana menü müziği
    [SerializeField] private float musicVolume = 0.7f; // Müzik ses seviyesi (0-1 arası)
    [SerializeField] private bool playOnStart = true; // Başlangıçta otomatik çal

    [Header("Ses Efektleri Ayarları")]
    [SerializeField] private AudioSource sfxSource; // Ses efektleri için AudioSource
    [SerializeField] private float sfxVolume = 1f; // Ses efektleri ses seviyesi
    [SerializeField] private AudioClip coinCollectSound; // Altın toplama sesi
    [SerializeField] private AudioClip fuelCollectSound; // Benzin bidonu toplama sesi

    [Header("Motor Ses Ayarları")]
    [SerializeField] private AudioSource engineSource; // Motor sesi için AudioSource
    [SerializeField] private AudioClip engineSoundClip; // Motor sesi
    [SerializeField] private float engineVolume = 0.5f; // Motor ses seviyesi
    [SerializeField] private float idlePitch = 0.8f; // Rölanti pitch (gaz verilmediğinde)
    [SerializeField] private float maxPitch = 1.5f; // Maksimum pitch (tam gaz)
    [SerializeField] private float pitchChangeSpeed = 2f; // Pitch değişim hızı

    private string currentSceneName;

    private void Awake()
    {
        // Singleton pattern - sadece bir tane AudioManager olmalı
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne değişimlerinde korun
            InitializeAudioSources();
        }
        else
        {
            // Eğer zaten bir AudioManager varsa, bu objeyi yok et
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Sahne değişimlerini dinle
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // İlk sahne için müziği kontrol et
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        // Event listener'ı temizle
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// AudioSource componentlerini otomatik oluştur veya bul
    /// </summary>
    private void InitializeAudioSources()
    {
        // Müzik için AudioSource
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true; // Müzik döngüde çalacak
            musicSource.playOnAwake = false; // Otomatik başlamasın, biz kontrol edelim
            musicSource.volume = musicVolume;
        }

        // Ses efektleri için AudioSource
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }

        // Motor sesi için AudioSource
        if (engineSource == null)
        {
            GameObject engineObj = new GameObject("EngineSource");
            engineObj.transform.SetParent(transform);
            engineSource = engineObj.AddComponent<AudioSource>();
            engineSource.loop = true; // Motor sesi sürekli döngüde
            engineSource.playOnAwake = false;
            engineSource.volume = engineVolume;
            engineSource.pitch = idlePitch; // Başlangıçta rölanti
        }
    }

    /// <summary>
    /// Sahne yüklendiğinde çağrılır
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;

        // Ana menü sahnesindeysek müziği çal
        if (currentSceneName == "MainMenu")
        {
            PlayMenuMusic();
            StopEngineSound(); // Ana menüde motor sesi çalmasın
        }
        else
        {
            // Diğer sahnelerde müziği durdur, motor sesini başlat
            StopMusic();
            StartEngineSound(); // Oyun sahnelerinde motor sesi başlasın
        }
    }

    /// <summary>
    /// Ana menü müziğini çalar
    /// </summary>
    public void PlayMenuMusic()
    {
        if (menuMusicClip == null)
        {
            Debug.LogWarning("AudioManager: menuMusicClip atanmamış! Inspector'dan atayın.");
            return;
        }

        if (musicSource == null)
        {
            Debug.LogError("AudioManager: musicSource bulunamadı!");
            return;
        }

        // Eğer zaten aynı müzik çalıyorsa, tekrar başlatma
        if (musicSource.isPlaying && musicSource.clip == menuMusicClip)
        {
            return;
        }

        musicSource.clip = menuMusicClip;
        musicSource.volume = musicVolume;
        musicSource.Play();
        
        Debug.Log("Ana menü müziği başlatıldı.");
    }

    /// <summary>
    /// Müziği durdurur
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("Müzik durduruldu.");
        }
    }

    /// <summary>
    /// Müziği duraklatır (devam ettirmek için ResumeMusic kullan)
    /// </summary>
    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    /// <summary>
    /// Duraklatılmış müziği devam ettirir
    /// </summary>
    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

    /// <summary>
    /// Müzik ses seviyesini ayarlar (0-1 arası)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    /// <summary>
    /// Ses efektleri ses seviyesini ayarlar (0-1 arası)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    /// <summary>
    /// Ses efekti çalar (gelecekte buton sesleri vs. için)
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    /// <summary>
    /// Altın toplama sesini çalar
    /// </summary>
    public void PlayCoinCollectSound()
    {
        if (coinCollectSound == null)
        {
            Debug.LogWarning("AudioManager: coinCollectSound atanmamış! Inspector'dan atayın.");
            return;
        }

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(coinCollectSound, sfxVolume);
        }
        else
        {
            Debug.LogError("AudioManager: sfxSource bulunamadı!");
        }
    }

    /// <summary>
    /// Benzin bidonu toplama sesini çalar
    /// </summary>
    public void PlayFuelCollectSound()
    {
        if (fuelCollectSound == null)
        {
            Debug.LogWarning("AudioManager: fuelCollectSound atanmamış! Inspector'dan atayın.");
            return;
        }

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(fuelCollectSound, sfxVolume);
        }
        else
        {
            Debug.LogError("AudioManager: sfxSource bulunamadı!");
        }
    }

    /// <summary>
    /// Müzik çalıyor mu kontrol eder
    /// </summary>
    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }

    /// <summary>
    /// Mevcut müzik ses seviyesini döndürür
    /// </summary>
    public float GetMusicVolume()
    {
        return musicVolume;
    }

    /// <summary>
    /// Mevcut SFX ses seviyesini döndürür
    /// </summary>
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    /// <summary>
    /// Motor sesini başlatır (oyun sahnelerinde)
    /// </summary>
    public void StartEngineSound()
    {
        if (engineSoundClip == null)
        {
            Debug.LogWarning("AudioManager: engineSoundClip atanmamış! Inspector'dan atayın.");
            return;
        }

        if (engineSource == null)
        {
            Debug.LogError("AudioManager: engineSource bulunamadı!");
            return;
        }

        // Eğer zaten çalıyorsa, tekrar başlatma
        if (engineSource.isPlaying && engineSource.clip == engineSoundClip)
        {
            return;
        }

        engineSource.clip = engineSoundClip;
        engineSource.volume = engineVolume;
        engineSource.pitch = idlePitch; // Rölanti ile başla
        engineSource.Play();
        
        Debug.Log("Motor sesi başlatıldı.");
    }

    /// <summary>
    /// Motor sesini durdurur
    /// </summary>
    public void StopEngineSound()
    {
        if (engineSource != null && engineSource.isPlaying)
        {
            engineSource.Stop();
            Debug.Log("Motor sesi durduruldu.");
        }
    }

    /// <summary>
    /// Motor sesinin pitch'ini günceller (gaz verildiğinde çağrılır)
    /// </summary>
    /// <param name="inputValue">Input değeri (-1 ile 1 arası, mutlak değer kullanılır)</param>
    public void UpdateEnginePitch(float inputValue)
    {
        if (engineSource == null || !engineSource.isPlaying)
        {
            return;
        }

        // Input değerinin mutlak değerini al (sadece gaz miktarı önemli)
        float absInput = Mathf.Abs(inputValue);
        
        // Pitch'i hesapla: rölanti (0 input) -> idlePitch, tam gaz (1 input) -> maxPitch
        float targetPitch = Mathf.Lerp(idlePitch, maxPitch, absInput);
        
        // Yumuşak geçiş için lerp kullan
        float currentPitch = engineSource.pitch;
        engineSource.pitch = Mathf.Lerp(currentPitch, targetPitch, pitchChangeSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Motor ses seviyesini ayarlar (0-1 arası)
    /// </summary>
    public void SetEngineVolume(float volume)
    {
        engineVolume = Mathf.Clamp01(volume);
        if (engineSource != null)
        {
            engineSource.volume = engineVolume;
        }
    }

    /// <summary>
    /// Motor sesi çalıyor mu kontrol eder
    /// </summary>
    public bool IsEngineSoundPlaying()
    {
        return engineSource != null && engineSource.isPlaying;
    }
}

