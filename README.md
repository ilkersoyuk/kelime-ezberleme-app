# Kelime Ezber Pro • YDS & YÖKDİL Hazırlık Uygulaması (C# Blazor PWA)

Bu uygulama, **Zafer Hoca YDS/YÖKDİL Çıkmış Kelimeler Listesi (1124 Kelime)** ve **YDS'de En Çok Çıkan Sözcükler (1000 Kelime)** olmak üzere toplam **2124 kelimelik** devasa kelime havuzunu içeren, **C# (.NET 9 Blazor WebAssembly)** mimarisiyle geliştirilmiş, mobil öncelikli bir PWA (Progressive Web App) kelime ezberleme uygulamasıdır.

Referans proje: [KPSS Takip+](https://97eminblgn.github.io/kpss-takip/) tasarım standartlarına uygun olarak sade, şık, göz yormayan gece mavisi / indiqo / zümrüt renk paletiyle tasarlanmıştır.

---

## 🌟 Öne Çıkan Özellikler

1. **Mobil Öncelikli & PWA (Ana Ekrana Ekle)**:
   - Tarayıcıdan açıldığında *"Ana Ekrana Ekle"* seçeneğiyle telefonunuza gerçek bir mobil uygulama gibi kurulur (URL çubuğu olmadan tam ekran çalışır).
   - Servis çalışanı (`service-worker.js`) sayesinde **çevrimdışı (offline)** internet olmadan da çalışır.

2. **2 Büyük Kelime Havuzu (Toplam 2,124 Kelime)**:
   - **Zafer Hoca Çıkmış Kelimeler (1124 Kelime)**: Kelime, Türkçe telaffuz/okunuş, Türkçe anlam ve ünite takibi.
   - **YDS Sık Çıkanlar (1000 Kelime)**: Kelime, eş anlamlılar (`= synonyms`), örnek cümleler, bağlamlar ve kalıplar.
   - Tüm kelimeler 50'şer kelimelik kolay tamamlanabilir ünitelere ayrılmıştır.

3. **Oyunlaştırma & Seviye Sistemi (Gamification)**:
   - **20+ Seviye & Unvanlar**: Seviye 1 (Kelime Çırağı 🐣) ➔ Seviye 20+ (YDS Efsanesi 🔥).
   - **XP Puanlama**:
     - Ezberledim: +15 XP
     - Öğreniyorum: +8 XP
     - Doğru Quiz Cevabı: +12 XP
     - Eşleştirme Çifti: +6 XP
     - Görev Tamamlama: +35..60 XP
   - **Günlük Görevler (Missions)**: Her gece yenilenen 4 dinamik görev (Kart tekrarı, Quiz doğru sayısı, Ezber hedefi, Favorileme).
   - **Çalışma Serisi (Streak 🔥)**: Günlük düzenli çalışma takibi.
   - **Başarı Rozetleri (Badges)**: İlk Adım, Kelime Avcısı, Test Kurdu, YDS Ustası vb.

4. **Ezberleme Modları**:
   - **3D Kelime Kartları (Flashcards)**: Çift taraflı dokunmatik çevirme, orijinal İngilizce sesli okuma (🔊 TTS), Leitner ezber durumları (*Zor/Tekrar*, *Öğreniyorum*, *Ezberledim*).
   - **4 Şıklı Çoktan Seçmeli Test (Quiz)**: Dinamik şıklar, anında yeşil/kırmızı görsel geri bildirim, titreşimli geribildirim, sonuç karnesi ve yanlış yapılan kelimeleri tek tıkla favorilere ekleme.
   - **Hızlı Eşleştirme Oyunu (Speed Match)**: 5 İngilizce - 5 Türkçe kutucuğu hızla eşleştirme mini oyunu.
   - **Sözlük & Anlık Arama**: Kelimeye göre Türkçe veya İngilizce anında arama, ünite ve favori filtreleri, detay modalı.

5. **Veri Yedekleme & Geri Yükleme (JSON Backup)**:
   - İlerlemeyi kaybetmemek için tek tıkla telefonunuza `.json` yedeği indirin.
   - Başka bir telefona geçtiğinizde *"Yedeği Yükle"* diyerek tüm XP, seviye ve ezber geçmişinizi anında geri yükleyin.

---

## 🚀 Bilgisayarda Yerel Olarak Çalıştırma

Terminal veya PowerShell'de bu klasördeyken:

```bash
dotnet run
```

Uygulama otomatik olarak `http://localhost:5298` adresinde çalışacaktır. Tarayıcınızdan bu adrese girerek hemen kullanabilirsiniz.

---

## 🌐 GitHub Pages Üzerinde Yayınlama Rehberi
*(Tıpkı `https://97eminblgn.github.io/kpss-takip/` gibi internette yayınlamak için)*

1. GitHub'da yeni bir repository (depo) oluşturun (Örn: `kelime-ezber`).
2. Bu klasördeki dosyaları repoya yükleyin (git push):
   ```bash
   git init
   git add .
   git commit -m "İlk sürüm: YDS Kelime Ezber PWA"
   git branch -M main
   git remote add origin https://github.com/<kullanici-adiniz>/<repo-adiniz>.git
   git push -u origin main
   ```
3. GitHub deponuzun **Settings > Pages** sekmesine gidin.
4. Kaynak olarak `GitHub Actions` veya `dist/wwwroot` branchini seçin.
5. Yayınlanan linke (Örn: `https://<kullanici-adiniz>.github.io/<repo-adiniz>/`) telefonunuzun tarayıcısından girip **"Ana Ekrana Ekle"** dediğinizde uygulamanız telefonunuzda hazır!
