Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Collections.Concurrent
Imports System.Globalization

Namespace Theme
    ''' <summary>
    ''' ThemeImageCache
    ''' ──────────────────────────────────────────────────────────────────────────────
    ''' Zentraler Bild-Cache für Ressourcen mit optionaler Aufhellung.
    ''' 
    ''' FUNKTION:
    ''' • Lädt Bilder aus den Projektressourcen (My.Resources) per Namen.
    ''' • Erzeugt (optional) aufgehellte Varianten per ColorMatrix.
    ''' • Cacht die erzeugten Images pro Schlüssel (Ressourcenname + Aufhellung).
    ''' • Steuert via deepCopy, ob eine gecachte Instanz (geteilt) oder eine
    '''   eigenständige Bitmap-Kopie (Caller besitzt &amp; muss Dispose aufrufen) geliefert wird.
    ''' 
    ''' API (wichtigste):
    ''' • GetResImg(name, brighten)                                     ' kurz, Standardhelligkeit, keine DeepCopy
    ''' • GetResImg(name, brighten, amount, deepCopy)                    ' erweitert
    ''' • BrightenImageIfNeeded(img) / BrightenImage(img, amount)        ' Utilities
    ''' • ClearCache()                                                   ' Cache leeren + Dispose
    ''' 
    ''' HINWEISE:
    ''' • deepCopy:=False → ideal für UI-Zustände (Normal/Hover/Selected …), niemals selbst Disposen.
    ''' • deepCopy:=True  → liefert immer eine neue Bitmap-Kopie; Caller besitzt &amp; Disposen ist Pflicht.
    ''' • Unterschiedliche Helligkeiten (amount) werden getrennt gecacht.
    ''' • Bei Theme-Wechsel (Dark/Light) ggf. ClearCache() aufrufen.
    ''' </summary>
    Public Module ThemeImageCache

        ' Cache: Schlüssel ist z. B. "CompassN|b0" oder "CompassN|b1:a0.15"
        Private ReadOnly _cache As New ConcurrentDictionary(Of String, Image)()



        ' ======================================================================
        '  KURZE ÜBERLADUNG
        ' ======================================================================

        ''' <summary>
        ''' Holt ein Bild aus den Projektressourcen und liefert es optional aufgehellt zurück.
        ''' 
        ''' Diese Überladung nutzt den globalen Standardwert BrightenAmountDefault
        ''' und erstellt standardmäßig KEINE DeepCopy (deepCopy:=False).
        ''' 
        ''' Parameter:
        ''' • name     – Ressourcenname (z. B. "CompassN").
        ''' • brighten – True = Aufhellung aktiv, False = Originalfarben.
        ''' 
        ''' Rückgabe:
        ''' • Image-Objekt aus dem Cache (geteilter Besitz, nicht selbst Disposen).
        ''' 
        ''' Hinweise:
        ''' • Für UI-Zustände (Normal, Hover, Selected …) gedacht.
        ''' • Soll eine eigene Kopie erzeugt werden (z. B. zum Modifizieren),
        '''   bitte die erweiterte Überladung mit deepCopy:=True verwenden.
        ''' </summary>
        Public Function GetResImg(name As String, brighten As Boolean) As Image
            Return GetResImg(name, brighten, -1.0F, False)
        End Function

        ' ======================================================================
        '  ERWEITERTE ÜBERLADUNG (mit amount + deepCopy)
        ' ======================================================================

        ''' <summary>
        ''' Holt ein Bild aus den Projektressourcen und liefert es optional aufgehellt zurück.
        ''' 
        ''' Parameter:
        ''' • name      – Ressourcenname (z. B. "CompassN").
        ''' • brighten  – True = Aufhellung aktiv, False = Originalfarben.
        ''' • amount    – Helligkeitsbetrag (0.0 … 1.0). Standard = BrightenAmountDefault.
        '''               amount &lt; 0 → BrightenAmountDefault wird verwendet.
        ''' • deepCopy  – Steuerung des Speicherverhaltens:
        '''               False = gecachte Variante (geteilt, nie selbst Disposen!).
        '''               True  = eigene Bitmap-Kopie (Caller ist Eigentümer → Dispose Pflicht!).
        ''' 
        ''' Rückgabe:
        ''' • Image-Objekt. 
        '''   Bei deepCopy:=False immer das gleiche gecachte Objekt pro Key.
        '''   Bei deepCopy:=True jedes Mal eine neue Bitmap-Kopie.
        ''' 
        ''' Hinweise:
        ''' • deepCopy:=False ist ideal für UI-Zustände (Normal, Hover, Selected …).
        ''' • deepCopy:=True ist sinnvoll, wenn das Bild modifiziert oder in einer 
        '''   temporären Grafikoperation verwendet werden soll.
        ''' • Unterschiedliche Aufhellungswerte (amount) werden getrennt im Cache gehalten.
        ''' • Mit ClearCache() können alle gecachten Bilder freigegeben werden.
        ''' </summary>
        Public Function GetResImg(name As String,
                              brighten As Boolean,
                              Optional amount As Single = -1.0F,
                              Optional deepCopy As Boolean = False) As Image
            If String.IsNullOrWhiteSpace(name) Then Return Nothing

            ' Ziel-Aufhellung ermitteln/klammern
            Dim useAmount As Single = If(amount < 0.0F, INI.Global_BrightenAmount, amount)
            If useAmount < 0.0F Then useAmount = 0.0F
            If useAmount > 1.0F Then useAmount = 1.0F

            ' Cache-Key
            Dim key As String
            If brighten Then
                key = name & "|b1:a" & useAmount.ToString(CultureInfo.InvariantCulture)
            Else
                key = name & "|b0"
            End If

            'Dim baseObj As Object = MjGfx_GfxMain(name, 16)
            ' Aus Cache holen oder neu erzeugen
            Dim img As Image = _cache.GetOrAdd(
            key,
            Function(k As String) As Image
                Dim baseObj As Object = My.Resources.ResourceManager.GetObject(name, My.Resources.Culture)
                If baseObj Is Nothing Then Return Nothing

                Dim baseImg As Image = DirectCast(baseObj, Image)

                ' Eigene Kopie der Basis erstellen, Cache besitzt diese Instanz
                Dim clone As New Bitmap(baseImg)

                If Not brighten OrElse useAmount <= 0.0001F Then
                    Return clone
                End If

                ' Aufhellen und Ergebnis zurückgeben (Cache-Besitz)
                Return MjGDI.BrightenImage(clone, useAmount)
            End Function)

            ' DeepCopy: dem Aufrufer eine eigene Bitmap übergeben (Caller besitzt)
            If deepCopy AndAlso img IsNot Nothing Then
                Return New Bitmap(img)
            End If

            ' Geteilte (gecachte) Instanz
            Return img
        End Function

        ''' <summary>
        ''' Liefert eine Bitmap-Variante aus dem Cache (geteilter Besitz; nicht disposen).
        ''' holt sich brighten aus INI.Global_DarkMode
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function GetResBmp(name As String) As Bitmap
            Return GetResBmp(name, INI.Global_DarkMode)
        End Function

        ''' <summary>
        ''' Analog zu GetResImg(name, brighten).
        ''' Liefert eine Bitmap-Variante aus dem Cache (geteilter Besitz; nicht disposen).
        ''' </summary>
        Public Function GetResBmp(name As String, brighten As Boolean) As Bitmap
            Dim img As Image = GetResImg(name, brighten, -1.0F, False)
            If img Is Nothing Then Return Nothing
            ' Robust: falls jemals kein Bitmap zurückkäme, sichere Kopie erzeugen
            If TypeOf img Is Bitmap Then
                Return DirectCast(img, Bitmap)
            Else
                Return New Bitmap(img)
            End If
        End Function

        ''' <summary>
        ''' Analog zu GetResImg(name, brighten, amount, deepCopy).
        ''' deepCopy:=False → gecachte Bitmap (geteilter Besitz; nicht disposen).
        ''' deepCopy:=True  → eigene Bitmap-Kopie (Caller besitzt; Dispose Pflicht).
        ''' </summary>
        Public Function GetResBmp(name As String,
                          brighten As Boolean,
                          Optional amount As Single = -1.0F,
                          Optional deepCopy As Boolean = False) As Bitmap
            Dim img As Image = GetResImg(name, brighten, amount, deepCopy)
            If img Is Nothing Then Return Nothing
            If deepCopy Then
                ' GetResImg hat bereits kopiert – dennoch defensiv echte eigene Kopie
                Return New Bitmap(img)
            End If
            If TypeOf img Is Bitmap Then
                Return DirectCast(img, Bitmap)
            Else
                Return New Bitmap(img)
            End If
        End Function


        ' ======================================================================
        '  CACHE-WARTUNG
        ' ======================================================================

        ''' <summary>
        ''' Leert den Cache und gibt alle gecachten Bilder frei (Dispose).
        ''' Nachfolgende Zugriffe erzeugen die Images neu.
        ''' </summary>
        Public Sub ClearCache()
            For Each kv As KeyValuePair(Of String, Image) In _cache
                kv.Value?.Dispose()
            Next
            _cache.Clear()
        End Sub

        ' ======================================================================
        '  AUFHELL-UTILITIES
        ' ======================================================================

        ''' <summary>
        ''' Convenience: Erzeugt eine aufgehellte Kopie mit BrightenAmountDefault.
        ''' Original bleibt unverändert.
        ''' </summary>
        Public Function BrightenImageIfNeeded(src As Image) As Image
            Return MjGDI.BrightenImage(src, INI.Global_BrightenAmount)
        End Function

    End Module
End Namespace