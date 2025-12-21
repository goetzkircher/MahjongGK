Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Const TEST_STUBS = False ' <- auf False stellen, wenn die echten Enums/INI/Funktionen vorhanden sind

Imports MahjongGK.Spielfeld

Public Module SpielfeldTest_SpielsteinGenerator

    Public Sub RunAll()
        Debug.WriteLine("=== SpielsteinGenerator Smoke-Test ===")

        Test_StoneSet_Mode()
        Test_StoneStream_Mode()
        Test_Shuffle_And_Insert()
        Test_StrohwitwenLogik()
        Test_Statistik()

        Debug.WriteLine("=== Smoke-Test fertig ===")
    End Sub

    Private Sub Test_StoneSet_Mode()
        Debug.WriteLine(vbCrLf & "--- StoneSet-Modus ---")

        Dim gen As New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneSet_144)
        ' Test: Vorrat initial gefüllt?
        Debug.WriteLine($"Init Vorrat.Count = {gen.Vorrat.Count}")

        ' Erwartung: Steinzahl ist Vielfaches von 144 oder 152 (abhängig von INI.Editor_stoneSet144SteineErzeugen)
        Dim n As Integer = gen.Vorrat.Count
        Dim ok144 As Boolean = (n Mod 144 = 0)
        Dim ok152 As Boolean = (n Mod 152 = 0)
        Debug.WriteLine($"Vorrat Mod 144 = {n Mod 144}, Mod 152 = {n Mod 152}")
        If Not (ok144 OrElse ok152) Then
            Debug.WriteLine("WARN: Im StoneSet-Modus ist die Steinsumme nicht Vielfaches von 144/152 (prüfe INI/Logik).")
        End If

        ' Ziehe 10 Steine (jeweils RemoveAt an exakt dem Index)
        For i As Integer = 1 To 10
            Dim idx As Integer = Math.Min(gen.Vorrat.Count - 1, 5)
            Dim s As SteinIndexEnum = gen.GetSelectedStein(idx)
            Debug.WriteLine($"Zug {i}: Index={idx}, Stein={s}, Rest={gen.Vorrat.Count}")
        Next

        ' Refill im StoneSet nur bei leerem Vorrat (per Default)
        Dim before As Integer = gen.Vorrat.Count
        gen.CheckAndRefillVorrat()
        Debug.WriteLine($"Refill-Versuch bei Count={before} -> Count={gen.Vorrat.Count} (sollte unverändert bleiben, wenn >0)")

        ' Jetzt Vorrat leeren und Refill explizit erlauben
        gen.Vorrat.Clear()
        gen.CheckAndRefillVorrat(refillStoneSet:=True)
        Debug.WriteLine($"Leer+Refill (StoneSet): Count={gen.Vorrat.Count} (sollte wieder gefüllt sein)")
    End Sub

    Private Sub Test_StoneStream_Mode()
        Debug.WriteLine(vbCrLf & "--- StoneStream-Modus ---")

        Dim gen As New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneStream_Base144_Continuous)
        Dim startCount As Integer = gen.Vorrat.Count
        Debug.WriteLine($"Init Vorrat.Count = {startCount}")

        ' In kleinen Portionen nachfüllen lassen (unter Schwelle)
        gen.Vorrat.Clear()
        gen.VorratNachschubschwelle = 100 ' klein halten, damit Refill greift
        gen.VorratMaxUBound = 400

        gen.CheckAndRefillVorrat()
        Debug.WriteLine($"Nach erstem Refill (StoneStream) Count={gen.Vorrat.Count}")

        ' **Invariante:** StoneStream liefert Paare => Gesamtzahl stets gerade
        If (gen.Vorrat.Count And 1) <> 0 Then
            Debug.WriteLine("FEHLER: StoneStream-ReFill hat ungerade Anzahl erzeugt.")
        End If

        ' Ziehe 25 Steine → danach Refill anstoßen
        For i As Integer = 1 To 25
            Dim s As SteinIndexEnum = gen.GetSelectedStein(0)
        Next
        Debug.WriteLine($"Nach 25 Ziehungen: Count={gen.Vorrat.Count}")

        gen.CheckAndRefillVorrat()
        Debug.WriteLine($"Nach erneutem Refill: Count={gen.Vorrat.Count} (<= MaxUBound={gen.VorratMaxUBound})")
    End Sub

    Private Sub Test_Shuffle_And_Insert()
        Debug.WriteLine(vbCrLf & "--- Shuffle & Insert ---")

        Dim gen As New SpielsteinGenerator(visibleAreaMaxLength:=20, generatorMode:=GeneratorModi.StoneStream_Base152_Continuous) With {
            .VorratNoSortAreaEndIndex = 5
        }

        Debug.WriteLine($"Vor Shuffle (erste 12): {Dump(gen.Vorrat, 12)}")
        gen.ShuffleVorrat()
        Debug.WriteLine($"Nach Shuffle (erste 12): {Dump(gen.Vorrat, 12)}")

        ' Insert links von Index 3
        Dim testStein As SteinIndexEnum = SteinIndexEnum.Punkt01
        gen.InsertLeftFromSelectedStein(index:=3, sie:=testStein)
        Debug.WriteLine($"Nach Insert links von 3 (erste 8): {Dump(gen.Vorrat, 8)}")

        ' GetSelectedStein löscht exakt an Position
        Dim old As SteinIndexEnum = gen.Vorrat(3)
        Dim got As SteinIndexEnum = gen.GetSelectedStein(3)
        Debug.WriteLine($"Vorher an Pos 3: {old}, GetSelected(3)={got} → OK={old.Equals(got)}")
    End Sub

    Private Sub Test_StrohwitwenLogik()
        Debug.WriteLine(vbCrLf & "--- Strohwitwen ---")

        Dim gen As New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneStream_Base152_Continuous)
        ' Künstlich ein paar Steine duplizieren/entfernen, um ungerade Häufigkeit zu erzwingen
        ' Wir nehmen einige Normal-Steine und löschen ihren Partner
        ' (Vereinfachung: wir erzwingen garantiert ungerade Counts)
        EnsureOddCount(gen.Vorrat, SteinIndexEnum.Punkt01)
        EnsureOddCount(gen.Vorrat, SteinIndexEnum.Bambus3)
        EnsureOddCount(gen.Vorrat, SteinIndexEnum.Symbol7)

        Dim hasWitwenVorher As Boolean = gen.VorratHasStrohWitwen(windsAreInOneClickGroup:=False)
        Debug.WriteLine($"HasStrohwitwen (vorher, nur berechnet): {hasWitwenVorher}")

        Dim cnt As Integer = gen.RemovePaareFromVorrat(windsAreInOneClickGroup:=False)
        Debug.WriteLine($"RemovePaareFromVorrat → Rest={cnt}, StopNachschub={gen.VorratStopNachschub}")
        Debug.WriteLine($"Inhalt (bis 20): {Dump(gen.Vorrat, 20)}")

        ' Danach wieder freigeben & auffüllen
        gen.VorratStopNachschub = False
        Debug.WriteLine($"StopNachschub wieder False → Count={gen.Vorrat.Count}")
    End Sub

    Private Sub Test_Statistik()

        Debug.WriteLine(vbCrLf & "--- Statistik GeneratorModi.StoneSet ---")
        Dim gen As New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneSet_144)
        'gen.Vorrat.Clear()
        'gen.VorratNachschubschwelle = 100 ' klein halten, damit Refill greift
        'gen.VorratMaxUBound = 400

        gen.DebugPrintStatistic()
        Debug.WriteLine(vbCrLf & "--- Statistik GeneratorModi.StoneStream ---")
        gen = New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneStream_Base152_Continuous)
        Debug.WriteLine($"Nach erstem Refill (StoneStream) Count={gen.Vorrat.Count}")
        For pass As Integer = 1 To 10
            gen.DebugPrintStatistic()
            gen.Vorrat.Clear()
            gen.CheckAndRefillVorrat()
        Next

    End Sub


    ' --- Helpers -------------------------------------------------------------

    Private Function Dump(list As List(Of SteinIndexEnum), Optional take As Integer = 10) As String
        If list Is Nothing OrElse list.Count = 0 Then Return "(leer)"
        Dim n As Integer = Math.Min(list.Count, take)
        Dim parts As New List(Of String)(n)
        For i As Integer = 0 To n - 1
            parts.Add(list(i).ToString())
        Next
        Return String.Join(", ", parts)
    End Function

    Private Sub EnsureOddCount(vorrat As List(Of SteinIndexEnum), sie As SteinIndexEnum)
        ' Zähle sie
        Dim c As Integer = 0
        For Each s As SteinIndexEnum In vorrat
            If s = sie Then c += 1
        Next
        If (c And 1) = 0 Then
            ' Geradzahlig → einen entfernen, damit ungerade wird
            Dim idx As Integer = vorrat.FindIndex(Function(x) x = sie)
            If idx >= 0 Then vorrat.RemoveAt(idx)
        End If
    End Sub

