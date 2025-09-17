Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.IO.Compression

''' <summary>
''' Kapselt die Initiallogik für Hintergrundgrafiken:
''' - Seed aus ZIP (Werksbestand) nach AppData\Hintergrundgrafiken (nur fehlende Dateien)
''' - Prüfung und Aufräumen Benutzerverzeichnis (Eigene_Hintergrundgrafiken)
''' - Thumbnail-Erzeugung im selben Ordner: "Bildname.thb.png"
''' - Entfernen überzähliger Thumbs
''' </summary>
Public Class BackgroundZipManager

    ' -------------------------
    ' Konfiguration
    ' -------------------------
    Private Const THUMB_MAX_EDGE As Integer = 500       ' maximale Kantenlänge Thumbnail
    Private Const DIAG_MIN_PX As Integer = 600          ' Mindestdiagonale für Benutzerbilder

    Private Const FILESIZE_MAX_MBYTES As Long = 6L
    Private Const FILESIZE_MAX_BYTES As Long = FILESIZE_MAX_MBYTES * 1024L * 1024L ' 6 MB
    Private Const BAD_SUBDIR As String = "Unbrauchbare Bilder"

    ' Erlaubte Bildextensions (Lower-Invariant, ohne Punkt)
    Private Shared ReadOnly AllowedExt As String() =
        New String() {"jpg", "jpeg", "png", "bmp", "gif", "webp"}

    ' -------------------------
    ' Öffentlicher Einstieg
    ' -------------------------
    Public Sub InitializeOnStartup()
        Dim seedZipPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                 "Images", "Photography", "Hintergrundgrafiken.zip")

        Dim dirFactory As String = INI.AppDataDirectory(AppDataSubDir.Hintergrundgrafiken)
        Dim dirUser As String = INI.AppDataDirectory(AppDataSubDir.Eigene_Hintergrundgrafiken)

        Directory.CreateDirectory(dirFactory)
        Directory.CreateDirectory(dirUser)

        ' 1) Werksbestand aus ZIP: nur fehlende Dateien kopieren
        If File.Exists(seedZipPath) Then
            SeedZipToDirectory(seedZipPath, dirFactory)
        End If

        ' 2) Benutzerverzeichnis prüfen & unbrauchbare verschieben
        CleanupUserDirectory(dirUser)

        ' 3) Thumbnails erzeugen/ergänzen (Factory & User)
        EnsureThumbnails(dirFactory)
        EnsureThumbnails(dirUser)

        ' 4) Überzählige Thumbs entfernen (Factory & User)
        RemoveDanglingThumbs(dirFactory)
        RemoveDanglingThumbs(dirUser)
    End Sub


    ' 2) NEU HINZUFÜGEN (damit das Formular den Wert kennt, ohne Shared zu verwenden):
    Public Shared ReadOnly Property ThumbMaxEdge As Integer
        Get
            Return THUMB_MAX_EDGE
        End Get
    End Property

    Public Shared ReadOnly Property InfoText As String
        Get
            Dim sb As New System.Text.StringBuilder
            sb.Append("Du kannst eigene Hintergrundgrafiken benutzen. ")
            sb.Append("Es dürfen Png, Jpg, Gif und Webp Dateien sein. ")
            sb.Append("Sie sollten querformatig sein und in der Diagonalen ")
            sb.Append($"mindestens {DIAG_MIN_PX} Pixel lang sein. ")
            sb.Append($"Die Datei darf nicht größer als {FILESIZE_MAX_MBYTES} MB sein. ")
            sb.Append("Verwende als Dateinamen nicht nur Zahlen, sondern auch Buchstaben. ")
            sb.AppendLine("Speichere sie in:")
            sb.AppendLine(INI.AppDataDirectory(AppDataSubDir.Eigene_Hintergrundgrafiken))
            sb.AppendFormat("und starte das Programm neu.")
            Return sb.ToString
        End Get
    End Property

    ' -------------------------
    ' Seed ZIP → Zielverzeichnis
    ' -------------------------
    Private Sub SeedZipToDirectory(zipPath As String, targetDir As String)
        Using za As ZipArchive = ZipFile.OpenRead(zipPath)
            For Each entry As ZipArchiveEntry In za.Entries
                If String.IsNullOrEmpty(entry.Name) Then Continue For ' Ordner
                Dim ext As String = GetExtLower(entry.Name)
                If Not AllowedExt.Contains(ext) Then Continue For

                Dim outPath As String = Path.Combine(targetDir, entry.Name)
                ' Ordnerstruktur im ZIP beibehalten/erzeugen
                Dim outDir As String = Path.GetDirectoryName(outPath)
                If Not String.IsNullOrEmpty(outDir) Then
                    Directory.CreateDirectory(outDir)
                End If

                If Not File.Exists(outPath) Then
                    ' Nur fehlende Dateien extrahieren
                    ExtractEntry(entry, outPath)
                End If
            Next
        End Using
    End Sub

    Private Sub ExtractEntry(entry As ZipArchiveEntry, outPath As String)
        ' robustes Extrahieren (überschreibt NICHT)
        Using src As Stream = entry.Open()
            Using dst As New FileStream(outPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                src.CopyTo(dst)
            End Using
        End Using
    End Sub

    ' -------------------------
    ' Benutzerverzeichnis prüfen/aufräumen
    ' -------------------------
    Private Sub CleanupUserDirectory(userDir As String)
        Dim badDir As String = Path.Combine(userDir, BAD_SUBDIR)
        Directory.CreateDirectory(badDir)

        For Each file As String In EnumerateImages(userDir)
            ' --- NEU: Ordner "Unbrauchbare Bilder" komplett überspringen ---
            Dim p As String = Path.GetDirectoryName(file)
            If Not String.IsNullOrEmpty(p) AndAlso
           String.Equals(Path.GetFullPath(p), Path.GetFullPath(badDir), StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If
            ' ---------------------------------------------------------------

            ' Thumbs auslassen
            If IsThumbFile(file) Then Continue For

            Try
                Dim fi As New FileInfo(file)
                If fi.Length > FILESIZE_MAX_BYTES Then
                    MoveOverwrite(file, Path.Combine(badDir, Path.GetFileName(file)))
                    Continue For
                End If

                Dim w As Integer
                Dim h As Integer
                If Not TryProbeImageSize(file, w, h) Then
                    MoveOverwrite(file, Path.Combine(badDir, Path.GetFileName(file)))
                    Continue For
                End If

                Dim heightTooTall As Boolean = (CDbl(h) > 2.0 * CDbl(w))
                Dim widthTooWide As Boolean = (CDbl(w) > 3.0 * CDbl(h))
                Dim diag As Double = Math.Sqrt(CDbl(w) * CDbl(w) + CDbl(h) * CDbl(h))
                Dim diagTooSmall As Boolean = (diag < DIAG_MIN_PX)

                If heightTooTall OrElse widthTooWide OrElse diagTooSmall Then
                    MoveOverwrite(file, Path.Combine(badDir, Path.GetFileName(file)))
                End If

            Catch
                MoveOverwrite(file, Path.Combine(badDir, Path.GetFileName(file)))
            End Try
        Next
    End Sub


    ' -------------------------
    ' Thumbs erzeugen/ergänzen
    ' -------------------------
    Private Sub EnsureThumbnails(rootDir As String)
        For Each imgPath As String In EnumerateImages(rootDir)
            If IsThumbFile(imgPath) Then Continue For
            Dim thumbPath As String = imgPath & ".thb.png"
            If File.Exists(thumbPath) Then Continue For

            Try
                Using bmp As Bitmap = LoadBitmapUnlocked(imgPath)
                    Using thumb As Bitmap = MakeThumbnail(bmp, THUMB_MAX_EDGE)
                        SavePng(thumbPath, thumb)
                    End Using
                End Using
            Catch
                ' Thumbnailerzeugung fehlschlägt? → überspringen
            End Try
        Next
    End Sub

    Private Sub RemoveDanglingThumbs(rootDir As String)
        For Each thb As String In Directory.EnumerateFiles(rootDir, "*.thb.png", SearchOption.AllDirectories)
            Dim baseImg As String = thb.Substring(0, thb.Length - ".thb.png".Length)
            ' Nur als Thumb zählen, wenn das Basisbild eine echte Bilddatei ist
            If Not File.Exists(baseImg) OrElse Not AllowedExt.Contains(GetExtLower(baseImg)) Then
                Try
                    File.Delete(thb)
                Catch
                    ' Ignorieren (z. B. fehlende Rechte)
                End Try
            End If
        Next
    End Sub

    ' -------------------------
    ' Utilities
    ' -------------------------
    Private Shared Function EnumerateImages(rootDir As String) As IEnumerable(Of String)
        ' Alle Dateien mit erlaubten Extensions, rekursiv
        Return Directory.EnumerateFiles(rootDir, "*.*", SearchOption.AllDirectories) _
                       .Where(Function(f As String)
                                  Dim ext As String = GetExtLower(f)
                                  Return AllowedExt.Contains(ext) OrElse f.EndsWith(".thb.png", StringComparison.OrdinalIgnoreCase)
                              End Function)
    End Function

    Private Shared Function GetExtLower(pathname As String) As String
        Dim ext As String = Path.GetExtension(pathname)
        If String.IsNullOrEmpty(ext) Then Return ""
        If ext.StartsWith(".", StringComparison.Ordinal) Then ext = ext.Substring(1)
        Return ext.ToLowerInvariant()
    End Function

    Private Shared Function IsThumbFile(path As String) As Boolean
        Return path.EndsWith(".thb.png", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Sub MoveOverwrite(src As String, dest As String)
        Dim srcFull As String = Path.GetFullPath(src)
        Dim destFull As String = Path.GetFullPath(dest)

        ' --- NEU: keine Aktion, wenn Quelle und Ziel identisch sind ---
        If String.Equals(srcFull, destFull, StringComparison.OrdinalIgnoreCase) Then
            Return
        End If
        ' --------------------------------------------------------------

        Try
            Dim dir As String = Path.GetDirectoryName(destFull)
            If Not String.IsNullOrEmpty(dir) Then
                Directory.CreateDirectory(dir)
            End If
            If File.Exists(destFull) Then
                File.Delete(destFull)
            End If
            File.Move(srcFull, destFull)
        Catch
            ' Fallback Copy+Delete (andere Partition etc.)
            Try
                File.Copy(srcFull, destFull, overwrite:=True)
                File.Delete(srcFull)
            Catch
                ' Ignorieren
            End Try
        End Try
    End Sub


    Private Shared Function TryProbeImageSize(path As String, ByRef width As Integer, ByRef height As Integer) As Boolean
        width = 0 : height = 0
        Try
            Using bmp As Bitmap = LoadBitmapUnlocked(path)
                width = bmp.Width
                height = bmp.Height
                Return True
            End Using
        Catch
            ' Hinweis: Falls WebP o. a. Formate ohne Magick.NET nicht lesbar sind → False
            Return False
        End Try
    End Function

    Private Shared Function LoadBitmapUnlocked(filePath As String) As Bitmap
        ' Ohne File-Lock laden (kopiert das Bild in einen neuen Bitmap-Puffer)
        Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            Using img As Image = Image.FromStream(fs, useEmbeddedColorManagement:=True, validateImageData:=True)
                Return New Bitmap(img)
            End Using
        End Using
    End Function

    Private Shared Function MakeThumbnail(src As Image, maxEdge As Integer) As Bitmap
        Dim w As Integer = src.Width
        Dim h As Integer = src.Height
        Dim scale As Single = CSng(maxEdge) / CSng(Math.Max(w, h))
        If scale > 1.0F Then scale = 1.0F ' nicht hochskalieren
        Dim tw As Integer = Math.Max(1, CInt(Math.Round(w * scale)))
        Dim th As Integer = Math.Max(1, CInt(Math.Round(h * scale)))

        Dim bmp As New Bitmap(tw, th, PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.HighQuality
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.DrawImage(src, New Rectangle(0, 0, tw, th))
        End Using
        Return bmp
    End Function

    Private Shared Sub SavePng(pathname As String, bmp As Bitmap)
        Dim dir As String = Path.GetDirectoryName(pathname)
        If Not String.IsNullOrEmpty(dir) Then
            Directory.CreateDirectory(dir)
        End If
        bmp.Save(pathname, ImageFormat.Png)
    End Sub

End Class

