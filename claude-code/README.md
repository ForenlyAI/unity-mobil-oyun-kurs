# Claude Code'a verilen gerçek istekler

Bu klasördeki dosyalar kurs hazırlanırken yapılan gerçek oturumlardan:

- `istek-1.4.txt` → `oneri-1.4.txt`: "hedefe adım kadar yaklaş" işlevi. Yazılan sürüm altı testin beşini geçti
  (`../beklenen/ders-1.4-claude-surumu.xml`); kalan test istekte söylenmeyen bir durumdu (eksi adım). Projedeki `Hareket.cs` düzeltilmiş sürümdür.
- `istek-3.4.txt` → `oneri-3.4.txt`: "daire kutuya değiyor mu" işlevi. Yazılan sürüm altı testin altısını geçti; projedeki `SeviyeDenetimi.cs` içinde kullanılıyor.

Kendi isteğinizi yazarken: ne istediğinizi, hangi dosyaya yazılacağını ve **kenar durumlarını** söyleyin; kodu kabul etmeden önce testleri koşun.

Örnek istek (Claude Code'u proje klasöründe açtıktan sonra):

> Bu bir Unity 6.3 projesi. Unity'yi yalnız `unity` komut satırı aracıyla kullan. `Assets/Kafe/Scripts/Kurallar/Puan.cs` içindeki
> çarpma cezasını iki yüze çıkar, ilgili testlerin beklenen değerlerini güncelle, `unity test . --mode EditMode` koş ve raporu özetle.