End Module

' =====================================================================
'                          STUBS (optional)
' =====================================================================

#If TEST_STUBS Then

Namespace Spielfeld
    ' --- Enums (Kurzfassung) ---
    Public Enum GeneratorModi
        StoneStream
        StoneSet
    End Enum

    Public Enum SteinRndGruppe
        Normal
        Flower
        Season
    End Enum

    Public Enum SteinIndexEnum
        Dummy = 0
        Punkt1 = 1
        Punkt2 = 2
        Punkt3 = 3
        Punkt4 = 4
        Punkt5 = 5
        Punkt6 = 6
        Punkt7 = 7
        Punkt8 = 8
        Punkt9 = 9
        Bambus1 = 10
        Bambus2 = 11
        Bambus3 = 12
        Bambus4 = 13
        Bambus5 = 14
        Bambus6 = 15
        Bambus7 = 16
        Bambus8 = 17
        Bambus9 = 18
        Symbol1 = 19
        Symbol2 = 20
        Symbol3 = 21
        Symbol4 = 22
        Symbol5 = 23
        Symbol6 = 24
        Symbol7 = 25
        Symbol8 = 26
        Symbol9 = 27
        DrachenRot = 28
        DrachenGrün = 29
        DrachenWeiß = 30
        WindOst = 31
        WindSüd = 32
        WindWest = 33
        WindNord = 34
        BlütePflaume = 35
        BlüteOrchidee = 36
        BlüteChrisantheme = 37
        BlüteBambus = 38
        JahrFrühling = 39
        JahrSommer = 40
        JahrHerbst = 41
        JahrWinter = 42
    End Enum
