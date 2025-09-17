Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Namespace MahjongGKSymbolFactory

    '
    ''' <summary>
    ''' Mahjong-Serien für Face-Tinting.
    ''' </summary>
    Public Enum TileSeries
        Dots            ' Kreise
        Bamboo          ' Bambus
        Characters      ' Zeichen
        Honors          ' Winde/Drache
        Flowers         ' Blumen
        Seasons         ' Jahreszeiten
    End Enum

    '
    ''' <summary>
    ''' Farbpaletten für die Serien (pro Preset leicht anders nuanciert).
    ''' </summary>
    Public NotInheritable Class SeriesPalettes
        Private Sub New()
        End Sub

        Public Shared Function Classic(series As TileSeries, Optional alpha As Integer = 24) As Color
            Select Case series
                Case TileSeries.Dots : Return Color.FromArgb(alpha, 40, 90, 200) ' leichtes Royal-Blau
                Case TileSeries.Bamboo : Return Color.FromArgb(alpha, 10, 140, 60) ' sattes Grün
                Case TileSeries.Characters : Return Color.FromArgb(alpha, 200, 40, 40) ' zurückhaltendes Rot
                Case TileSeries.Honors : Return Color.FromArgb(alpha, 120, 60, 10) ' warmes Braun
                Case TileSeries.Flowers : Return Color.FromArgb(alpha, 160, 60, 140) ' Magenta
                Case TileSeries.Seasons : Return Color.FromArgb(alpha, 40, 120, 120) ' Petrol
                Case Else : Return Color.FromArgb(0, 0, 0, 0)
            End Select
        End Function

        Public Shared Function Modern(series As TileSeries, Optional alpha As Integer = 18) As Color
            Select Case series
                Case TileSeries.Dots : Return Color.FromArgb(alpha, 50, 110, 255) ' kühles Blau
                Case TileSeries.Bamboo : Return Color.FromArgb(alpha, 30, 170, 100) ' frisches Grün
                Case TileSeries.Characters : Return Color.FromArgb(alpha, 235, 70, 70) ' klares Rot
                Case TileSeries.Honors : Return Color.FromArgb(alpha, 90, 90, 90) ' neutral-grau
                Case TileSeries.Flowers : Return Color.FromArgb(alpha, 180, 80, 180) ' modern Magenta
                Case TileSeries.Seasons : Return Color.FromArgb(alpha, 60, 160, 180) ' kühles Türkis
                Case Else : Return Color.FromArgb(0, 0, 0, 0)
            End Select
        End Function

        Public Shared Function Textured(series As TileSeries, Optional alpha As Integer = 28) As Color
            Select Case series
                Case TileSeries.Dots : Return Color.FromArgb(alpha, 30, 80, 180) ' gedecktes Blau
                Case TileSeries.Bamboo : Return Color.FromArgb(alpha, 20, 120, 70) ' gedecktes Grün
                Case TileSeries.Characters : Return Color.FromArgb(alpha, 170, 50, 40) ' warmes Rot
                Case TileSeries.Honors : Return Color.FromArgb(alpha, 120, 70, 30) ' erdig
                Case TileSeries.Flowers : Return Color.FromArgb(alpha, 150, 70, 130) ' warm Magenta
                Case TileSeries.Seasons : Return Color.FromArgb(alpha, 50, 120, 120) ' gedecktes Petrol
                Case Else : Return Color.FromArgb(0, 0, 0, 0)
            End Select
        End Function
    End Class

    '
    ''' <summary>
    ''' Kombiniert Preset + Serienfarbe zu einem vollständigen TileStyle.
    ''' </summary>
    Public NotInheritable Class TileStyleFactory
        Private Sub New()
        End Sub

        Public Shared Function CreatePresetForSeries(preset As TilePreset,
                                                     surfaceSize As Size,
                                                     series As TileSeries,
                                                     Optional faceTintAlphaOverride As Integer? = Nothing) As TileStyle
            Dim st As TileStyle = TileStyle.CreatePreset(preset, surfaceSize)

            Dim tint As Color
            Select Case preset
                Case TilePreset.Klassisch
                    tint = SeriesPalettes.Classic(series, If(faceTintAlphaOverride, 24))
                Case TilePreset.ModernGlatt
                    tint = SeriesPalettes.Modern(series, If(faceTintAlphaOverride, 18))
                Case TilePreset.StarkTexturiert
                    tint = SeriesPalettes.Textured(series, If(faceTintAlphaOverride, 28))
                Case Else
                    tint = Color.FromArgb(0, 0, 0, 0)
            End Select

            st.FaceTint = tint
            Return st
        End Function

    End Class

End Namespace

