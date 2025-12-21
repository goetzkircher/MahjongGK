
Namespace MahjongGKSymbolFactory

    ' ''' 
    ' ''' <summary>
    ' ''' Stil-Parameter für den Mahjong-Ziegel.
    ' ''' </summary>
    Public Structure TileStyle
        Public CornerRadius As Single            ' Eckenradius (px)
        Public EdgeWidth As Single               ' Breite des 3D-Rahmens (px)
        Public ShadowSize As Single              ' Größe/Weichheit des Schattens (px)
        Public FaceInset As Single               ' Abstand von Außenkante zur „Oberseite“ (px)
        Public FaceTint As Color                 ' Farbton der Oberseite (wird über Ivory gelegt)
        Public BodyTop As Color                  ' Ziegeloberfläche (Außenkörper) oben
        Public BodyBottom As Color               ' Ziegeloberfläche (Außenkörper) unten
        Public EdgeLight As Color                ' Lichtkante (links/oben)
        Public EdgeDark As Color                 ' Schattenkante (rechts/unten)
        Public FaceIvoryLight As Color           ' Grundfarbe Oberseite (hell)
        Public FaceIvoryDark As Color            ' Grundfarbe Oberseite (dunkler)
        Public GlossOpacity As Integer           ' 0..255
        Public NoiseOpacity As Integer           ' 0..255 (Papierkorn)
        Public SymbolShadowOpacity As Integer    ' 0..255
        Public SymbolScale As Single             ' 0.0..1.0 Anteil der Face-Fläche
        Public FaceTintMinAlpha As Integer       ' 0..255, z.B. 28
        Public FaceTintBoostMax As Single        ' 1.0..2.0, z.B. 1.8F
        ' Kanten-Feinschliff (optional übers INI/Slider steuerbar)
        Public EdgeLightAlpha As Integer         ' 0..255 (überschreibt Alpha von EdgeLight; 0 = nehme EdgeLight.A)
        Public EdgeDarkAlpha As Integer          ' 0..255 (überschreibt Alpha von EdgeDark;  0 = nehme EdgeDark.A)
        Public EdgeLightWidthPx As Single        ' >0 = feste px-Breite, 0 = auto (EdgeWidth*0.16, min 2)
        Public EdgeDarkWidthPx As Single         ' >0 = feste px-Breite, 0 = auto (EdgeWidth*0.22, min 3)


        Public Shared Function CreateDefault(surfaceSize As Size) As TileStyle
            Dim base As Single = CSng(Math.Max(6, Math.Min(surfaceSize.Width, surfaceSize.Height) / 12.5))
            Return New TileStyle With {
                .CornerRadius = base,                       ' ~ moderat abgerundet
                .EdgeWidth = Math.Max(8.0F, base * 0.75F),
                .ShadowSize = Math.Max(18.0F, base * 2.0F),
                .FaceInset = Math.Max(6.0F, base * 0.55F),
                .FaceTint = Color.FromArgb(20, 0, 96, 160), ' zarter Blauton als Default
                .BodyTop = Color.FromArgb(255, 250, 250, 250),
                .BodyBottom = Color.FromArgb(255, 220, 220, 220),
                .EdgeLight = Color.FromArgb(110, 255, 255, 255),
                .EdgeDark = Color.FromArgb(110, 0, 0, 0),
                .FaceIvoryLight = Color.FromArgb(255, 255, 252, 246),
                .FaceIvoryDark = Color.FromArgb(255, 241, 235, 225),
                .GlossOpacity = 80,
                .NoiseOpacity = 18,
                .SymbolShadowOpacity = 110,
                .SymbolScale = 0.86,
                .FaceTintMinAlpha = 28,
                .FaceTintBoostMax = 1.8F,
                .EdgeLightAlpha = 0,        ' 0 = benutze EdgeLight.A
                .EdgeDarkAlpha = 0,         ' 0 = benutze EdgeDark.A
                .EdgeLightWidthPx = 0.0F,   ' 0 = auto
                .EdgeDarkWidthPx = 0.0F
            }
        End Function
        '
        ''' <summary>
        ''' Liefert einen vordefinierten Stil für Mahjong-Ziegel.
        ''' </summary>
        Public Shared Function CreatePreset(preset As TilePreset, surfaceSize As Size) As TileStyle
            Dim st As TileStyle = TileStyle.CreateDefault(surfaceSize)

            Select Case preset
                Case TilePreset.Klassisch
                    ' Warmes Elfenbein, deutliche Kante, dezentes Glanzlicht
                    st.FaceIvoryLight = Color.FromArgb(255, 255, 252, 246)
                    st.FaceIvoryDark = Color.FromArgb(255, 241, 235, 225)
                    st.FaceTint = Color.FromArgb(20, 128, 80, 20)   ' leicht grünlicher Ton
                    st.EdgeWidth = 10
                    st.CornerRadius = 14
                    st.GlossOpacity = 70
                    st.NoiseOpacity = 12
                    st.SymbolShadowOpacity = 120

                Case TilePreset.ModernGlatt
                    ' Kühle, fast weiße Oberfläche, sehr clean
                    st.FaceIvoryLight = Color.FromArgb(255, 250, 250, 250)
                    st.FaceIvoryDark = Color.FromArgb(255, 235, 235, 235)
                    st.FaceTint = Color.FromArgb(10, 40, 40, 120)   ' kühler Blaustich
                    st.EdgeWidth = 6
                    st.CornerRadius = 8
                    st.GlossOpacity = 110
                    st.NoiseOpacity = 0        ' kein Korn -> sehr glatt
                    st.SymbolShadowOpacity = 80

                Case TilePreset.StarkTexturiert
                    ' Deutlich „materieller“ Look mit kräftigem Korn
                    st.FaceIvoryLight = Color.FromArgb(255, 250, 248, 240)
                    st.FaceIvoryDark = Color.FromArgb(255, 228, 220, 210)
                    st.FaceTint = Color.FromArgb(28, 160, 120, 60)  ' warm, erdig
                    st.EdgeWidth = 12
                    st.CornerRadius = 16
                    st.GlossOpacity = 50
                    st.NoiseOpacity = 35       ' deutliches Rauschen für „Porzellan/Keramik“
                    st.SymbolShadowOpacity = 140
            End Select

            Return st
        End Function
    End Structure
End Namespace