Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

'Namespace Theme
Public Module ThemeResourcesRaw_zlv ' zlv = zum löschen vorgesehen

    '        ' Cache teilt unveränderte, unverfälschte Instanzen
    '        ' (ThemeManager klont bei Bedarf vor dem Aufhellen selbst!)
    '        Private ReadOnly _cache As New ConcurrentDictionary(Of String, Bitmap)(StringComparer.OrdinalIgnoreCase)

    '        'Mit Lamdafunktion:
    '        ''''' <summary>
    '        ''''' Liefert eine unveränderte 32bppARGB-Bitmap aus den Ressourcen (roh, nicht aufgehellt).
    '        ''''' Rückgabe ist CACHED und darf NICHT verändert oder disposed werden!
    '        ''''' </summary>
    '        ''Public Function GetResBmpRaw(name As String) As Bitmap
    '        ''    Return _cache.GetOrAdd(name,
    '        ''    Function(n As String) As Bitmap
    '        ''        Dim obj As Object = My.Resources.ResourceManager.GetObject(n)
    '        ''        If obj Is Nothing Then Throw New ArgumentException($"Resource '{n}' nicht gefunden.")
    '        ''        Dim img As Image = TryCast(obj, Image)
    '        ''        If img Is Nothing Then Throw New InvalidCastException($"Resource '{n}' ist kein Image.")

    '        ''        Dim bmp As New Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb)
    '        ''        bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution)
    '        ''        Using g As Graphics = Graphics.FromImage(bmp)
    '        ''            g.DrawImage(img, 0, 0, img.Width, img.Height)
    '        ''        End Using
    '        ''        Return bmp
    '        ''    End Function)
    '        ''End Function

    '        'ohne Lamda-Funktioneen
    '        Private Function LoadResBmp(n As String) As Bitmap
    '            Dim obj As Object = My.Resources.ResourceManager.GetObject(n)
    '            If obj Is Nothing Then Throw New ArgumentException($"Resource '{n}' nicht gefunden.")
    '            Dim img As Image = TryCast(obj, Image)
    '            If img Is Nothing Then Throw New InvalidCastException($"Resource '{n}' ist kein Image.")

    '            Dim bmp As New Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb)
    '            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution)
    '            Using g As Graphics = Graphics.FromImage(bmp)
    '                g.DrawImage(img, 0, 0, img.Width, img.Height)
    '            End Using
    '            Return bmp
    '        End Function

    '        Public Function GetResBmpRaw(name As String) As Bitmap
    '            Return _cache.GetOrAdd(name, AddressOf LoadResBmp)
    '        End Function


End Module
'End Namespace

