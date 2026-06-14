Public Module IniEnumerationen

    ''' <summary>
    ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString"
    ''' Verwendung: Entweder über Dim Path As String = INI.AppDataDefault(.....)
    ''' oder durch Nutzung (im Modul INI) einer Public Property Kopier_VorlageFürPfade As String
    ''' Die gewünschten Pfade werden automatisch angelegt.
    ''' </summary>
    Public Enum AppDataSubDir
        None
        INI
        Spiele
        Diverses
        Hintergrundgrafiken
        Eigene_Hintergrundgrafiken
        Temporär
    End Enum
    ''' <summary>
    ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString\SubSubDefault.value.ToString"
    ''' Verwendung wie AppDataSubDir
    ''' </summary>
    Public Enum AppDataSubSubDir
        None
        LetztesSpiel
        EigeneSpiele
        Spielesammlung
        Diverses_ScreenShots
        Temporär_SpielfeldInfo
    End Enum

    ''' <summary>
    ''' Enumeration der verwendeten Dateinamen.
    ''' Die Endung mit einem Unterstrich abtrennen.
    ''' Die Endung muss 3 Zeichen lang sein.
    ''' </summary>
    Public Enum AppDataFileName
        None
        Steininfos_xml
        ScreeenShot_png
        Spielfeldinfo_xml
    End Enum

    Public Enum AppDataTimeStamp
        None
        Add
        LookForLastTimeStamp
    End Enum

    ''' <summary>
    ''' In dieser Enum kann ein Pattern verschlüsselt werden.
    ''' Es gilt: 
    ''' _Q_ = ? (Question, Fragezeichen),
    ''' _N_ = # (Number),
    ''' _S_ = * (Stern, Star),
    ''' _D_ = . (Dot, Punkt).
    ''' _SD_ = *.
    ''' _SDS_ = *.*
    ''' Beispiel: Dateiname_S__D_ext --> Dateiname*.ext
    ''' </summary>
    Public Enum AppDataFilePattern
        None
        Steininfos_xml
    End Enum

    ''' <summary>
    ''' Auf englisch, weil in der INI
    ''' Die verwendeten Steinsätze
    ''' </summary>
    Public Enum TileSetInUse
        InternalSet
    End Enum

End Module
