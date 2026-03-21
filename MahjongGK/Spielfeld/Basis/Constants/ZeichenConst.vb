Module ZeichenConst


    '''' <summary>
    '''' Tilde („ungefähr“, „ähnlich“, Statistik. (Verteilungen: X ~ N(0,1))
    '''' </summary>
    'Public Const CHAR_UNGEFÄHR1 As Char = "~"c
    ''
    '''' <summary>
    '''' Doppelte Tilde (Näherungswerte, Rundungen, z. B. π ≈ 3,14)
    '''' </summary>
    'Public Const CHAR_UNGEFÄHR2 As Char = "≈"c
    ''
    '''' <summary>
    '''' VORSICHT: NICHT DIKTENGLEICH! (fehlt in den meisten Schriften, daher Fallback in eine Standardschrift. -> Layout wird zerschossen.)
    '''' kongruent / entspricht (Geometrie (Dreiecke), Entsprechung, Ähnlichkeit)
    '''' </summary>
    'Public Const CHAR_ENTSPRICHT As Char = "≅"c

    'Public Const CHAR_DELTA As Char = "Δ"c
    '
    'Public Const CHAR_ As Char = ""c
    'Public Const CHAR_ As Char = ""c
    'Public Const CHAR_ As Char = ""c
End Module
