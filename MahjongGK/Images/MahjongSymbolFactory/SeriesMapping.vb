Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Namespace MahjongGKSymbolFactory

    '
    ''' <summary>
    ''' Serienzuordnung für deinen exakten Enum-Satz.
    ''' </summary>
    Public NotInheritable Class SeriesMapping
        Private Sub New()
        End Sub

        '
        ''' <summary>
        ''' Liefert die Serie für einen Steinindex.
        ''' </summary>
        Public Shared Function MapEnumToSeries(si As SteinIndexEnum) As TileSeries
            Select Case si
                Case SteinIndexEnum.Punkt01, SteinIndexEnum.Punkt02, SteinIndexEnum.Punkt03,
                     SteinIndexEnum.Punkt04, SteinIndexEnum.Punkt05, SteinIndexEnum.Punkt06,
                     SteinIndexEnum.Punkt07, SteinIndexEnum.Punkt08, SteinIndexEnum.Punkt09
                    Return TileSeries.Dots

                Case SteinIndexEnum.Bambus1, SteinIndexEnum.Bambus2, SteinIndexEnum.Bambus3,
                     SteinIndexEnum.Bambus4, SteinIndexEnum.Bambus5, SteinIndexEnum.Bambus6,
                     SteinIndexEnum.Bambus7, SteinIndexEnum.Bambus8, SteinIndexEnum.Bambus9
                    Return TileSeries.Bamboo

                Case SteinIndexEnum.Symbol1, SteinIndexEnum.Symbol2, SteinIndexEnum.Symbol3,
                     SteinIndexEnum.Symbol4, SteinIndexEnum.Symbol5, SteinIndexEnum.Symbol6,
                     SteinIndexEnum.Symbol7, SteinIndexEnum.Symbol8, SteinIndexEnum.Symbol9
                    Return TileSeries.Characters

                Case SteinIndexEnum.DracheR, SteinIndexEnum.DracheG, SteinIndexEnum.DracheW,
                     SteinIndexEnum.WindOst, SteinIndexEnum.WindSüd, SteinIndexEnum.WindWst, SteinIndexEnum.WindNrd
                    Return TileSeries.Honors

                Case SteinIndexEnum.BlütePf, SteinIndexEnum.BlüteOr, SteinIndexEnum.BlüteCt, SteinIndexEnum.BlüteBa
                    Return TileSeries.Flowers

                Case SteinIndexEnum.JahrFrl, SteinIndexEnum.JahrSom, SteinIndexEnum.JahrHer, SteinIndexEnum.JahrWin
                    Return TileSeries.Seasons

                Case SteinIndexEnum.ErrorSy
                    Return TileSeries.Honors ' neutral behandeln – oder separat einfärben

                Case Else
                    Return TileSeries.Characters ' Fallback
            End Select
        End Function

        '
        ''' <summary>
        ''' Convenience: Map über den numerischen Index 0..41 (deiner Enum-Reihenfolge).
        ''' </summary>
        Public Shared Function MapIndexToSeries(idx As Integer) As TileSeries
            Dim si As SteinIndexEnum = CType(idx, SteinIndexEnum)
            Return MapEnumToSeries(si)
        End Function

        '
        ''' <summary>
        ''' Optional: Farb-Feintuning für Honors (Drachen/Winde) statt nur Serientint.
        ''' Gibt Nothing zurück, wenn der Serien-Tint verwendet werden soll.
        ''' </summary>
        Public Shared Function TryGetHonorFaceTint(si As SteinIndexEnum, preset As TilePreset) As Color?
            Select Case si
                Case SteinIndexEnum.DracheR
                    Return If(preset = TilePreset.ModernGlatt,
                              Color.FromArgb(22, 230, 60, 60),
                              Color.FromArgb(26, 200, 50, 50))
                Case SteinIndexEnum.DracheG
                    Return If(preset = TilePreset.ModernGlatt,
                              Color.FromArgb(20, 40, 180, 80),
                              Color.FromArgb(24, 30, 140, 70))
                Case SteinIndexEnum.DracheW
                    Return If(preset = TilePreset.ModernGlatt,
                              Color.FromArgb(14, 120, 120, 120),
                              Color.FromArgb(18, 100, 100, 100))
                Case SteinIndexEnum.WindOst, SteinIndexEnum.WindSüd, SteinIndexEnum.WindWst, SteinIndexEnum.WindNrd
                    Return If(preset = TilePreset.ModernGlatt,
                              Color.FromArgb(14, 90, 90, 90),
                              Color.FromArgb(18, 80, 80, 80))
                Case Else
                    Return Nothing
            End Select
        End Function

    End Class

    '
    ''' <summary>
    ''' Rendert einen kompletten Satz anhand deiner Enum-Reihenfolge – mit Preset + Serienfarbe.
    ''' </summary>
    Public NotInheritable Class SetRenderer
        Private Sub New()
        End Sub

        ''' 
        ''' <summary>
        ''' symbols muss in der Reihenfolge deiner Enum-Werte (0..41) stehen.
        ''' </summary>
        Public Shared Function RenderSetWithSeries(symbols As IList(Of Bitmap),
                                                   size As Size,
                                                   preset As TilePreset,
                                                   Optional overrideAlpha As Integer? = Nothing) As List(Of Bitmap)
            Dim result As New List(Of Bitmap)(symbols.Count)
            For i As Integer = 0 To symbols.Count - 1
                Dim si As SteinIndexEnum = CType(i, SteinIndexEnum)
                Dim series As TileSeries = SeriesMapping.MapEnumToSeries(si)

                Dim st As TileStyle = TileStyleFactory.CreatePresetForSeries(preset, size, series,
                                                                             faceTintAlphaOverride:=overrideAlpha)

                ' Feinabstimmung für Honors (falls gewünscht)
                Dim honorTint As Color? = SeriesMapping.TryGetHonorFaceTint(si, preset)
                If honorTint.HasValue Then
                    st.FaceTint = honorTint.Value
                End If

                result.Add(MahjongTileRenderer.RenderTile(size, symbols(i), st))
            Next
            Return result
        End Function

    End Class

End Namespace

