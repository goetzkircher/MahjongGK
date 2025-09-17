## Sicherheit & Signierung

Alle Assemblies dieses Projekts sind mit einem **Strong Name** signiert.

- Der Schlüssel (`keys/MahjongGK.snk`) liegt **bewusst im Projektordner**,
  damit jeder Fork oder lokale Build ohne Zusatzaufwand funktioniert.
- Der Schlüssel ist **nicht geheim**; der Zweck ist die **Assembly-Identität**, nicht Kryptografie-Schutz.
- Für eine Open-Source-Anwendung wie MahjongGK ist das üblich und unkritisch.

**PublicKeyToken der Kern-Assembly:**
`da4bd985dc23bd93`

### Integritätsprüfung

1. Laden Sie das Release von GitHub Releases.
2. Ermitteln Sie den Hash Ihrer Datei (z. B. in PowerShell):
   ```powershell
   Get-FileHash .\MahjongGK.exe -Algorithm SHA256
   ```
   → vergleichen Sie mit der beigefügten `SHA256SUMS.txt`.
3. Optional: Strong-Name-Token prüfen:
   ```powershell
   [System.Reflection.AssemblyName]::GetAssemblyName('.\MahjongGK.Core.dll').FullName
   ```
   Das Ergebnis muss das oben angegebene **PublicKeyToken** enthalten.
