# Unity ve Yapay Zekâyla Simülasyon Oyunu — kurs dosyaları

Kafede bardağı dökmeden taşıyan insansı robot oyunu. Bu klasörde gerçek bir **Unity projesi** var (`kafe-robot/`):
sahne, prefab, C# bileşenleri, seviye dosyası ve Unity Test Framework testleri. Kursta kodu **Claude Code** yazdı,
Unity'ye Unity'nin resmî komut satırı aracı **Unity CLI** ile bağlandı; siz de aynı yolu izleyeceksiniz.

| Araç | Kursta kullanılan sürüm |
|---|---|
| Unity Editor | Unity 6.3 LTS (6000.3.26f1), Unity Personal (ücretsiz) |
| Şablon | Universal 3D (URP 17.3.0) |
| Unity CLI | 1.0.0-beta.13 (beta) |
| Pipeline paketi | com.unity.pipeline 0.8.0-exp.1 (deneysel; "önizleme") |
| Test | Unity Test Framework 1.6.0 · Input System 1.20.0 |

Bardağın yolu ve ivmesi bir **benzetim kaydından** gelir (`kafe-robot/Assets/Kafe/Veri/benzetim_kaydi.csv`); gerçek robot ölçümü değildir.
Oda ve masa ölçüleri, hız ve ivme sınırı, dökülme eşiği ve puan kuralı **ÖRNEK** değerlerdir.

## 1. Kurulum

1. **Unity CLI** (macOS / Linux): `curl -fsSL https://unity.com/install.sh | bash` — Windows (PowerShell): `irm https://unity.com/install.ps1 | iex`.
   Yeni bir terminal açıp `unity --version` ile denetleyin.
2. **Giriş ve lisans:** `unity auth login`, ardından `unity license` (listede Unity Personal görünmeli).
3. **Unity 6.3 LTS + Web modülü:** `unity install 6000.3.26f1 --module webgl` (ya da Unity Hub'dan Unity 6.3 LTS ve "Web Build Support").
4. **Editörü bir kez elle açın:** `unity open kafe-robot`. İlk açılışta Unity "Unity Editor Software Terms" onayını sorar; bunu kendiniz okuyup onaylarsınız.
   İlk açılış paketleri indirir ve `Library/` klasörünü kurar; birkaç dakika sürer.

## 2. Claude Code'u Unity'ye bağlama

Yapay zekâ ajanları Unity'ye yalnız Unity'nin kendi araçlarıyla bağlanır (Unity Hizmet Koşulları); editör penceresini tıklatan araç kullanılmaz.

- `unity setup claude` — Unity'nin Claude Code eklentisini kurar (Unity becerileri eklentiyle gelir). **Ya da**
- `unity skill install claude-code` — yalnız Unity CLI rehberini Claude Code'a kurar (kursta bu yol denendi). İkisini birlikte kurmayın.
- İsteğe bağlı: `unity mcp configure claude-code` Unity'nin MCP sunucusunu Claude Code'a kaydeder. Kurs hazırlanırken sunucu Personal lisansla
  açıldı ve araçlarını listeledi; araç çağrıları açık bir editör ister ve kursta denenmedi.

Sonra proje klasöründe Claude Code'u açın ve Unity'yi CLI ile kullanmasını söyleyin. Örnek istekler `claude-code/` klasöründe.

## 3. Testleri koşma

```bash
cd kafe-robot
unity test . --mode EditMode --output ../editmode.xml     # 31 test: kurallar, sahne, prefab, fizik
unity test . --mode PlayMode --output ../playmode.xml     # 9 test: oyun Play modunda oynatılır
```

Editör kapalıyken koşun (aynı projeyi iki editör açamaz). Testler ölçtükleri sayıları `kafe-robot/Olcumler/*.json` dosyalarına yazar.
Sonuçlarınızı `beklenen/` klasörüyle karşılaştırın: `beklenen/OLCUMLER.json`, `beklenen/editmode.xml`, `beklenen/playmode.xml`, `beklenen/olcumler/`.
Kare süresi (`kare-butcesi.json`) ve oynatıştaki kare sayısı makineye göre değişir; ötekiler aynı çıkmalıdır.

Ders ekranlarındaki kareleri yeniden üretmek için (uzun sürer): `unity test . --mode PlayMode --filter "Kafe.Testler.GorselKayit"`.

## 4. Sahneyi yeniden kurma

```bash
unity run . -- -executeMethod Kafe.EditorAraclari.SahneKurucu.Kur
```

Editörde aynı iş: menü **Kafe > Sahneyi Kur**. Sahne `Assets/Kafe/Seviyeler/seviye-1.json` dosyasından kurulur.

## 5. Web'e derleme

```bash
unity build . --create-profile WebGL                       # bir kez (projede hazır geliyor)
unity build . --profile WebGL --output-path ../kafe-robot-web
unity build run .                                          # yerel sunucuda tarayıcıda açar
```

Sıkıştırma kapalı seçildi: çıktı her durağan dosya sunucusunda açılır (`python3 -m http.server` gibi). Tarayıcılar derlemeyi
doğrudan dosyadan (`file://`) açmaz; küçük bir sunucu gerekir. Telefona kurulan uygulama dosyası (APK / iOS) bu kursun kapsamında değildir.

## 6. Klasörler

- `kafe-robot/Assets/Kafe/Scripts/Kurallar/` — oyun kuralları, saf C# (puan, durum makinesi, piksel–metre çevirisi, seviye denetimi)
- `kafe-robot/Assets/Kafe/Scripts/Bilesenler/` — Unity bileşenleri (robot sürücüsü, bardak, oyun yöneticisi, dokunma girdisi, güvenli alan)
- `kafe-robot/Assets/Kafe/Editor/SahneKurucu.cs` — sahneyi, prefabı ve malzemeleri kuran editör betiği
- `kafe-robot/Assets/Kafe/Tests/` — Edit Mode ve Play Mode testleri
- `kafe-robot/Assets/Kafe/Seviyeler/` — seviye dosyası ve kayıtlı dokunuşlar (JSON)
- `beklenen/` — kurs hazırlanırken alınan test raporları ve ölçümler
- `claude-code/` — Claude Code'a verilen gerçek istekler ve yazdığı ilk sürümler (Ders 1.4 ve 3.4)
- `RUBRIK.md` — bitirme işi ve ölçüt çizelgesi

## Yapay zekâ beyanı

Bu projedeki C# kodu, testler ve sahne kurucu betik Claude Code ile yazıldı; her parça Unity Test Framework testleriyle ve gerçek Unity koşumuyla sınandı.
