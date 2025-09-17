Imports System.Globalization

Namespace MahjongGKSymbolFactory
    '
    ''' <summary>
    ''' Live-editierbare Render-Settings für TileStyle (INI-gebunden).
    ''' </summary>
    Public Class TileRenderSettings
        Public Property CornerRadius As Single = 14.0F
        Public Property EdgeWidth As Single = 10.0F
        Public Property ShadowSize As Single = 24.0F
        Public Property FaceInset As Single = 8.0F

        Public Property FaceTint As Color = Color.FromArgb(20, 0, 96, 160)
        Public Property BodyTop As Color = Color.FromArgb(255, 250, 250, 250)
        Public Property BodyBottom As Color = Color.FromArgb(255, 220, 220, 220)
        Public Property EdgeLight As Color = Color.FromArgb(110, 255, 255, 255)
        Public Property EdgeDark As Color = Color.FromArgb(110, 0, 0, 0)
        Public Property FaceIvoryLight As Color = Color.FromArgb(255, 255, 252, 246)
        Public Property FaceIvoryDark As Color = Color.FromArgb(255, 241, 235, 225)

        Public Property GlossOpacity As Integer = 80          ' 0..255
        Public Property NoiseOpacity As Integer = 18          ' 0..255
        Public Property SymbolShadowOpacity As Integer = 110  ' 0..255
        Public Property SymbolScale As Single = 0.86F         ' 0..1
        '
        Public Property FaceTintMinAlpha As Integer = 28
        Public Property FaceTintBoostMax As Single = 1.8F
        '
        Public Property EdgeLightAlpha As Integer = 0
        Public Property EdgeDarkAlpha As Integer = 0
        Public Property EdgeLightWidthPx As Single = 0.0F
        Public Property EdgeDarkWidthPx As Single = 0.0F

        '
        ''' <summary>
        ''' Lädt Werte aus einer INI-Sektion in diese Instanz.
        ''' </summary>
        Public Sub LoadFromIni(ini As IniFile, section As String)
            CornerRadius = ini.ReadSingle(section, "CornerRadius", CornerRadius)
            EdgeWidth = ini.ReadSingle(section, "EdgeWidth", EdgeWidth)
            ShadowSize = ini.ReadSingle(section, "ShadowSize", ShadowSize)
            FaceInset = ini.ReadSingle(section, "FaceInset", FaceInset)

            FaceTint = ini.ReadColor(section, "FaceTint", FaceTint)
            BodyTop = ini.ReadColor(section, "BodyTop", BodyTop)
            BodyBottom = ini.ReadColor(section, "BodyBottom", BodyBottom)
            EdgeLight = ini.ReadColor(section, "EdgeLight", EdgeLight)
            EdgeDark = ini.ReadColor(section, "EdgeDark", EdgeDark)
            FaceIvoryLight = ini.ReadColor(section, "FaceIvoryLight", FaceIvoryLight)
            FaceIvoryDark = ini.ReadColor(section, "FaceIvoryDark", FaceIvoryDark)

            GlossOpacity = ini.ReadInt(section, "GlossOpacity", GlossOpacity)
            NoiseOpacity = ini.ReadInt(section, "NoiseOpacity", NoiseOpacity)
            SymbolShadowOpacity = ini.ReadInt(section, "SymbolShadowOpacity", SymbolShadowOpacity)
            SymbolScale = ini.ReadSingle(section, "SymbolScale", SymbolScale)

            FaceTintMinAlpha = ini.ReadInt(section, "FaceTintMinAlpha", FaceTintMinAlpha)
            FaceTintBoostMax = ini.ReadSingle(section, "FaceTintBoostMax", FaceTintBoostMax)

            EdgeLightAlpha = ini.ReadInt(section, "EdgeLightAlpha", EdgeLightAlpha)
            EdgeDarkAlpha = ini.ReadInt(section, "EdgeDarkAlpha", EdgeDarkAlpha)
            EdgeLightWidthPx = ini.ReadSingle(section, "EdgeLightWidthPx", EdgeLightWidthPx)
            EdgeDarkWidthPx = ini.ReadSingle(section, "EdgeDarkWidthPx", EdgeDarkWidthPx)

        End Sub

        '
        ''' <summary>
        ''' Schreibt aktuelle Werte in eine INI-Sektion.
        ''' </summary>
        Public Sub SaveToIni(ini As IniFile, section As String)
            ini.WriteString(section, "CornerRadius", CornerRadius.ToString(CultureInfo.InvariantCulture))
            ini.WriteString(section, "EdgeWidth", EdgeWidth.ToString(CultureInfo.InvariantCulture))
            ini.WriteString(section, "ShadowSize", ShadowSize.ToString(CultureInfo.InvariantCulture))
            ini.WriteString(section, "FaceInset", FaceInset.ToString(CultureInfo.InvariantCulture))

            ini.WriteString(section, "FaceTint", IniFile.ColorToString(FaceTint))
            ini.WriteString(section, "BodyTop", IniFile.ColorToString(BodyTop))
            ini.WriteString(section, "BodyBottom", IniFile.ColorToString(BodyBottom))
            ini.WriteString(section, "EdgeLight", IniFile.ColorToString(EdgeLight))
            ini.WriteString(section, "EdgeDark", IniFile.ColorToString(EdgeDark))
            ini.WriteString(section, "FaceIvoryLight", IniFile.ColorToString(FaceIvoryLight))
            ini.WriteString(section, "FaceIvoryDark", IniFile.ColorToString(FaceIvoryDark))

            ini.WriteString(section, "GlossOpacity", GlossOpacity.ToString(CultureInfo.InvariantCulture))
            ini.WriteString(section, "NoiseOpacity", NoiseOpacity.ToString(CultureInfo.InvariantCulture))
            ini.WriteString(section, "SymbolShadowOpacity", SymbolShadowOpacity.ToString(CultureInfo.InvariantCulture))
            ini.WriteString(section, "SymbolScale", SymbolScale.ToString(CultureInfo.InvariantCulture))

            ini.WriteString(section, "FaceTintMinAlpha", FaceTintMinAlpha.ToString(System.Globalization.CultureInfo.InvariantCulture))
            ini.WriteString(section, "FaceTintBoostMax", FaceTintBoostMax.ToString(System.Globalization.CultureInfo.InvariantCulture))

            ini.WriteString(section, "EdgeLightAlpha", EdgeLightAlpha.ToString(Globalization.CultureInfo.InvariantCulture))
            ini.WriteString(section, "EdgeDarkAlpha", EdgeDarkAlpha.ToString(Globalization.CultureInfo.InvariantCulture))
            ini.WriteString(section, "EdgeLightWidthPx", EdgeLightWidthPx.ToString(Globalization.CultureInfo.InvariantCulture))
            ini.WriteString(section, "EdgeDarkWidthPx", EdgeDarkWidthPx.ToString(Globalization.CultureInfo.InvariantCulture))

        End Sub

        '
        ''' <summary>
        ''' Überträgt die Settings auf einen TileStyle (für die aktuelle Zielgröße bereits richtig).
        ''' </summary>
        Public Function ToTileStyle(baseStyle As TileStyle) As TileStyle
            Dim st As TileStyle = baseStyle
            st.CornerRadius = CornerRadius
            st.EdgeWidth = EdgeWidth
            st.ShadowSize = ShadowSize
            st.FaceInset = FaceInset

            st.FaceTint = FaceTint
            st.BodyTop = BodyTop
            st.BodyBottom = BodyBottom
            st.EdgeLight = EdgeLight
            st.EdgeDark = EdgeDark
            st.FaceIvoryLight = FaceIvoryLight
            st.FaceIvoryDark = FaceIvoryDark

            st.GlossOpacity = GlossOpacity
            st.NoiseOpacity = NoiseOpacity
            st.SymbolShadowOpacity = SymbolShadowOpacity
            st.SymbolScale = SymbolScale

            st.FaceTintMinAlpha = FaceTintMinAlpha
            st.FaceTintBoostMax = FaceTintBoostMax

            st.EdgeLightAlpha = EdgeLightAlpha
            st.EdgeDarkAlpha = EdgeDarkAlpha
            st.EdgeLightWidthPx = EdgeLightWidthPx
            st.EdgeDarkWidthPx = EdgeDarkWidthPx


            Return st
        End Function
    End Class
End Namespace
