<h1 align="center" id="title">eBelge_DataSyncHub</h1>

<p id="description">
  * Bu proje e-Belge, e-Defter, e-Beyanname ve OKC sitelerindeki duyuruları tarar. Eğer güncel bir duyuru bulunursa bu bilgiyi Firebase'e kaydeder ve aynı zamanda One Signal aracılığıyla bildirim gönderir. Otomatik olarak her saat başı yeniden çalışan Windows service projesidir.</p>

  
  
<h2>🧐 Features</h2>

Here're some of the project's best features:

*   https://ebelge.gib.gov.tr/duyurular.html sayfasındaki güncel duyuruları tarar.
*   https://www.edefter.gov.tr/duyurular.html sayfasındaki güncel duyuruları tarar.
*   https://www.defterbeyan.gov.tr/tr/duyurular sayfasındaki güncel duyuruları tarar.
*   https://ynokc.gib.gov.tr/Home/DuyuruArsiv sayfasındaki güncel duyuruları tarar.
*   OneSignal Push Notification
*   Firebase Database
*   Hata Yönetimi (Olay Görüntüleyicisi)

<h2>🛠️ Installation Steps:</h2>

<p>1. Install NuGet Package</p>

```
FirebaseDatabase.net
```

<p>2. Install NuGet Package</p>

```
HtmlAgilityPack
```

<p>3. Install NuGet Package</p>

```
Newtonsoft.Json
```

<p>4. Edit Settings.json</p>

```
oneSignal > apiKey
```

<p>5. Edit Settings.json</p>

```
OneSignal > appId
```

<p>6. Edit Settings.json</p>

```
fireBaseUri
```

<p>7. After the project is compiled.</p>

```
Start command prompt as administrator and run this command. sc create eBelge_DataSyncHub binPath= "..X\eBelge_DataSyncHub.exe"
```

  
  
<h2>💻 Built with</h2>

Technologies used in the project:

*   C#
*   Windows Service