End Namespace

' Max-Index der Enum (0..42)
Friend Const MJ_STEININDEX_MAX As Integer = 42

' --- INI-Defaults für den Test ---
Friend NotInheritable Class INI
    Private Sub New()
    End Sub
    Public Shared ReadOnly Property Editor_HalfSteinSetCountDefault As Integer = 2
    Public Shared ReadOnly Property Editor_stoneSet144SteineErzeugen As Boolean = True
    Public Shared ReadOnly Property Editor_VorratNoSortAreaEndIndexDefault As Integer = 10
    Public Shared ReadOnly Property Editor_VorratMaxUBoundDefault As Integer = 500
    Public Shared ReadOnly Property Editor_VorratNachschubschwelleDefault As Integer = 100
    Public Shared ReadOnly Property Editor_StandardMengeSteinPaareJePortion As Integer = 30
    Public Shared ReadOnly Property Editor_VerhältnisNormalsteineZuSondersteine As Double = 17.0R
End Class

' --- Gruppenbildung (stark vereinfachte Version)
Public  Function GetSteinClickGruppe(s As Spielfeld.SteinIndexEnum, windsOneClick As Boolean) As Integer
    ' Für den Smoke-Test: Winds ggf. zusammenfassen
    Select Case s
        Case Spielfeld.SteinIndexEnum.WindOst, Spielfeld.SteinIndexEnum.WindSüd, Spielfeld.SteinIndexEnum.WindWest, Spielfeld.SteinIndexEnum.WindNord
            If windsOneClick Then Return 31 ' gleiche Gruppe (Ost) als Stellvertreter
    End Select
    Return CInt(s)
End Function

' --- Zufallszahl-Helfer (min inkl., max exkl.)
Friend Function GetZufallszahl(minInclusive As Integer, maxExclusive As Integer) As Integer
    Static rnd As New Random(123456)
    Return rnd.Next(minInclusive, maxExclusive)
End Function

#End If

