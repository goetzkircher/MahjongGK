Imports System.IO

Namespace MahjongGKSymbolFactory
    ''' <summary>
    ''' Erzeugt, sofern nicht vorhanden, die TileStylePresets.pre 
    ''' </summary>
    Public Class CreateIniFileTileStylePresetsDefault

        Sub New()
            LoadFileContent()
        End Sub
        '
        ''' <summary>
        ''' Der Dateiname. Ist er ohne die Endung ".pre" wird diese angehängt.
        ''' </summary>
        ''' <param name="name"></param>
        Sub New(name As String)
            If Not String.IsNullOrEmpty(name) Then
                If Not name.EndsWith(".pre") Then
                    name &= ".pre"
                End If
            End If
            _Name_ext = name
            LoadFileContent()
        End Sub

        Private _Content As String = Nothing
        Private _Name_ext As String = Nothing

        Public ReadOnly Property Content As String
            Get
                Return _Content
            End Get
        End Property
        Public ReadOnly Property FileName_ext As String
            Get
                Return _Name_ext
            End Get
        End Property

        Public ReadOnly Property FullPath As String
            Get
                Return GetFullPath()
            End Get
        End Property
        Public ReadOnly Property ContentDefault As String
            Get
                Dim sb As New System.Text.StringBuilder
                '
                sb.AppendLine("[Preset.Classic]")
                sb.AppendLine("CornerRadius = 14")
                sb.AppendLine("EdgeWidth = 10")
                sb.AppendLine("ShadowSize = 24")
                sb.AppendLine("FaceInset = 8")
                sb.AppendLine("FaceTint = 140060A0")       ' ARGB hex (AARRGGBB) – hier Alpha=0x14 (=20)
                sb.AppendLine("BodyTop = FFF9F9F9")
                sb.AppendLine("BodyBottom = FFDCDCDC")
                sb.AppendLine("EdgeLight = 6EFFffff")
                sb.AppendLine("EdgeDark = 6.0")
                sb.AppendLine("FaceIvoryLight = FFFFFCF6")
                sb.AppendLine("FaceIvoryDark = FFF1EBE1")
                sb.AppendLine("GlossOpacity = 70")
                sb.AppendLine("NoiseOpacity = 12")
                sb.AppendLine("SymbolShadowOpacity = 120")
                sb.AppendLine("SymbolScale = 0.86")
                '
                sb.AppendLine("[Preset.Modern]")
                sb.AppendLine("CornerRadius = 8")
                sb.AppendLine("EdgeWidth = 6")
                sb.AppendLine("ShadowSize = 20")
                sb.AppendLine("FaceInset = 7")
                sb.AppendLine("FaceTint = 0A282878")
                sb.AppendLine("BodyTop = FFFAFAFA")
                sb.AppendLine("BodyBottom = FFEDEDED")
                sb.AppendLine("EdgeLight = 70.0FFFFFF")
                sb.AppendLine("EdgeDark = 55000000")
                sb.AppendLine("FaceIvoryLight = FFFAFAFA")
                sb.AppendLine("FaceIvoryDark = FFEDEDED")
                sb.AppendLine("GlossOpacity = 110")
                sb.AppendLine("NoiseOpacity = 0")
                sb.AppendLine("SymbolShadowOpacity = 80")
                sb.AppendLine("SymbolScale = 0.88")
                '
                sb.AppendLine("[Preset.Textured]")
                sb.AppendLine("CornerRadius = 16")
                sb.AppendLine("EdgeWidth = 12")
                sb.AppendLine("ShadowSize = 28")
                sb.AppendLine("FaceInset = 9")
                sb.AppendLine("FaceTint = 1CA0783C")
                sb.AppendLine("BodyTop = FFFAF8F0")
                sb.AppendLine("BodyBottom = FFE6DAC8")
                sb.AppendLine("EdgeLight = 70.0FFFFFF")
                sb.AppendLine("EdgeDark = 70000000")
                sb.AppendLine("FaceIvoryLight = FFFAF8F0")
                sb.AppendLine("FaceIvoryDark = FFE4DCCD")
                sb.AppendLine("GlossOpacity = 50")
                sb.AppendLine("NoiseOpacity = 35")
                sb.AppendLine("SymbolShadowOpacity = 140")
                sb.AppendLine("SymbolScale = 0.85")
                '

                Return sb.ToString

            End Get
        End Property

        Public Sub SaveIniFileContent()
            Try
                If String.IsNullOrWhiteSpace(_Content) Then
                    _Content = ContentDefault
                End If
                File.WriteAllText(GetFullPath, _Content)
            Catch ex As Exception

            End Try
        End Sub

        Public Sub LoadFileContent()
            Dim fullPath As String = GetFullPath()
            If Not File.Exists(fullPath) Then
                SaveIniFileContent()
            End If
            Try
                _Content = File.ReadAllText(fullPath)
            Catch ex As Exception

            End Try
        End Sub

        Private Function GetFullPath() As String

            If String.IsNullOrEmpty(_Name_ext) Then
                _Name_ext = "TileStylePresets.set"
            End If

            Return IO.Path.Combine(AppDataDirectory(AppDataSubDir.Steine198x252Layout), _Name_ext)


        End Function

    End Class

End Namespace