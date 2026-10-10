# Bitirme işi ve ölçüt çizelgesi

**Bitirme işi:** kafe oyunu için kendi seviyenizi hazırlayın. Teslim: seviye dosyanız (`Assets/Kafe/Seviyeler/seviye-benim.json`),
dokunuş listeniz (`benim-dokunuslar.json`), iki test raporu (`editmode.xml`, `playmode.xml`) ve oyunun bittiği anın bir ekran görüntüsü.

Her ölçütü kendiniz denetleyebilirsiniz; "nasıl denetlenir" sütunu Unity'de ya da komut satırında yapılacak işi söyler.

| Ölçüt | Yeterli | Eksik | Nasıl denetlenir |
|---|---|---|---|
| Seviye dosyası | En az dört masa, başlangıç ve teslim yeri var; hiçbiri bir masaya değmiyor | Dosya açılmıyor ya da başlangıç bir masanın içinde | Seviye denetimi testi (`Seviye_Dosyasi_Temiz`) sizin dosyanızla geçer |
| Sahne | Sahne kendi seviye dosyanızdan kuruldu; masalar prefab kopyası | Masalar elle tek tek çizilmiş | `unity run . -- -executeMethod Kafe.EditorAraclari.SahneKurucu.Kur` çıkış 0; Hierarchy'de masalar prefab kopyası olarak görünür |
| Teslim | Kayıtlı dokunuşlarınızla robot bardağı dökmeden teslim ediyor | Bardak dökülüyor ya da süre doluyor | Play Mode testinde durum `Teslim`, puan sıfırdan büyük |
| Tekrarlanabilirlik | Aynı kayıt iki kez oynatılınca süre, puan ve çarpma sayısı aynı | İki oynatış farklı | `Ayni_Girdi_Ayni_Sonuc` testi sizin kaydınızla geçer |
| Testler | Edit Mode ve Play Mode testlerinin hepsi geçiyor | Kalan test var | `unity test . --mode EditMode` ve `--mode PlayMode`; raporlarda `failed="0"` |
| Yeni kural | Kendi eklediğiniz bir kural (örneğin yeni bir puan cezası ya da durum) için en az bir test yazdınız | Kural var ama testi yok | Yeni test raporda adıyla görünür ve geçer |
| Claude Code katmanı (isteğe bağlı) | Kuralı Claude Code'a yazdırdınız; isteğinizi ve ilk yazılan kodu sakladınız, testten sonra kabul ettiniz | Kod okunmadan ve sınanmadan kabul edilmiş | İstek metni, ilk sürüm ve test raporu teslimde yer alır |
| Web derlemesi (isteğe bağlı) | Oyun Web için derlendi ve tarayıcıda oynanıyor | Derleme hata veriyor | `unity build . --profile WebGL --output-path ../kafe-robot-web`, ardından `unity build run .` |

**Geçme koşulu:** ilk altı ölçütün en az beşi "yeterli". Seviyenin ölçülerini siz belirlersiniz; değerleri teslimin başına yazın.

Not: bu kurs Unity 6.3 LTS ile hazırlandı. Telefona kurulan uygulama dosyası (APK / iOS) bitirme işinde istenmez.
Yapay zekâ kullandıysanız teslimde hangi parçayı asistanın yazdığını bir cümleyle belirtin.
