'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <MahjongGK@t-online.de>            #
'#                                                                         #
'#                     MahjongGK  -  Mahjong Solitär                       #
'#                                                                         #
'#   This program is free software: you can redistribute it and/or modify  #
'#   it under the terms of the GNU General Public License as published by  #
'#   the Free Software Fundament, either version 3 of the License, or     #
'#   at your option any later version.                                     #
'#                                                                         #
'#   This program is distributed in the hope that it will be useful,       #
'#   but WITHOUT ANY WARRANTY; without even the implied warranty of        #
'#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          #
'#   GNU General Public License for more details.                          #
'#   https://www.gnu.org/licenses/gpl-3.0.html                             #
'#                                                                         #
'###########################################################################
'
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
'
#Disable Warning IDE0079
#Disable Warning IDE1006


Imports System.IO
Imports System.Text
Imports System.Xml.Serialization

Public Class frmGfxCompiler

    Private Enum Geistertyp
        Normal
        Unsichtbar
        NotUsed
        WerkstückEinfügefehler
        WerkstückZufallsgrafik
    End Enum
    Private Class StatusDir
        Public Property Name As String
        Public Property Path As String
    End Class


    Private ReadOnly _rootAndere As String =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                     "Visual Studio", "MahjongGK", "Eigene Ressourcen Quelle", "Andere")

    Private ReadOnly _rootSteine As String =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                     "Visual Studio", "MahjongGK", "Eigene Ressourcen Quelle", "Mahjongsteine")

    'Hierhin kopiert dder GfxCompiler. Die IDE kopiert dann weiter in das Programmverzeichnis.
    Private ReadOnly _rootOutput As String =
               Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                     "Visual Studio", "MahjongGK", "MahjongGK", "Bitmaps Eigene Ressourcen")

    'Dahin verschiebt das Programm sie:
    'Private ReadOnly _outRoot As String =
    '    Path.Combine(Application.StartupPath, "Eigene Ressourcen")

    ' UI grob (Designerfrei):
    Private WithEvents btnCompileBasis As Button
    Private WithEvents btnCompileSteine As Button
    Private WithEvents btnInfo As Button
    Private WithEvents txtLog As TextBox
    Private WithEvents prg As ProgressBar

    Private Sub frmGfxCompiler_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "GfxCompiler-Compiler (SVG/BMP → XML)"
        Me.Width = 900
        Me.Height = 300
        Me.StartPosition = FormStartPosition.CenterParent

        btnCompileBasis = New Button() With {.Left = 50, .Top = 10, .Width = 300, .Text = "'Eigene Ressoucen\Andere'  kompilieren"}
        btnCompileSteine = New Button() With {.Left = 450, .Top = 10, .Width = 200, .Text = "'...\Mahjongsteine\' kompilieren"}
        btnInfo = New Button() With {.Left = 670, .Top = 10, .Width = 200, .Text = "Info"}

        prg = New ProgressBar() With {.Left = 12, .Top = 45, .Width = 860, .Height = 18}
        txtLog = New TextBox() With {.Left = 12, .Top = 75, .Width = 860, .Height = 470, .Multiline = True, .ScrollBars = ScrollBars.Both, .WordWrap = False}

        Me.Controls.AddRange(New Control() {btnCompileBasis, btnCompileSteine, btnInfo, prg, txtLog})

        If Not Directory.Exists(_rootOutput) Then Directory.CreateDirectory(_rootOutput)
        Log($"Output: {_rootOutput}")
    End Sub

    Private Sub Log(msg As String)
        txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}")
    End Sub

    Private Sub btnInfo_Click(sender As Object, e As EventArgs) Handles btnInfo.Click
        Dim sb As New System.Text.StringBuilder
        sb.AppendLine("Dieses Tool sammelt die vom Programm benötigten Grafiken ein")
        sb.AppendLine("und erzeugt die notwendigen Xml-Dateien, die die Grafiken")
        sb.AppendLine("behinhalten sowie den notwendigen Code um die Grafiken zu")
        sb.AppendLine("verwalten und zur Verfügung zu stellen.")
        sb.AppendLine()
        sb.AppendLine("Die Pfade zu den Wurzelverzeichnissen der Grafiken sind")
        sb.AppendLine("hardcodiert. Anpassung in frmGfxCompiler gleich ganz oben.")

        MsgBox(sb.ToString, MsgBoxStyle.Information)
    End Sub
    ' ──────────────────────────────────────────────────────────────────────────
    '   BASIS-KOMPILIERUNG
    ' ──────────────────────────────────────────────────────────────────────────
    Private Sub btnCompileBasis_Click(sender As Object, e As EventArgs) Handles btnCompileBasis.Click

        Dim packs As New List(Of GfxPack)
        Dim packFiles As New List(Of String)


        For Each s As SteinSatz In DirectCast([Enum].GetValues(GetType(SteinSatz)), SteinSatz())
            If s = SteinSatz.None Then
                Continue For
            End If
            Try
                Dim basisName As String = s.ToString
                Dim basisDir As String = Path.Combine(_rootAndere, basisName)
                If Not Directory.Exists(basisDir) Then
                    Throw New DirectoryNotFoundException($"Basis-Verzeichnis nicht gefunden: {basisDir} - btnCompileBasis_Click")
                End If
                Dim outFile As String = Path.Combine(_rootOutput, $"Basis_{basisName}.gfx.xml")
                Dim p1 As GfxPack = CompileBasis(basisName, basisDir, outFile)
                packs.Add(p1) : packFiles.Add(outFile)

                Log($"OK: {outFile}")

            Catch ex As Exception
                Log("FEHLER (Basis): " & ex.Message)
            End Try
        Next

        WriteMjGfxModule(packs, packFiles)
    End Sub


    Private Function CompileBasis(basisName As String, basisDir As String, outFile As String) As GfxPack
        Log($"Kompiliere Basis: {basisName} aus {basisDir}")

        Dim files As List(Of String) =
        Directory.EnumerateFiles(basisDir, "*.*", SearchOption.AllDirectories) _
                 .Where(Function(p) p.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) OrElse
                                     p.EndsWith(".png", StringComparison.OrdinalIgnoreCase) OrElse
                                     p.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)) _
                 .ToList()

        Dim excludeSegment As String = Path.DirectorySeparatorChar & "Originale" & Path.DirectorySeparatorChar

        prg.Minimum = 0 : prg.Maximum = files.Count : prg.Value = 0

        ' Key = baseKey (ohne ##q), Wert = Liste Einträge (SVG/PNG)
        Dim dict As New Dictionary(Of String, List(Of GfxEntry))(StringComparer.OrdinalIgnoreCase)

        For Each f As String In files
            If f.IndexOf(excludeSegment, StringComparison.OrdinalIgnoreCase) >= 0 Then
                Continue For
            End If

            prg.Value += 1

            ' Relativer Ordner vom basisDir (für Folder)
            Dim relDir As String =
            Path.GetDirectoryName(f).Substring(basisDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            relDir = relDir.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)

            Dim nameNoExt As String = Path.GetFileNameWithoutExtension(f)
            Dim q As Integer? = Nothing
            Dim baseKey As String = StripSizeQ(nameNoExt, q) ' ##q abschneiden
            Dim sizeHint As String = If(q.HasValue, $"{q.Value}q", "")

            Dim entry As New GfxEntry() With {
            .Key = baseKey,
            .SizeHint = sizeHint,
            .Folder = relDir,            ' <-- wichtig für ImageList
            .BaseName = baseKey          ' <-- stabiler Sortiername
        }

            If f.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) Then
                entry.Kind = "svg"
                Dim txt As String = ReadTextFromFileUtf8NoBom(f)
                Dim payload As Byte() = System.Text.Encoding.UTF8.GetBytes(txt)
                entry.DataGzBase64 = DeflateGZip(payload)
            Else
                entry.Kind = "png"
                Dim bytes() As Byte = ReadBytesFromFile(f)
                entry.DataGzBase64 = DeflateGZip(bytes)
            End If

            If Not dict.ContainsKey(baseKey) Then dict(baseKey) = New List(Of GfxEntry)()
            dict(baseKey).Add(entry)
        Next

        ' Pack erzeugen: nur Key-gruppiert übernehmen (SVG > PNG Auswahl später zur Laufzeit)
        Dim pack As New GfxPack With {.Name = basisName}
        For Each kvp As KeyValuePair(Of String, List(Of GfxEntry)) In dict
            ' Wir speichern ALLE Varianten (SVG + PNG), die Laufzeit wählt die passende (SVG first)
            pack.Entries.AddRange(kvp.Value)
        Next

        Dim ser As New XmlSerializer(GetType(GfxPack))
        Using fs As FileStream = File.Create(outFile)
            ser.Serialize(fs, pack)
        End Using

        Return pack
    End Function

    ' ──────────────────────────────────────────────────────────────────────────
    '   STEIN-KOMPILIERUNG (VALIDIEREND)
    ' ──────────────────────────────────────────────────────────────────────────
    Private Sub btnCompileSteine_Click(sender As Object, e As EventArgs) Handles btnCompileSteine.Click


        Try
            If Not Directory.Exists(_rootSteine) Then
                Throw New DirectoryNotFoundException("Mahjongsteine-Root fehlt: " & _rootSteine & " - btnCompileSteine_Click")
            End If

            ' Alle Steinsatz-Ordner unter Mahjongsteine durchgehen (Ordnername muss Enum-Namen entsprechen)
            Dim satzDirs() As String = Directory.GetDirectories(_rootSteine)
            If satzDirs.Length = 0 Then
                Throw New InvalidOperationException("Keine Steinsatz-Unterordner gefunden. - btnCompileSteine_Click")
            End If

            For Each Dir As String In satzDirs
                Dim satzName As String = Path.GetFileName(Dir)
                Dim satz As SteinSatz
                If Not [Enum].TryParse(Of SteinSatz)(satzName, ignoreCase:=True, result:=satz) Then
                    Log($"Überspringe Ordner (kein gültiger SteinSatz): {Dir}  - btnCompileSteine_Click")
                    Continue For
                End If
                If satz <> SteinSatz.None Then
                    Dim outFile As String = Path.Combine(_rootOutput, $"Steine_{satz}.xml")
                    CompileSteinSatz(Dir, satz, outFile)
                    Log($"OK: {outFile}")
                End If
            Next
        Catch ex As Exception
            Log("FEHLER (Steine): " & ex.Message & " - btnCompileSteine_Click")
        End Try

    End Sub

    Private Sub CompileSteinSatz(satzDir As String, steinSatz As SteinSatz, outFile As String)

        Log($"Kompiliere Steinsatz: {steinSatz} aus {satzDir}")

        ' Erwartete Status-Ordner
        Dim allStatusNames() As String = [Enum].GetNames(GetType(SteinStatus))
        Dim statusDirs As List(Of StatusDir) =
            allStatusNames.Select(Function(n) New StatusDir With {
                .Name = n,
                .Path = Path.Combine(satzDir, n)
            }).ToList()

        ' Mindestens einer muss existieren; besser alle. Fehlt einer → wir kompilieren trotzdem, aber Entries daraus werden Ghosts zur Laufzeit.
        If Not statusDirs.Any(Function(sd) Directory.Exists(sd.Path)) Then
            Throw New DirectoryNotFoundException("Es existiert kein einziger SteinStatus-Unterordner für " & steinSatz.ToString() & " - CompileSteinSatz")
        End If

        Dim pack As New SteinPack With {.SteinSatz = steinSatz.ToString()}
        Dim refW As Integer = 0, refH As Integer = 0
        Dim allIndexNames As HashSet(Of String) =
             [Enum].GetNames(GetType(SteinIndexEnum)).ToHashSet(StringComparer.OrdinalIgnoreCase)


        'zuerst nach der Größe suchen
        For Each sd As StatusDir In statusDirs
            If Not Directory.Exists(sd.Path) Then Continue For
            Dim pngs As List(Of String) = Directory.EnumerateFiles(sd.Path, "*.png", SearchOption.TopDirectoryOnly).ToList()
            For Each file As String In pngs
                Dim bytes() As Byte = ReadBytesFromFile(file)
                Using ms As New MemoryStream(bytes)
                    Using img As Image = System.Drawing.Image.FromStream(ms)
                        refW = img.Width
                        refH = img.Height
                        Exit For
                    End Using
                End Using
                If refW > 0 AndAlso refH > 0 Then Exit For
            Next
            If refW > 0 AndAlso refH > 0 Then Exit For
        Next

        If refW <= 0 OrElse refH <= 0 Then
            refW = INI.GfxCompiler_GhostWitdth
            refH = INI.GfxCompiler_GhostHeight
        End If

        For Each sd As StatusDir In statusDirs


            'Status aus dem Verzeichnisnamen rückgewinnen
            Dim steinStatus As SteinStatus = DirectCast([Enum].Parse(GetType(SteinStatus), sd.Name, ignoreCase:=True), SteinStatus)

            If Not Directory.Exists(sd.Path) Then
                Log($"Warnung: Status-Ordner fehlt: {sd.Path} → Laufzeit zeigt Ghosts - CompileSteinSatz")
                Continue For
            End If

            ' Alle PNGs lesen
            Dim pngs As List(Of String) = Directory.EnumerateFiles(sd.Path, "*.png", SearchOption.TopDirectoryOnly).ToList()
            ' Validierung: es dürfen nur Dateinamen sein, die Enum-Namen exakt treffen
            Dim indexFound As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)


            For Each file As String In pngs

                Dim nameNoExt As String = Path.GetFileNameWithoutExtension(file)

                If Not allIndexNames.Contains(nameNoExt) Then
                    Throw New InvalidDataException($"Überzählige oder ungültige PNG im Status '{sd.Name}': {nameNoExt} -  CompileSteinSatz")
                End If

                ' Bytes lesen
                Dim bytes() As Byte = ReadBytesFromFile(file)

                ' Breite/Höhe prüfen (über Image)
                Using ms As New MemoryStream(bytes)
                    Using img As Image = System.Drawing.Image.FromStream(ms)
                        Dim w As Integer = img.Width
                        Dim h As Integer = img.Height
                        If w <> refW OrElse h <> refH Then
                            Throw New InvalidDataException($"Größen-Mismatch in '{sd.Name}\{nameNoExt}.png' erwartet {refW}x{refH}, gefunden {w}x{h} -  CompileSteinSatz")
                        End If
                    End Using
                End Using

                Dim entry As New SteinEntry With {
                    .Status = sd.Name,
                    .Index = nameNoExt,
                    .PngGzBase64 = DeflateGZip(bytes)
                }
                pack.Entries.Add(entry)
                indexFound.Add(nameNoExt)
            Next

            ' Vollständigkeit pro Status prüfen (optional „hart“ → Exception)
            Dim missing As List(Of String) = allIndexNames.Except(indexFound, StringComparer.OrdinalIgnoreCase).ToList()
            If missing.Count = 43 Then
                If refW = 0 OrElse refH = 0 Then
                    refW = INI.GfxCompiler_GhostWitdth
                    refH = INI.GfxCompiler_GhostHeight
                End If
                'alle fehlen
                Dim size As New Size(refW, refH)
                Select Case steinStatus
                    Case SteinStatus.I00Unsichtbar
                        AddSteinStatusGhosts(pack, indexFound, size, steinStatus, Geistertyp.Unsichtbar)

                    Case SteinStatus.I06NotUnsed
                        AddSteinStatusGhosts(pack, indexFound, size, steinStatus, Geistertyp.NotUsed)

                    Case SteinStatus.I08WerkstückEinfügeFehler
                        AddSteinStatusGhosts(pack, indexFound, size, steinStatus, Geistertyp.WerkstückEinfügefehler)

                    Case SteinStatus.I09WerkstückZufallsgrafik
                        AddSteinStatusGhosts(pack, indexFound, size, steinStatus, Geistertyp.WerkstückEinfügefehler)

                    Case Else
                        AddSteinStatusGhosts(pack, indexFound, size, steinStatus, Geistertyp.Normal)

                End Select
            ElseIf missing.Count > 0 Then
                Throw New InvalidDataException($"Fehlende PNGs im Status '{sd.Name}': {String.Join(", ", missing)} -  CompileSteinSatz")
            End If
        Next

        If refW <= 0 OrElse refH <= 0 Then
            Throw New InvalidDataException($"Keine PNGs gefunden. -  CompileSteinSatz")
        End If

        pack.RefWidth = refW
        pack.RefHeight = refH

        ' Schreiben
        Dim ser As New XmlSerializer(GetType(SteinPack))
        Using fs As FileStream = File.Create(outFile)
            ser.Serialize(fs, pack)
        End Using
    End Sub


    ''Private Function TryParseSetNumberFromDirName(dir As DirectoryInfo, ByRef setNo As Integer) As Boolean
    ''    setNo = 0
    ''    If dir Is Nothing Then Return False
    ''    Dim name As String = dir.Name
    ''    ' "Satz12", "satz3", etc.
    ''    Dim digits As String = New String(Array.FindAll(name.ToCharArray(), Function(c) Char.IsDigit(c)))
    ''    If digits.Length = 0 Then Return False
    ''    Dim n As Integer
    ''    If Integer.TryParse(digits, n) AndAlso n > 0 Then
    ''        setNo = n
    ''        Return True
    ''    End If
    ''    Return False
    ''End Function

    ' ''' Liefert Grundfarbe (RGB) und Form (Kreis/Rechteck) gemäß deiner Regel:
    ' ''' 1: Rot, 2: Grün, 3: Blau, 4: Cyan, 5: Magenta, 6: Gelb, dann Farben wiederholen – aber Rechteck statt Kreis.
    ''Private Sub GetSetColorAndShape(setNo As Integer, ByRef baseRgb As Color, ByRef useRectangle As Boolean)
    ''    If setNo <= 0 Then setNo = 1
    ''    Dim idx As Integer = ((setNo - 1) Mod 6) + 1
    ''    Select Case idx
    ''        Case 1 : baseRgb = Color.FromArgb(255, 0, 0)      ' Rot
    ''        Case 2 : baseRgb = Color.FromArgb(0, 255, 0)      ' Grün
    ''        Case 3 : baseRgb = Color.FromArgb(0, 0, 255)      ' Blau
    ''        Case 4 : baseRgb = Color.FromArgb(0, 255, 255)    ' Cyan (Gegenfarbe zu Rot)
    ''        Case 5 : baseRgb = Color.FromArgb(255, 0, 255)    ' Magenta (Gegenfarbe zu Grün)
    ''        Case 6 : baseRgb = Color.FromArgb(255, 255, 0)    ' Gelb (Gegenfarbe zu Blau)
    ''        Case Else : baseRgb = Color.FromArgb(128, 128, 128)
    ''    End Select
    ''    ' 1..6 Kreis, 7..12 Rechteck, 13..18 Kreis, ...
    ''    useRectangle = (((setNo - 1) \ 6) Mod 2) = 1
    ''End Sub

    ' ''' Ermittelt die numerische Enum-ID aus dem Namen (z. B. "Bambus3" → 42).
    ' ''' Passe "SteinIndexEnum" an deinen Enumtyp an.
    ''Private Function TryGetEnumNumericValue(enumType As Type, name As String, ByRef value As Integer) As Boolean
    ''    value = 0
    ''    If enumType Is Nothing OrElse Not enumType.IsEnum Then Return False
    ''    If String.IsNullOrWhiteSpace(name) Then Return False
    ''    Try
    ''        Dim obj As Object = [Enum].Parse(enumType, name, ignoreCase:=True)
    ''        value = CInt(obj)
    ''        Return True
    ''    Catch
    ''        Return False
    ''    End Try
    ''End Function

    Private Sub AddSteinStatusGhosts(ByRef pack As SteinPack, indexFound As HashSet(Of String), size As Size, steinStatus As SteinStatus, geistertyp As Geistertyp)

        Dim lob As New List(Of Bitmap)

        ' For Each steinStatus As SteinStatus In DirectCast([Enum].GetValues(GetType(SteinStatus)), SteinStatus())
        For Each sie As SteinIndexEnum In DirectCast([Enum].GetValues(GetType(SteinIndexEnum)), SteinIndexEnum())

            Dim bmp As Bitmap = CreateGhostBitmap(size, INI.GfxCompiler_GhostBasisColor(steinStatus), useRectangle:=False, steinStatus, geistertyp)
            lob.Add(bmp)

            Dim bytes() As Byte = ReadBytesFromBitmap(bmp)

            Dim entry As New SteinEntry With {
                    .Status = steinStatus.ToString,
                    .Index = sie.ToString,
                    .PngGzBase64 = DeflateGZip(bytes)
                }
            pack.Entries.Add(entry)
            indexFound.Add(sie.ToString)
        Next

        ''MjDebug.DebugBitmaps.DebugShowArrayBitmaps(lob.ToArray)

        ''MjDebug.DebugStep.WaitForStep()

    End Sub


    ' Zeichnet eine einzelne Geistergrafik.
    ' - Hintergrund: transparent
    ' - Grundform: Kreis ODER Rechteck, Füllung = baseRgb mit Alpha GHOST_SHAPE_ALPHA, Rand = baseRgb voll deckend
    ' - Text: numerischer Enum-Wert, zentriert, Schriftfarbe = baseRgb voll deckend
    Private Function CreateGhostBitmap(size As Size, baseRgb As Color, useRectangle As Boolean, enumNumber As Integer, geistertyp As Geistertyp) As Bitmap
        ' ─────────────────────────────────────────────────────────────────────────────
        ' Robust: Zielgröße bestimmen (inkl. Schatten). 0 → INI-Defaults.
        ' ─────────────────────────────────────────────────────────────────────────────
        Dim W As Integer = If(size.Width > 0, size.Width, INI.GfxCompiler_GhostWitdth)
        Dim H As Integer = If(size.Height > 0, size.Height, INI.GfxCompiler_GhostHeight)

        Dim diag As Double = Math.Sqrt(CDbl(W) * W + CDbl(H) * H)

        ' Schattenbreiten in Pixeln (Min 0)
        Dim shR As Single = CSng(Math.Max(0.0, Math.Round(diag * INI.GfxCompiler_GhostShadowRightFaktor)))
        Dim shB As Single = CSng(Math.Max(0.0, Math.Round(diag * INI.GfxCompiler_GhostShadowBottomFaktor)))

        ' Arbeitsfläche: Größe inklusive Schatten
        Dim bmp As New Bitmap(W, H, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic

            ' ─────────────────────────────────────────────────────────────────────────
            ' Untergrund füllen (sichtbar in Ecken/Radius): neutrales Grau aus INI
            ' ─────────────────────────────────────────────────────────────────────────
            g.Clear(INI.GfxCompiler_GhostUGrdColor)

            ' ClientRect ohne Schatten (hier kommt der Stein rein)
            Dim clientRect As New RectangleF(0.0F, 0.0F, W - shR, H - shB)

            ' Leichter Außenabstand, damit Linien/Antialias nicht abgeschnitten werden
            Dim edgePad As Single = 0.75F
            Dim outerRect As RectangleF = RectangleF.Inflate(clientRect, -edgePad, -edgePad)

            ' Abrundungsradius (bezug auf Diagonale), geclamped
            Dim curvF As Double = Math.Max(0.0, Math.Min(0.25, INI.GfxCompiler_GhostCurvatureFaktor)) ' 0..25% der Diagonale
            Dim radius As Single = CSng(diag * curvF)

            ' Innenlinie: Abstand & Strichbreite (min. 1px)
            Dim padF As Double = Math.Max(0.0, INI.GfxCompiler_GhostPaddingFaktor)
            Dim frameF As Double = Math.Max(0.0, INI.GfxCompiler_GhostFrameStrokeWidthFaktor)
            Dim innerPad As Single = CSng(Math.Max(1.0, diag * padF))
            Dim frameW As Single = CSng(Math.Max(1.0, diag * frameF))

            ' Farben
            Dim fillCol As Color = Color.FromArgb(255, baseRgb) ' voll deckend, Alpha später einheitlich gesetzt
            Dim strokeCol As Color = Color.FromArgb(255, baseRgb)
            Dim hiCol As Color = Color.FromArgb(64, Color.White) ' zarte Highlights
            Dim loCol As Color = Color.FromArgb(48, Color.Black) ' zarte Schattenkante

            ' ─────────────────────────────────────────────────────────────────────────
            ' 1) Form füllen (Ellipse oder abgerundetes Rechteck)
            ' ─────────────────────────────────────────────────────────────────────────
            Using brFill As New SolidBrush(fillCol)
                If useRectangle Then
                    Using gp As Drawing2D.GraphicsPath = MakeRoundedRectPath(outerRect, radius)
                        g.FillPath(brFill, gp)
                    End Using
                Else
                    g.FillEllipse(brFill, outerRect)
                End If
            End Using

            ' ─────────────────────────────────────────────────────────────────────────
            ' 2) dezente Kanten-Modulation (Top-Left leicht heller, Bottom-Right dunkler)
            ' ─────────────────────────────────────────────────────────────────────────
            ' Dünner innerer Lichtsaum oben/links
            Using penHi As New Pen(hiCol, Math.Max(1.0F, frameW * 0.6F)) With {.Alignment = Drawing2D.PenAlignment.Inset}
                If useRectangle Then
                    Using gp As Drawing2D.GraphicsPath = MakeRoundedRectPath(outerRect, radius)
                        g.DrawPath(penHi, gp)
                    End Using
                Else
                    g.DrawEllipse(penHi, outerRect)
                End If
            End Using
            ' Zarter Schattensaum unten/rechts
            Using penLo As New Pen(loCol, Math.Max(1.0F, frameW * 0.6F)) With {.Alignment = Drawing2D.PenAlignment.Inset}
                ' Trick: Pfad minimal nach rechts/unten versetzen
                Dim shifted As RectangleF = outerRect
                shifted.Offset(Math.Max(1.0F, frameW * 0.5F), Math.Max(1.0F, frameW * 0.5F))
                If useRectangle Then
                    Using gp As Drawing2D.GraphicsPath = MakeRoundedRectPath(shifted, Math.Max(0.0F, radius - frameW * 0.5F))
                        g.DrawPath(penLo, gp)
                    End Using
                Else
                    g.DrawEllipse(penLo, shifted)
                End If
            End Using

            ' ─────────────────────────────────────────────────────────────────────────
            ' 3) Innenlinie (nach innen versetzt)
            ' ─────────────────────────────────────────────────────────────────────────
            Dim innerRect As RectangleF = RectangleF.Inflate(outerRect, -innerPad, -innerPad)
            Dim innerRadius As Single = Math.Max(0.0F, radius - innerPad)
            Using penInner As New Pen(strokeCol, frameW) With {.Alignment = Drawing2D.PenAlignment.Inset}
                If useRectangle Then
                    Using gp As Drawing2D.GraphicsPath = MakeRoundedRectPath(innerRect, innerRadius)
                        g.DrawPath(penInner, gp)
                    End Using
                Else
                    g.DrawEllipse(penInner, innerRect)
                End If
            End Using

            ' ─────────────────────────────────────────────────────────────────────────
            ' 4) Text (Enum-Nummer) – groß, mittig
            ' ─────────────────────────────────────────────────────────────────────────
            Dim textRect As RectangleF = RectangleF.Inflate(innerRect, -Math.Max(1.0F, frameW), -Math.Max(1.0F, frameW))
            Dim text As String = enumNumber.ToString(Globalization.CultureInfo.InvariantCulture)

            ' Greife auf verfügbare Fläche zu; starte groß und reduziere, bis es passt
            Dim sf As New StringFormat() With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center,
            .Trimming = StringTrimming.EllipsisCharacter,
            .FormatFlags = StringFormatFlags.NoClip
        }
            Dim font As Font = Nothing
            Try
                Dim sMin As Single = Math.Min(textRect.Width, textRect.Height)
                Dim fSize As Single = Math.Max(8.0F, sMin * 0.7F)
                Dim measured As SizeF
                Do
                    If font IsNot Nothing Then font.Dispose()
                    font = New Font("Segoe UI Semibold", fSize, FontStyle.Bold, GraphicsUnit.Pixel)
                    measured = g.MeasureString(text, font, New SizeF(1000.0F, 1000.0F), sf)
                    If measured.Width <= textRect.Width AndAlso measured.Height <= textRect.Height Then Exit Do
                    fSize -= Math.Max(1.0F, fSize * 0.08F)
                Loop While fSize > 6.0F

                Using brText As New SolidBrush(Color.Black)
                    g.DrawString(text, font, brText, textRect, sf)
                End Using
            Finally
                If font IsNot Nothing Then font.Dispose()
            End Try

            ' ─────────────────────────────────────────────────────────────────────────
            ' 5) Schatten rechts & unten (Gradienten in die reservierten Bereiche)
            ' ─────────────────────────────────────────────────────────────────────────
            If shR > 0.5F Then
                Dim r As New RectangleF(clientRect.Right, clientRect.Top + radius * 0.4F, shR, clientRect.Height - radius * 0.4F)
                Using lgb As New Drawing2D.LinearGradientBrush(r, Color.FromArgb(140, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), Drawing2D.LinearGradientMode.Horizontal)
                    g.FillRectangle(lgb, r)
                End Using
            End If
            If shB > 0.5F Then
                Dim r As New RectangleF(clientRect.Left + radius * 0.4F, clientRect.Bottom, clientRect.Width - radius * 0.4F, shB)
                Using lgb As New Drawing2D.LinearGradientBrush(r, Color.FromArgb(140, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), Drawing2D.LinearGradientMode.Vertical)
                    g.FillRectangle(lgb, r)
                End Using
            End If

            ' Abgerundete Ecke unten rechts etwas abdunkeln (Eckschatten)
            If shR > 0.5F OrElse shB > 0.5F Then
                Dim cr As Single = Math.Min(20.0F, radius * 1.2F)
                Dim corner As New RectangleF(
                                clientRect.Right - cr,
                                clientRect.Bottom - cr,
                                cr + shR,
                                cr + shB
                            )

                Using gp As New Drawing2D.GraphicsPath()
                    gp.AddEllipse(corner) ' ← Gradient-Form

                    Using pgb As New Drawing2D.PathGradientBrush(gp)
                        pgb.WrapMode = Drawing2D.WrapMode.Clamp
                        pgb.CenterColor = Color.FromArgb(120, 0, 0, 0)          ' innen dunkler
                        pgb.SurroundColors = New Color() {Color.FromArgb(0, 0, 0, 0)} ' außen ausfaden
                        ' Optional: Center leicht in die Ecke ziehen
                        ' pgb.CenterPoint = New PointF(clientRect.Right, clientRect.Bottom)

                        g.FillPath(pgb, gp) ' ← WICHTIG: den Pfad füllen, nicht FillEllipse
                    End Using
                End Using
            End If
        End Using

        ' ─────────────────────────────────────────────────────────────────────────────
        ' Einheitliche Transparenz ganz zum Schluss
        ' ─────────────────────────────────────────────────────────────────────────────
        MjGDI.SetAlphaInPlace(bmp, CByte(Math.Max(0, Math.Min(255, INI.GfxCompiler_GhostTransparenzUnsichtbar))))

        Return bmp
    End Function

    ' Helper: Rounded-Rect Pfad
    Private Shared Function MakeRoundedRectPath(rect As RectangleF, radius As Single) As Drawing2D.GraphicsPath
        Dim r As Single = Math.Max(0.0F, Math.Min(radius, Math.Min(rect.Width, rect.Height) * 0.5F))
        Dim d As Single = r * 2.0F
        Dim gp As New Drawing2D.GraphicsPath()
        If r <= 0.1F Then
            gp.AddRectangle(rect)
            gp.CloseFigure()
            Return gp
        End If
        gp.AddArc(rect.X, rect.Y, d, d, 180, 90)
        gp.AddArc(rect.Right - d, rect.Y, d, d, 270, 90)
        gp.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90)
        gp.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90)
        gp.CloseFigure()
        Return gp
    End Function

    '### Erzeugen der "Eigene Ressourcen\Verwaltung\MjGfx.vb"

    ' Schreibt das generierte MjGfx.vb in den PROJEKTORDNER (nicht bin\Debug).
    ' packs: bereits deserialisierte Packs (oder direkt das Return von CompileBasis(...))
    ' packFiles: exakt diese Pack-XMLs werden im Modul registriert und zur Laufzeit geladen (relativ zu BaseDirectory)
    Friend Sub WriteMjGfxModule(packs As IEnumerable(Of GfxPack),
                            packFiles As IEnumerable(Of String),
                            Optional targetRelPath As String = "Eigene Ressourcen\Verwaltung\MjGfx.vb")

        If packs Is Nothing Then Throw New ArgumentNullException(NameOf(packs))
        If packFiles Is Nothing Then Throw New ArgumentNullException(NameOf(packFiles))

        ' ── Zielpfad: Projektwurzel ermitteln (BaseDirectory\..\..)
        Dim projectRoot As String = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\.."))
        Dim targetPath As String = Path.Combine(projectRoot, targetRelPath)
        Dim targetDir As String = Path.GetDirectoryName(targetPath)
        If String.IsNullOrEmpty(targetDir) Then Throw New InvalidOperationException("Ungültiger Zielpfad für MjGfx.vb.")
        Directory.CreateDirectory(targetDir)

        ' ─────────────────────────────────────────────────────────────────────────────
        ' Sammeln: normale Keys + ImageList-Gruppierung (Top-Ordner "ImageList_*")
        ' ─────────────────────────────────────────────────────────────────────────────
        Dim allKeys As New SortedSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ' Folder → (BaseName → Kandidatenliste)
        Dim folderMap As New Dictionary(Of String, Dictionary(Of String, List(Of GfxEntry)))(StringComparer.OrdinalIgnoreCase)

        For Each p As GfxPack In packs
            If p Is Nothing OrElse p.Entries Is Nothing Then Continue For
            For Each e As GfxEntry In p.Entries
                If e Is Nothing Then Continue For
                If String.IsNullOrWhiteSpace(e.Key) Then Continue For

                ' Top-Level-Ordner bestimmen
                Dim top As String = ""
                If Not String.IsNullOrEmpty(e.Folder) Then
                    Dim parts As String() = e.Folder.Split(New Char() {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar},
                                                       StringSplitOptions.RemoveEmptyEntries)
                    If parts.Length > 0 Then top = parts(0)
                End If

                If Not String.IsNullOrEmpty(top) AndAlso top.StartsWith("ImageList_", StringComparison.OrdinalIgnoreCase) Then
                    ' → in ImageList-Struktur einsortieren
                    Dim groups As Dictionary(Of String, List(Of GfxEntry)) = Nothing
                    If Not folderMap.TryGetValue(top, groups) Then
                        groups = New Dictionary(Of String, List(Of GfxEntry))(StringComparer.OrdinalIgnoreCase)
                        folderMap(top) = groups
                    End If
                    Dim bn As String = If(String.IsNullOrEmpty(e.BaseName), e.Key, e.BaseName)
                    Dim lst As List(Of GfxEntry) = Nothing
                    If Not groups.TryGetValue(bn, lst) Then
                        lst = New List(Of GfxEntry)()
                        groups(bn) = lst
                    End If
                    lst.Add(e)
                Else
                    ' → normale Einzelfunktion
                    allKeys.Add(e.Key)
                End If
            Next
        Next

        ' ─────────────────────────────────────────────────────────────────────────────
        ' Generieren: Modultext aufbauen
        ' ─────────────────────────────────────────────────────────────────────────────
        Dim sb As New StringBuilder(250000)

        ' Kopf + Warnzeile
        sb.AppendLine("' WICHTIG: Hier keine Änderungen vornehmen, sie werden gelöscht. Änderungen nur in frmGfxCompiler.WriteMjGfxModule vornehmen!")
        sb.AppendLine("Option Compare Text")
        sb.AppendLine("Option Explicit On")
        sb.AppendLine("Option Infer Off")
        sb.AppendLine("Option Strict On")
        sb.AppendLine()
        sb.AppendLine("Imports System")
        sb.AppendLine("Imports System.IO")
        sb.AppendLine("Imports System.IO.Compression")
        sb.AppendLine("Imports System.Drawing")
        sb.AppendLine("Imports System.Drawing.Drawing2D")
        sb.AppendLine("Imports System.Drawing.Imaging") ' <-- NEU (für ImageAttributes/ColorMatrix)
        sb.AppendLine("Imports System.Windows.Forms")
        sb.AppendLine("Imports System.Xml.Serialization")
        sb.AppendLine("Imports System.Text")
        sb.AppendLine("Imports System.Collections.Generic")
        sb.AppendLine("Imports System.Linq")
        sb.AppendLine("Imports Svg")
        sb.AppendLine()
        sb.AppendLine("''' <summary>")
        sb.AppendLine("''' Auto-generiertes Zugriffmodul auf GfxPack-Inhalte.")
        sb.AppendLine("''' SVG wird bevorzugt. PNG-Auswahl erfolgt anhand der INTRINSISCHEN PNG-Pixelgröße (IHDR), nicht anhand q##.")
        sb.AppendLine("''' ImageList_* Ordner erzeugen jeweils eine Funktion, die eine ImageList gefüllt zurückgibt.")
        sb.AppendLine("''' Generiert am " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & ".")
        sb.AppendLine("''' </summary>")
        sb.AppendLine("Public Module MjGfx")
        sb.AppendLine()

        ' Registrierte Pack-Dateien (relativ zu BaseDirectory)
        sb.AppendLine("    ' ── Registrierte GfxPack-Dateien (relativ zum App-Basisverzeichnis) ───────────────")
        sb.AppendLine("    Private ReadOnly s_packFiles As String() = {")
        Dim first As Boolean = True
        For Each f As String In packFiles
            Dim rel As String = MakePathRelativeToBase(f)
            If Not first Then sb.AppendLine(",")
            sb.Append("        """ & rel.Replace("""", """""") & """")
            first = False
        Next
        sb.AppendLine()
        sb.AppendLine("    }")
        sb.AppendLine()

        ' Statische Felder
        sb.AppendLine("    Private s_inited As Boolean = False")
        sb.AppendLine("    Private ReadOnly s_dict As New Dictionary(Of String, List(Of GfxEntry))(StringComparer.OrdinalIgnoreCase)")
        sb.AppendLine("    ' Cache für PNG-Größen, Key = entry.DataGzBase64 (String-Referenz)")
        sb.AppendLine("    Private ReadOnly s_pngSizeCache As New Dictionary(Of String, Size)(StringComparer.Ordinal)")
        sb.AppendLine("    ' Für ImageList-Ordner: FolderTop → (BaseName → Kandidatenliste)")
        sb.AppendLine("    Private ReadOnly s_folderMap As New Dictionary(Of String, Dictionary(Of String, List(Of GfxEntry)))(StringComparer.OrdinalIgnoreCase)")
        sb.AppendLine()

        ' Lazy Load
        sb.AppendLine("    ''' <summary>Einmaliges Laden der registrierten GfxPack-XMLs und Aufbau von s_dict / s_folderMap.</summary>")
        sb.AppendLine("    Private Sub EnsureLoaded()")
        sb.AppendLine("        If s_inited Then Return")
        sb.AppendLine("        SyncLock s_dict")
        sb.AppendLine("            If s_inited Then Return")
        sb.AppendLine("            For Each rel As String In s_packFiles")
        sb.AppendLine("                Dim fullPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,""Eigene Ressourcen"", rel)")
        sb.AppendLine("                If Not File.Exists(fullPath) Then Continue For")
        sb.AppendLine("                Try")
        sb.AppendLine("                    Dim ser As New XmlSerializer(GetType(GfxPack))")
        sb.AppendLine("                    Using fs As FileStream = File.OpenRead(fullPath)")
        sb.AppendLine("                        Dim p As GfxPack = DirectCast(ser.Deserialize(fs), GfxPack)")
        sb.AppendLine("                        If p IsNot Nothing AndAlso p.Entries IsNot Nothing Then")
        sb.AppendLine("                            For Each e As GfxEntry In p.Entries")
        sb.AppendLine("                                If e Is Nothing OrElse String.IsNullOrWhiteSpace(e.Key) Then Continue For")
        sb.AppendLine("                                ' 1) s_dict: Key → Varianten")
        sb.AppendLine("                                Dim lst As List(Of GfxEntry) = Nothing")
        sb.AppendLine("                                If Not s_dict.TryGetValue(e.Key, lst) Then")
        sb.AppendLine("                                    lst = New List(Of GfxEntry)()")
        sb.AppendLine("                                    s_dict(e.Key) = lst")
        sb.AppendLine("                                End If")
        sb.AppendLine("                                lst.Add(e)")
        sb.AppendLine("                                ' 2) s_folderMap: nur Top-Ordner ImageList_* gruppieren")
        sb.AppendLine("                                Dim top As String = """"")
        sb.AppendLine("                                If Not String.IsNullOrEmpty(e.Folder) Then")
        sb.AppendLine("                                    Dim parts() As String = e.Folder.Split(New Char(){Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries)")
        sb.AppendLine("                                    If parts.Length > 0 Then top = parts(0)")
        sb.AppendLine("                                End If")
        sb.AppendLine("                                If Not String.IsNullOrEmpty(top) AndAlso top.StartsWith(""ImageList_"", StringComparison.OrdinalIgnoreCase) Then")
        sb.AppendLine("                                    Dim groups As Dictionary(Of String, List(Of GfxEntry)) = Nothing")
        sb.AppendLine("                                    If Not s_folderMap.TryGetValue(top, groups) Then")
        sb.AppendLine("                                        groups = New Dictionary(Of String, List(Of GfxEntry))(StringComparer.OrdinalIgnoreCase)")
        sb.AppendLine("                                        s_folderMap(top) = groups")
        sb.AppendLine("                                    End If")
        sb.AppendLine("                                    Dim bn As String = If(String.IsNullOrEmpty(e.BaseName), e.Key, e.BaseName)")
        sb.AppendLine("                                    Dim gl As List(Of GfxEntry) = Nothing")
        sb.AppendLine("                                    If Not groups.TryGetValue(bn, gl) Then")
        sb.AppendLine("                                        gl = New List(Of GfxEntry)()")
        sb.AppendLine("                                        groups(bn) = gl")
        sb.AppendLine("                                    End If")
        sb.AppendLine("                                    gl.Add(e)")
        sb.AppendLine("                                End If")
        sb.AppendLine("                            Next")
        sb.AppendLine("                        End If")
        sb.AppendLine("                    End Using")
        sb.AppendLine("                Catch")
        sb.AppendLine("                    ' korruptes Pack still überspringen")
        sb.AppendLine("                End Try")
        sb.AppendLine("            Next")
        sb.AppendLine("            s_inited = True")
        sb.AppendLine("        End SyncLock")
        sb.AppendLine("    End Sub")
        sb.AppendLine()

        ' Public Core
        sb.AppendLine("    ''' <summary>Liefert Bitmap für Key in gewünschter Größe (SVG bevorzugt, sonst beste PNG nach IHDR-Maßen).</summary>")
        sb.AppendLine("    Public Function MjGfx_GfxMain(key As String, width As Integer, Optional height As Integer = 0) As Bitmap")
        sb.AppendLine("        If width <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(width))")
        sb.AppendLine("        If height <= 0 Then height = width")
        sb.AppendLine("        EnsureLoaded()")
        sb.AppendLine("        Dim list As List(Of GfxEntry) = Nothing")
        sb.AppendLine("        If Not s_dict.TryGetValue(key, list) OrElse list Is Nothing OrElse list.Count = 0 Then Return Nothing")
        sb.AppendLine("        Dim svg As GfxEntry = list.FirstOrDefault(Function(e) String.Equals(e.Kind, ""svg"", StringComparison.OrdinalIgnoreCase))")
        sb.AppendLine("        If svg IsNot Nothing Then Return RenderSvgEntry(svg, width, height)")
        sb.AppendLine("        Dim pngBest As GfxEntry = PickBestPngByIntrinsicSize(list, width, height)")
        sb.AppendLine("        If pngBest Is Nothing Then Return Nothing")
        sb.AppendLine("        Return RenderPngEntry(pngBest, width, height)")
        sb.AppendLine("    End Function")
        sb.AppendLine()

        sb.AppendLine("    ''' <summary>Füllt das Label mit der Grafik gleichen Namens in der Größe des Labels</summary>")
        sb.AppendLine("    Public Sub MjGfx_GfxMain(lbl As Label)")
        sb.AppendLine("        lbl.Image = MjGfx_GfxMain(lbl.Name, lbl.Width, lbl.Height) ")
        ' sb.AppendLine("        lbl.Refresh")
        sb.AppendLine("    End Sub")
        sb.AppendLine()

        sb.AppendLine("    ''' <summary>Füllt das Button mit der Grafik gleichen Namens in der Größe 16x16</summary>")
        sb.AppendLine("    Public Sub MjGfx_GfxMain(btn As Button)")
        sb.AppendLine("        btn.TextImageRelation = TextImageRelation.ImageBeforeText")
        sb.AppendLine("        btn.Image = MjGfx_GfxMain(btn.Name, 16)")
        ' sb.AppendLine("        btn.Refresh")
        sb.AppendLine("    End Sub")
        sb.AppendLine()

        ' Auswahl-Helfer
        sb.AppendLine("    Private Function PickBestEntryForSize(Kandidats As IEnumerable(Of GfxEntry), w As Integer, h As Integer) As GfxEntry")
        sb.AppendLine("        Dim svg As GfxEntry = Kandidats.FirstOrDefault(Function(e) String.Equals(e.Kind, ""svg"", StringComparison.OrdinalIgnoreCase))")
        sb.AppendLine("        If svg IsNot Nothing Then Return svg")
        sb.AppendLine("        Return PickBestPngByIntrinsicSize(Kandidats, w, h)")
        sb.AppendLine("    End Function")
        sb.AppendLine()

        ' PNG-Auswahl nach echten Maßen
        sb.AppendLine("    Private Function PickBestPngByIntrinsicSize(Kandidats As IEnumerable(Of GfxEntry), desiredW As Integer, desiredH As Integer) As GfxEntry")
        sb.AppendLine("        Dim best As GfxEntry = Nothing")
        sb.AppendLine("        Dim bestScore As Integer = Integer.MaxValue")
        sb.AppendLine("        Dim bestIsDownscale As Boolean = False")
        sb.AppendLine("        For Each e As GfxEntry In Kandidats")
        sb.AppendLine("            If Not String.Equals(e.Kind, ""png"", StringComparison.OrdinalIgnoreCase) Then Continue For")
        sb.AppendLine("            Dim sz As Size = GetPngSize(e)")
        sb.AppendLine("            If sz.Width <= 0 OrElse sz.Height <= 0 Then Continue For")
        sb.AppendLine("            Dim dw As Integer = Math.Abs(sz.Width - desiredW)")
        sb.AppendLine("            Dim dh As Integer = Math.Abs(sz.Height - desiredH)")
        sb.AppendLine("            Dim score As Integer = Math.Max(dw, dh)")
        sb.AppendLine("            Dim isDownscale As Boolean = (sz.Width >= desiredW AndAlso sz.Height >= desiredH)")
        sb.AppendLine("            If score < bestScore Then")
        sb.AppendLine("                best = e : bestScore = score : bestIsDownscale = isDownscale")
        sb.AppendLine("            ElseIf score = bestScore Then")
        sb.AppendLine("                If isDownscale AndAlso Not bestIsDownscale Then")
        sb.AppendLine("                    best = e : bestIsDownscale = True")
        sb.AppendLine("                ElseIf isDownscale = bestIsDownscale Then")
        sb.AppendLine("                    Dim bsz As Size = If(best Is Nothing, Size.Empty, GetPngSize(best))")
        sb.AppendLine("                    Dim a As Integer = sz.Width * sz.Height")
        sb.AppendLine("                    Dim b As Integer = bsz.Width * bsz.Height")
        sb.AppendLine("                    If a > b Then best = e")
        sb.AppendLine("                End If")
        sb.AppendLine("            End If")
        sb.AppendLine("        Next")
        sb.AppendLine("        If best Is Nothing Then")
        sb.AppendLine("            best = Kandidats.FirstOrDefault(Function(x) String.Equals(x.Kind, ""png"", StringComparison.OrdinalIgnoreCase))")
        sb.AppendLine("        End If")
        sb.AppendLine("        Return best")
        sb.AppendLine("    End Function")
        sb.AppendLine()

        ' PNG-Size aus IHDR (nach GZip-Inflate); Ergebnis wird gecacht
        sb.AppendLine("    Private Function GetPngSize(entry As GfxEntry) As Size")
        sb.AppendLine("        If entry Is Nothing OrElse String.IsNullOrEmpty(entry.DataGzBase64) Then Return Size.Empty")
        sb.AppendLine("        Dim key As String = entry.DataGzBase64")
        sb.AppendLine("        Dim sz As Size = Size.Empty")
        sb.AppendLine("        If s_pngSizeCache.TryGetValue(key, sz) Then Return sz")
        sb.AppendLine("        Dim bytes As Byte() = InflateGZipFromBase64(entry.DataGzBase64)")
        sb.AppendLine("        Dim w As Integer = 0, h As Integer = 0")
        sb.AppendLine("        If TryReadPngIHDR(bytes, w, h) AndAlso w > 0 AndAlso h > 0 Then")
        sb.AppendLine("            sz = New Size(w, h)")
        sb.AppendLine("        Else")
        sb.AppendLine("            Try")
        sb.AppendLine("                Using ms As New MemoryStream(bytes, writable:=False)")
        sb.AppendLine("                    Using img As Image = Image.FromStream(ms, useEmbeddedColorManagement:=True, validateImageData:=True)")
        sb.AppendLine("                        sz = img.Size")
        sb.AppendLine("                    End Using")
        sb.AppendLine("                End Using")
        sb.AppendLine("            Catch")
        sb.AppendLine("                sz = Size.Empty")
        sb.AppendLine("            End Try")
        sb.AppendLine("        End If")
        sb.AppendLine("        If sz <> Size.Empty Then s_pngSizeCache(key) = sz")
        sb.AppendLine("        Return sz")
        sb.AppendLine("    End Function")
        sb.AppendLine()

        ' PNG-IHDR Parser
        sb.AppendLine("    Private Function TryReadPngIHDR(data As Byte(), ByRef width As Integer, ByRef height As Integer) As Boolean")
        sb.AppendLine("        width = 0 : height = 0")
        sb.AppendLine("        If data Is Nothing OrElse data.Length < 33 Then Return False")
        sb.AppendLine("        If Not (data(0) = &H89 AndAlso data(1) = &H50 AndAlso data(2) = &H4E AndAlso data(3) = &H47 AndAlso data(4) = &HD AndAlso data(5) = &HA AndAlso data(6) = &H1A AndAlso data(7) = &HA) Then Return False")
        sb.AppendLine("        Dim t0 As Integer = 8 + 4 ' skip length")
        sb.AppendLine("        If data.Length < t0 + 4 + 8 Then Return False")
        sb.AppendLine("        If Not (data(t0) = &H49 AndAlso data(t0 + 1) = &H48 AndAlso data(t0 + 2) = &H44 AndAlso data(t0 + 3) = &H52) Then Return False")
        sb.AppendLine("        Dim p As Integer = t0 + 4")
        sb.AppendLine("        width = (CInt(data(p)) << 24) Or (CInt(data(p + 1)) << 16) Or (CInt(data(p + 2)) << 8) Or CInt(data(p + 3))")
        sb.AppendLine("        height = (CInt(data(p + 4)) << 24) Or (CInt(data(p + 5)) << 16) Or (CInt(data(p + 6)) << 8) Or CInt(data(p + 7))")
        sb.AppendLine("        Return (width > 0 AndAlso height > 0)")
        sb.AppendLine("    End Function")
        sb.AppendLine()

        ' Render PNG
        sb.AppendLine("    Private Function RenderPngEntry(entry As GfxEntry, w As Integer, h As Integer) As Bitmap")
        sb.AppendLine("        Dim bytes As Byte() = InflateGZipFromBase64(entry.DataGzBase64)")
        sb.AppendLine("        Using ms As New MemoryStream(bytes, writable:=False)")
        sb.AppendLine("            Using portSrcEnum As Image = Image.FromStream(ms, useEmbeddedColorManagement:=True, validateImageData:=True)")
        sb.AppendLine("                Return ResizeImageCrisp(CType(portSrcEnum, Bitmap), w, h)")
        sb.AppendLine("            End Using")
        sb.AppendLine("        End Using")
        sb.AppendLine("    End Function")
        sb.AppendLine()

        ' Render SVG
        sb.AppendLine("    Private Function RenderSvgEntry(entry As GfxEntry, w As Integer, h As Integer) As Bitmap")
        sb.AppendLine("        Dim bytes As Byte() = InflateGZipFromBase64(entry.DataGzBase64)")
        sb.AppendLine("        Using ms As New MemoryStream(bytes, writable:=False)")
        sb.AppendLine("            Dim doc As SvgDocument = SvgDocument.Open(Of SvgDocument)(ms)")
        sb.AppendLine("            ' SVG rendert bereits in Zielgröße – wir schicken es trotzdem durch die zentrale Pipeline,")
        sb.AppendLine("            ' damit Format (32bppArgb) & ggf. Aufhellung im DarkMode einheitlich angewendet werden.")
        sb.AppendLine("            Dim tmp As Bitmap = doc.Draw(w, h)")
        sb.AppendLine("            Try")
        sb.AppendLine("                Return ResizeImageCrisp(tmp, w, h)")
        sb.AppendLine("            Finally")
        sb.AppendLine("                tmp.Dispose()")
        sb.AppendLine("            End Try")
        sb.AppendLine("        End Using")
        sb.AppendLine("    End Function")
        sb.AppendLine()


        ' Resize (hochwertig)
        sb.AppendLine("    Private Function ResizeImageCrisp(portSrcEnum As Bitmap, w As Integer, h As Integer) As Bitmap")
        sb.AppendLine("        If portSrcEnum Is Nothing Then Throw New ArgumentNullException(NameOf(portSrcEnum))")
        sb.AppendLine("        If w <= 0 OrElse h <= 0 Then Throw New ArgumentOutOfRangeException(""w/h"")")
        sb.AppendLine()
        sb.AppendLine("        Dim portDstEnum As New Bitmap(w, h, Drawing.Imaging.PixelFormat.Format32bppArgb)")
        sb.AppendLine("        ' DPI vom Quellbild übernehmen (falls sinnvoll)")
        sb.AppendLine("        Try")
        sb.AppendLine("            portDstEnum.SetResolution(portSrcEnum.HorizontalResolution, portSrcEnum.VerticalResolution)")
        sb.AppendLine("        Catch")
        sb.AppendLine("            ' exotic DPI -> Standard belassen")
        sb.AppendLine("        End Try")
        sb.AppendLine()
        sb.AppendLine("        Using g As Graphics = Graphics.FromImage(portDstEnum)")
        sb.AppendLine("            g.CompositingMode = CompositingMode.SourceOver")
        sb.AppendLine("            g.CompositingQuality = CompositingQuality.HighQuality")
        sb.AppendLine("            g.InterpolationMode = InterpolationMode.HighQualityBicubic")
        sb.AppendLine("            g.PixelOffsetMode = PixelOffsetMode.HighQuality")
        sb.AppendLine("            g.SmoothingMode = SmoothingMode.HighQuality")
        sb.AppendLine()
        sb.AppendLine("            Dim destRect As New Rectangle(0, 0, w, h)")
        sb.AppendLine("            Dim srcRect As New Rectangle(0, 0, portSrcEnum.Width, portSrcEnum.Height)")
        sb.AppendLine()
        sb.AppendLine("            ' Optional: Aufhellung im DarkMode via ColorMatrix (Skalierung der RGB-Kanäle)")
        sb.AppendLine("            Dim useBrighten As Boolean = False")
        sb.AppendLine("            Dim cm As ColorMatrix = Nothing")
        sb.AppendLine("            Dim ia As ImageAttributes = Nothing")
        sb.AppendLine("            Try")
        sb.AppendLine("                If INI.Global_DarkMode Then")
        sb.AppendLine("                    Dim brighten As Single = Math.Max(0.0F, INI.Global_BrightenAmount)")
        sb.AppendLine("                    If brighten > 0.0001F Then")
        sb.AppendLine("                        Dim factor As Single = 1.0F + brighten")
        sb.AppendLine("                        factor = Math.Min(factor, 3.0F) ' harte Kappe gegen Überstrahlung")
        sb.AppendLine("                        cm = New ColorMatrix(New Single()() {")
        sb.AppendLine("                            New Single() {factor, 0.0F,   0.0F,   0.0F, 0.0F},")
        sb.AppendLine("                            New Single() {0.0F,   factor, 0.0F,   0.0F, 0.0F},")
        sb.AppendLine("                            New Single() {0.0F,   0.0F,   factor, 0.0F, 0.0F},")
        sb.AppendLine("                            New Single() {0.0F,   0.0F,   0.0F,   1.0F, 0.0F},")
        sb.AppendLine("                            New Single() {0.0F,   0.0F,   0.0F,   0.0F, 1.0F}")
        sb.AppendLine("                        })")
        sb.AppendLine("                        ia = New ImageAttributes()")
        sb.AppendLine("                        ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)")
        sb.AppendLine("                        useBrighten = True")
        sb.AppendLine("                    End If")
        sb.AppendLine("                End If")
        sb.AppendLine("            Catch")
        sb.AppendLine("                ' Falls INI nicht verfügbar oder andere Fehler → ohne Aufhellung zeichnen")
        sb.AppendLine("                useBrighten = False")
        sb.AppendLine("            End Try")
        sb.AppendLine()
        sb.AppendLine("            If useBrighten AndAlso ia IsNot Nothing Then")
        sb.AppendLine("                g.DrawImage(portSrcEnum, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, ia)")
        sb.AppendLine("            Else")
        sb.AppendLine("                g.DrawImage(portSrcEnum, destRect, srcRect, GraphicsUnit.Pixel)")
        sb.AppendLine("            End If")
        sb.AppendLine("        End Using")
        sb.AppendLine()
        sb.AppendLine("        Return portDstEnum")
        sb.AppendLine("    End Function")
        sb.AppendLine()


        ' Inflate GZip (Base64)
        sb.AppendLine("    Private Function InflateGZipFromBase64(b64 As String) As Byte()")
        sb.AppendLine("        Dim gz As Byte() = Convert.FromBase64String(b64)")
        sb.AppendLine("        Using cms As New MemoryStream(gz, writable:=False)")
        sb.AppendLine("            Using gs As New GZipStream(cms, CompressionMode.Decompress, leaveOpen:=False)")
        sb.AppendLine("                Using outMs As New MemoryStream()")
        sb.AppendLine("                    gs.CopyTo(outMs)")
        sb.AppendLine("                    Return outMs.ToArray()")
        sb.AppendLine("                End Using")
        sb.AppendLine("            End Using")
        sb.AppendLine("        End Using")
        sb.AppendLine("    End Function")
        sb.AppendLine()

        ' ImageList bauen (für Top-Ordner ImageList_*)
        sb.AppendLine("    ''' <summary>Erzeugt eine ImageList aus dem angegebenen ImageList_-Top-Ordner in gewünschter Größe.</summary>")
        sb.AppendLine("    Private Function BuildImageListForFolder(folderTop As String, width As Integer, height As Integer) As ImageList")
        sb.AppendLine("        EnsureLoaded()")
        sb.AppendLine("        If width <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(width))")
        sb.AppendLine("        If height <= 0 Then height = width")
        sb.AppendLine("        Dim il As New ImageList()")
        sb.AppendLine("        il.ColorDepth = ColorDepth.Depth32Bit")
        sb.AppendLine("        il.ImageSize = New Size(width, height)")
        sb.AppendLine("        Dim groups As Dictionary(Of String, List(Of GfxEntry)) = Nothing")
        sb.AppendLine("        If Not s_folderMap.TryGetValue(folderTop, groups) OrElse groups Is Nothing OrElse groups.Count = 0 Then")
        sb.AppendLine("            Return il")
        sb.AppendLine("        End If")
        sb.AppendLine("        For Each bn As String In groups.Keys.OrderBy(Function(x) x, StringComparer.OrdinalIgnoreCase)")
        sb.AppendLine("            Dim best As GfxEntry = PickBestEntryForSize(groups(bn), width, height)")
        sb.AppendLine("            If best Is Nothing Then Continue For")
        sb.AppendLine("            Dim bmp As Bitmap = Nothing")
        sb.AppendLine("            If String.Equals(best.Kind, ""svg"", StringComparison.OrdinalIgnoreCase) Then")
        sb.AppendLine("                bmp = RenderSvgEntry(best, width, height)")
        sb.AppendLine("            Else")
        sb.AppendLine("                bmp = RenderPngEntry(best, width, height)")
        sb.AppendLine("            End If")
        sb.AppendLine("            If bmp IsNot Nothing Then")
        sb.AppendLine("                il.Images.Add(bn, bmp)")
        sb.AppendLine("            End If")
        sb.AppendLine("        Next")
        sb.AppendLine("        Return il")
        sb.AppendLine("    End Function")
        sb.AppendLine()

        ' ── Öffentliche Wrapper: normale Keys ────────────────────────────────────────
        For Each key As String In allKeys

            'btn und lbl werden über MjGfx_GfxMain abgerufen.
            'Die Grafik muss dazu den gleichen Namen haben wie das Label und der Button.
            If key.StartsWith("lbl", StringComparison.Ordinal) Then
                Continue For
            End If
            If key.StartsWith("btn", StringComparison.Ordinal) Then
                Continue For
            End If

            Dim fnName As String = "MjGfx_" & SanitizeForCodeIdentifier(key)
            sb.AppendLine("    ''' <summary>Gibt """ & key & """ als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>")
            sb.AppendLine("    Public Function " & fnName & "(width As Integer, Optional height As Integer = 0) As Bitmap")
            sb.AppendLine("        Return MjGfx_GfxMain(""" & key.Replace("""", """""") & """, width, height)")
            sb.AppendLine("    End Function")
            sb.AppendLine()
        Next

        ' ── Öffentliche Wrapper: ImageList-Funktionen (eine pro Top-Ordner) ─────────
        For Each folderTop As String In folderMap.Keys.OrderBy(Function(x) x, StringComparer.OrdinalIgnoreCase)
            Dim fnIl As String = SanitizeForCodeIdentifier(folderTop) ' KEIN Präfix
            sb.AppendLine("    ''' <summary>Erzeugt eine ImageList aus """ & folderTop & """ (Items alphabetisch nach BaseName) in gewünschter Größe.</summary>")
            sb.AppendLine("    Public Function MjGfx_" & fnIl & "(width As Integer, Optional height As Integer = 0) As ImageList")
            sb.AppendLine("        Return BuildImageListForFolder(""" & folderTop.Replace("""", """""") & """, width, height)")
            sb.AppendLine("    End Function")
            sb.AppendLine()
        Next

        sb.AppendLine("End Module")

        ' Datei schreiben (UTF-8 ohne BOM)
        File.WriteAllText(targetPath, sb.ToString(), New UTF8Encoding(False))
    End Sub

    ' Hilfsroutine: stabiler VB-Bezeichnerteil
    Private Function SanitizeForCodeIdentifier(src As String) As String
        If String.IsNullOrEmpty(src) Then Return "_"
        Dim sb As New StringBuilder(src.Length)
        Dim i As Integer
        For i = 0 To src.Length - 1
            Dim ch As Char = src(i)
            If Char.IsLetterOrDigit(ch) OrElse ch = "_"c Then
                sb.Append(ch)
            Else
                sb.Append("_"c)
            End If
        Next
        Return sb.ToString()
    End Function

    ' Hilfsroutine: macht Pfad relativ zu BaseDirectory (zur Laufzeit dort gesucht)
    Private Function MakePathRelativeToBase(full As String) As String
        If String.IsNullOrEmpty(full) Then Return ""
        Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
        Dim p As String = Path.GetFullPath(full)
        If p.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase) Then
            Dim rel As String = p.Substring(baseDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            Return rel.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
        End If
        ' Wenn nicht unterhalb, nur Dateiname (robust)
        Return Path.GetFileName(p)
    End Function



End Class
