# Ana Menü Kurulum Talimatları

## 📋 Adım Adım Kurulum

### 1. MainMenu Sahnesi Oluşturma

1. Unity Editor'da **Assets > Scenes** klasörüne sağ tıklayın
2. **Create > Scene** seçin
3. Sahneyi **"MainMenu"** olarak adlandırın
4. Sahneyi kaydedin

### 2. UI Canvas Oluşturma

1. Hierarchy'de sağ tıklayın → **UI > Canvas**
2. Canvas'ı seçin ve Inspector'da:
   - **Render Mode**: Screen Space - Overlay
   - **Canvas Scaler**: Scale With Screen Size
   - **Reference Resolution**: 1920 x 1080

### 3. Ana Menü Paneli

Canvas altında bir **Panel** oluşturun (sağ tık → UI > Panel):
- Adı: **"MainMenuPanel"**
- Anchor: Stretch-Stretch (tüm ekranı kaplasın)
- Background: İstediğiniz renk veya sprite

### 4. Oyun Başlığı

MainMenuPanel altında:
- **TextMeshPro - Text (UI)** ekleyin
- Adı: **"GameTitleText"**
- Metin: **"BUMPY ROADS"** (veya istediğiniz başlık)
- Font size: 80-100
- Ortala (Anchor: Middle-Center, Y: 200)

### 5. Ana Menü Butonları

MainMenuPanel altında 4 buton oluşturun (UI > Button - TextMeshPro):

#### a) Play Button
- Adı: **"PlayButton"**
- Metin: **"OYNA"**
- Pozisyon: Y: 0

#### b) Level Select Button
- Adı: **"LevelSelectButton"**
- Metin: **"SEVİYE SEÇ"**

#### c) Settings Button
- Adı: **"SettingsButton"**
- Metin: **"AYARLAR"**

#### d) Quit Button
- Adı: **"QuitButton"**
- Metin: **"ÇIKIŞ"**

**Butonları dikey olarak hizalayın** (Y pozisyonları: 0, -80, -160, -240 gibi)

### 6. Level Seçim Paneli

Canvas altında yeni bir **Panel** oluşturun:
- Adı: **"LevelSelectPanel"**
- Başlangıçta kapalı olmalı (Inspector'da aktif değil)

#### Level Butonları:
3 buton oluşturun (her biri için):
- **Level1Button** - Metin: "Level 1"
- **Level2Button** - Metin: "Level 2" 
- **Level3Button** - Metin: "Level 3"

Her butonun yanına bir **Image** ekleyin (kilit ikonu için):
- **Level1LockIcon**
- **Level2LockIcon**
- **Level3LockIcon**
- Başlangıçta kapalı olmalılar

Her butonun altına **TextMeshPro** ekleyin:
- **Level1Text**, **Level2Text**, **Level3Text**

**Back Button** ekleyin:
- Adı: **"BackToMainMenuButton"**
- Metin: **"GERİ"**

### 7. Ayarlar Paneli

Canvas altında yeni bir **Panel** oluşturun:
- Adı: **"SettingsPanel"**
- Başlangıçta kapalı

İçine ekleyin:
- **Slider** (Müzik sesi) - Adı: **"MusicVolumeSlider"**
- **Slider** (Efekt sesi) - Adı: **"SFXVolumeSlider"**
- **Toggle** (Tam ekran) - Adı: **"FullscreenToggle"**
- **Button** (Geri) - Adı: **"SettingsBackButton"**

### 8. MainMenuUI Script'i Ekleme

1. Canvas'a **MainMenuUI** script'ini ekleyin
2. Inspector'da tüm referansları atayın:
   - Main Menu Panel
   - Level Select Panel
   - Settings Panel
   - Tüm butonlar
   - Tüm text'ler
   - Lock icon'ları
   - Slider'lar ve toggle

### 9. LevelManager Ekleme

1. Hierarchy'de boş bir GameObject oluşturun
2. Adı: **"LevelManager"**
3. **LevelManager** script'ini ekleyin
4. Inspector'da:
   - **Level Scene Names** array'ine: "Level1", "Level2", "Level3" ekleyin
   - **Unlock All Levels**: Test için true yapabilirsiniz

### 10. Build Settings

1. **File > Build Settings** açın
2. **Add Open Scenes** ile MainMenu sahnesini ekleyin
3. MainMenu'yu **en üste** taşıyın (Index 0)
4. Level1, Level2, Level3'ü de ekleyin

### 11. Test

1. MainMenu sahnesini açın
2. Play'e basın
3. Butonların çalıştığını test edin

## 🎨 Tasarım Önerileri

- **Renkler**: Oyununuzun temasına uygun renkler kullanın
- **Font**: TextMeshPro font'unu büyük ve okunabilir yapın
- **Butonlar**: Hover ve click efektleri ekleyin
- **Arka plan**: Oyununuzun bir screenshot'ını veya arka plan görseli ekleyin

## ✅ Kontrol Listesi

- [ ] MainMenu sahnesi oluşturuldu
- [ ] Canvas ve paneller oluşturuldu
- [ ] Tüm butonlar oluşturuldu ve referanslar atandı
- [ ] MainMenuUI script'i eklendi ve tüm referanslar atandı
- [ ] LevelManager oluşturuldu
- [ ] Build Settings'e sahneler eklendi
- [ ] Test edildi ve çalışıyor

