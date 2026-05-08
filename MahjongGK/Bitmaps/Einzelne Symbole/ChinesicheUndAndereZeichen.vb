Public Module ChinesicheUndAndereZeichen

    'Hinweis: Farbverläufe mit dem GlyphGradientRenderer

    Public ReadOnly MahjongSymbols As String() = {
        "鐵",  ' Error-Zeichen – Eisen, „Fehler“ (rot dargestellt)
              _
              _  ' Gruppe A – 1. Farbe, offen/kantig (9)
        "山",  ' Berg
        "木",  ' Baum/Holz
        "水",  ' Wasser
        "火",  ' Feuer
        "王",  ' König
        "中",  ' Mitte/Zentrum
        "大",  ' Groß
        "小",  ' Klein
        "人",  ' Mensch
              _
              _ ' Gruppe B – 1. Farbe, rund/geschlossen (9)
        "口",  ' Mund
        "回",  ' Zurück, Umrahmung
        "園",  ' Garten, umschlossener Bereich
        "目",  ' Auge
        "耳",  ' Ohr
        "足",  ' Fuß
        "車",  ' Wagen
        "船",  ' Schiff
        "門",  ' Tor, Tür
              _
              _ ' Gruppe C – 1. Farbe, komplex/kontrastreich (9)
        "龍",  ' Drache
        "鳳",  ' Phönix
        "龜",  ' Schildkröte
        "書",  ' Buch
        "鍾",  ' Glocke
        "國",  ' Land, Reich
        "圓",  ' Kreis, rund
        "豐",  ' Fülle, Überfluss
        "樂",  ' Freude, Musik
              _
              _   ' Gruppe G – 1. Farbe, 3er-Gruppe (3)
        "愛",  ' Liebe
        "恩",  ' Gnade, Dankbarkeit
        "悟",  ' Erleuchtung, Einsicht
              _
              _ ' Gruppe D – 2. Farbe, Dach-Zeichen (4)
        "金",  ' Gold, Metall
        "家",  ' Haus, Familie
        "劍",  ' Schwert
        "銅",  ' Kupfer
              _
              _  ' Gruppe E – 3. Farbe, gemischt (4)
        "弓",  ' Bogen
        "刀",  ' Messer
        "心",  ' Herz
        "北",  ' Norden
              _
              _  ' Gruppe F – 4. Farbe, Jahreszeiten (4)
        "春",  ' Frühling
        "夏",  ' Sommer
        "秋",  ' Herbst
        "冬"   ' Winter
    }

    ' Bedeutungen in derselben Reihenfolge wie MahjongSymbols()
    Public ReadOnly MahjongSymbolMeanings As String() = {
        "Errorzeichen – Eisen (rot)",
                                     _
                                     _   ' Gruppe A – offen/kantig
        "Berg",
        "Baum / Holz",
        "Wasser",
        "Feuer",
        "König",
        "Mitte / Zentrum",
        "Groß",
        "Klein",
        "Mensch",
                 _
                 _   ' Gruppe B – rund/geschlossen
        "Mund",
        "Zurück / Umrahmung",
        "Garten / umschlossener Bereich",
        "Auge",
        "Ohr",
        "Fuß",
        "Wagen",
        "Schiff",
        "Tor / Tür",
                    _
                    _   ' Gruppe C – komplex/kontrastreich
        "Drache",
        "Phönix",
        "Schildkröte",
        "Buch",
        "Glocke",
        "Land / Reich",
        "Kreis / rund",
        "Fülle / Überfluss",
        "Freude / Musik",
                         _
                         _   ' Gruppe G – 3er-Gruppe
        "Liebe",
        "Gnade / Dankbarkeit",
        "Erleuchtung / Einsicht",
                                 _
                                 _   ' Gruppe D – Dach-Zeichen
        "Gold / Metall",
        "Haus / Familie",
        "Schwert",
        "Kupfer",
                 _
                 _   ' Gruppe E – gemischt
        "Bogen",
        "Messer",
        "Herz",
        "Norden",
                 _
                 _   ' Gruppe F – Jahreszeiten
        "Frühling",
        "Sommer",
        "Herbst",
        "Winter"
    }

    ' Symbole – Set 1: Klar & geometrisch
    Dim MahjongSymbolsSet1 As String() = {
    "⛔",  ' Error – Verbotsschild _
    "●",  ' Kreis gefüllt _
    "○",  ' Kreis leer _
    "■",  ' Quadrat gefüllt _
    "□",  ' Quadrat leer _
    "▲",  ' Dreieck oben _
    "△",  ' Dreieck unten _
    "◆",  ' Raute gefüllt _
    "◇",  ' Raute leer _
    "★",  ' Stern gefüllt _
    "✶",  ' Stern klein _
    "✷",  ' Stern groß _
    "✸",  ' Stern funkelnd _
    "✹",  ' Stern Blüte _
    "✺",  ' Stern Variation 1 _
    "✻",  ' Stern Variation 2 _
    "✼",  ' Stern Variation 3 _
    "✽",  ' Stern Variation 4 _
    "✾",  ' Stern Variation 5 _
    "←",  ' Pfeil links _
    "↑",  ' Pfeil oben _
    "→",  ' Pfeil rechts _
    "↓",  ' Pfeil unten _
    "↔",  ' Pfeil links-rechts _
    "↕",  ' Pfeil oben-unten _
    "↩",  ' Pfeil zurück _
    "↪",  ' Pfeil vorwärts _
    "↺",  ' Pfeil kreisend _
    "✖",  ' Kreuz _
    "✔",  ' Haken _
    "✚",  ' Plus _
    "✱",  ' Sternchen _
    "▴",  ' Dreieck hoch _
    "▾",  ' Dreieck runter _
    "◂",  ' Dreieck links _
    "▸",  ' Dreieck rechts _
    "◆",  ' Raute gefüllt _
    "◈",  ' Raute gemustert _
    "◇"   ' Raute leer _
}

    ' Symbole – Set 2: Spielerisch / Emoji
    Dim MahjongSymbolsSet2 As String() = {
    "💢",  ' Error – Wutzeichen _
    "☀",  ' Sonne _
    "☁",  ' Wolke _
    "☂",  ' Regenschirm _
    "☃",  ' Schneemann _
    "🌙",  ' Mond _
    "⚡",  ' Blitz _
    "🌈",  ' Regenbogen _
    "❄",  ' Schneeflocke _
    "☔",  ' Regenschauer _
    "🌸",  ' Kirschblüte _
    "🌼",  ' Blume gelb _
    "🌻",  ' Sonnenblume _
    "🌹",  ' Rose _
    "🌷",  ' Tulpe _
    "🍀",  ' Kleeblatt _
    "🍂",  ' Herbstblatt braun _
    "🍁",  ' Herbstblatt rot _
    "🌺",  ' Hibiskus _
    "🐱",  ' Katze _
    "🐶",  ' Hund _
    "🐦",  ' Vogel _
    "🐟",  ' Fisch _
    "🐢",  ' Schildkröte _
    "🦋",  ' Schmetterling _
    "🐞",  ' Marienkäfer _
    "🐘",  ' Elefant _
    "🐍",  ' Schlange _
    "👍",  ' Daumen hoch _
    "👎",  ' Daumen runter _
    "👌",  ' OK-Hand _
    "✌",  ' Victory-Zeichen _
    "❤️",  ' Herz rot _
    "💛",  ' Herz gelb _
    "💚",  ' Herz grün _
    "💙",  ' Herz blau _
    "🎵",  ' Note einfach _
    "🎶",  ' Noten doppelt _
    "🎼"   ' Notensystem _
}


    ' Symbole – Set 3: Klassisch / Piktogramm
    Dim MahjongSymbolsSet3 As String() = {
    "☠",  ' Error – Totenkopf _
    "●",  ' Kreis gefüllt _
    "○",  ' Kreis leer _
    "◎",  ' Kreis Doppel _
    "◉",  ' Kreis Punkt innen _
    "◌",  ' Kreis gestrichelt _
    "◍",  ' Kreis halbfett _
    "◯",  ' Kreis groß _
    "⚫",  ' Kreis schwarz _
    "⚪",  ' Kreis weiß _
    "■",  ' Quadrat gefüllt _
    "□",  ' Quadrat leer _
    "▪",  ' Quadrat klein schwarz _
    "▫",  ' Quadrat klein weiß _
    "◼",  ' Quadrat mittel schwarz _
    "◻",  ' Quadrat mittel weiß _
    "⬛",  ' Quadrat groß schwarz _
    "⬜",  ' Quadrat groß weiß _
    "▣",  ' Quadrat mit Muster _
    "★",  ' Stern gefüllt _
    "☆",  ' Stern leer _
    "✦",  ' Stern funkelnd 1 _
    "✧",  ' Stern funkelnd 2 _
    "✩",  ' Stern Variation 1 _
    "✪",  ' Stern Variation 2 _
    "✫",  ' Stern Variation 3 _
    "✬",  ' Stern Variation 4 _
    "✭",  ' Stern Variation 5 _
    "☽",  ' Mond links _
    "☾",  ' Mond rechts _
    "🌙",  ' Mond neu _
    "☪",  ' Mond mit Stern _
    "➔",  ' Pfeil rechts _
    "➘",  ' Pfeil rechts-unten _
    "➚",  ' Pfeil rechts-oben _
    "➤",  ' Pfeil dick _
    "✚",  ' Plus _
    "✖",  ' Kreuz _
    "✛"   ' Doppelplus _
}

    Dim MahjongMeaningsSet2 As String() = {
    "Error – Wutzeichen",
    "Sonne",
    "Wolke",
    "Regenschirm",
    "Schneemann",
    "Mond",
    "Blitz",
    "Regenbogen",
    "Schneeflocke",
    "Regenschauer",
    "Kirschblüte",
    "Blume gelb",
    "Sonnenblume",
    "Rose",
    "Tulpe",
    "Kleeblatt",
    "Herbstblatt braun",
    "Herbstblatt rot",
    "Hibiskus",
    "Katze",
    "Hund",
    "Vogel",
    "Fisch",
    "Schildkröte",
    "Schmetterling",
    "Marienkäfer",
    "Elefant",
    "Schlange",
    "Daumen hoch",
    "Daumen runter",
    "OK-Hand",
    "Victory-Zeichen",
    "Herz rot",
    "Herz gelb",
    "Herz grün",
    "Herz blau",
    "Note einfach",
    "Noten doppelt",
    "Notensystem"
}


    Dim MahjongMeaningsSet3 As String() = {
    "Error – Totenkopf",
    "Kreis gefüllt",
    "Kreis leer",
    "Kreis Doppel",
    "Kreis Punkt innen",
    "Kreis gestrichelt",
    "Kreis halbfett",
    "Kreis groß",
    "Kreis schwarz",
    "Kreis weiß",
    "Quadrat gefüllt",
    "Quadrat leer",
    "Quadrat klein schwarz",
    "Quadrat klein weiß",
    "Quadrat mittel schwarz",
    "Quadrat mittel weiß",
    "Quadrat groß schwarz",
    "Quadrat groß weiß",
    "Quadrat mit Muster",
    "Stern gefüllt",
    "Stern leer",
    "Stern funkelnd 1",
    "Stern funkelnd 2",
    "Stern Variation 1",
    "Stern Variation 2",
    "Stern Variation 3",
    "Stern Variation 4",
    "Stern Variation 5",
    "Mond links",
    "Mond rechts",
    "Mond neu",
    "Mond mit Stern",
    "Pfeil rechts",
    "Pfeil rechts-unten",
    "Pfeil rechts-oben",
    "Pfeil dick",
    "Plus",
    "Kreuz",
    "Doppelplus"
}


End Module
