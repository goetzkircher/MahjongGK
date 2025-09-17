# MahjongGK – Mahjong Solitär

**MahjongGK** ist ein Open-Source-Projekt für ein Mahjong-Solitär-Spiel mit eigenem Editor, Renderer und umfangreicher Anpassbarkeit.  
Es wird in **VB.NET (.NET Framework 4.8.1)** entwickelt und legt besonderen Wert auf saubere Architektur, flexible Erweiterbarkeit und eine klare Trennung von Daten, Logik und Darstellung.

---

## Features

- 🎴 Eigenes Layout- und Stein-Management  
- 🎨 Unterstützung von Bitmap, SVG und WebP-Grafiken  
- ⚙️ Editor zum Erstellen und Bearbeiten von Spielfeldern  
- 🌓 Anpassbares Light- und Dark-Theme  
- 🧩 Offene Architektur für Erweiterungen  

---

## Lizenz

Dieses Projekt steht unter der  
**GNU General Public License v3.0 or later (GPL-3.0-or-later)**  

Das bedeutet:  
- Du darfst den Quellcode frei verwenden, verändern und weitergeben.  
- Alle abgeleiteten Werke müssen ebenfalls unter der GPL veröffentlicht werden.  
- Es besteht **keine Garantie oder Haftung** für Schäden, die durch die Nutzung entstehen.  

👉 Siehe die [LICENSE](LICENSE)-Datei für den vollständigen Lizenztext.  

---

## Sicherheit & Signierung

Alle Assemblies dieses Projekts sind mit einem **Strong Name** signiert.

- Der Schlüssel (`keys/MahjongGK.snk`) liegt **bewusst im Projektordner**,  
  damit jeder Fork oder lokale Build ohne Zusatzaufwand funktioniert.  
- Das bedeutet: der Schlüssel ist **nicht geheim** – der Zweck ist hier
  lediglich die **eindeutige Assembly-Identität**, nicht Kryptografie-Schutz.  
- Für eine Open-Source-Anwendung wie MahjongGK ist das üblich und unkritisch.

**PublicKeyToken der Kern-Assembly:**  
`<hier deinen Token einsetzen, z. B. abcd1234ef567890>`

### Integritätsprüfung

1. Laden Sie das Release von [GitHub Releases](./releases).  
2. Ermitteln Sie den Hash Ihrer Datei (z. B. in PowerShell):  
   ``powershell
   Get-FileHash .\MahjongGK.exe -Algorithm SHA256


## Drittanbieter-Bibliotheken

Dieses Projekt verwendet externe Open-Source-Komponenten:

- [**Magick.NET**](https://github.com/dlemstra/Magick.NET)  
  Apache License 2.0 – Bildbearbeitung und WebP-Unterstützung  

- [**SVG.NET**](https://github.com/svg-net/SVG)  
  Microsoft Public License (MS-PL) – SVG-Darstellung  

---

 ## Fotografien und Hintergrundbilder
 
  - [Pixabay](https://pixabay.com) – Pixabay License  
  - OpenAI „Sora“ – KI-generierte Bilder, frei nutzbar

---

## Copyright

© 2025–2026 Götz Kircher <mahjonggk@t-online.de>  
SPDX-License-Identifier: GPL-3.0-or-later  

---

## Mitmachen

Beiträge sind willkommen!  
Bitte schreib mir an MahjongGK@t-online.de, wenn du Ideen, Bugfixes oder Erweiterungen hast.  

---

## Über mich

Von Beruf bin ich Schreinermeister und Dipl.Ing. Holztechnik.
Ich programmiere seit den Anfängen des PC in den 1980er Jahren.
Heute - als Rentner - nur noch als Hobby.

Ich wohne in Deutschland/Hessen/Mittelhessen/Marburg.
