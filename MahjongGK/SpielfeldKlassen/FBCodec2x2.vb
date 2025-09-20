Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Xml.Serialization

' ──────────────────────────────────────────────────────────────────────────────
' FBCodec2x2
' Serialisiert dein arrFB (Integer(,,)) nur über "Anker" je Stein:
' - Ankerzelle (oben-links) enthält (Index+1)*1000  (keine Flags)
' - Weitere Zellen im 2×2-Block:
'       rechts       : FLAG_XOffset
'       unten        : FLAG_YOffset
'       unten-rechts : FLAG_XOffset Or FLAG_YOffset
' - FLAG_ToggleFlag wird NIE gespeichert (Laufzeitflag).
'
' XML-Struktur:
' <FBAnchors X=".." Y=".." Z=".." AnchorBase="1000">
'   <Stones>
'     <S X=".." Y=".." Z=".." Id=".."/>
'     ...
'   </Stones>
' </FBAnchors>
' ──────────────────────────────────────────────────────────────────────────────
Public Module FBCodec2x2

    ' in Konstanten Deklariert
    ' '' ---- Bitflags ------------------------------------------------------------
    ''Public Const BIT0 As Integer = 1
    ''Public Const BIT1 As Integer = 2
    ''Public Const BIT2 As Integer = 4

    ''Public Const FLAG_XOffset As Integer = BIT0
    ''Public Const FLAG_YOffset As Integer = BIT1
    ''Public Const FLAG_ToggleFlag As Integer = BIT2

    ' Faktor für Anker-Kodierung
    Private Const THOUSAND As Integer = 1000

    ' ---- 2×2-Offsets relativ zum Anker (oben-links) --------------------------
    Private ReadOnly OFF_1 As (dx As Integer, dy As Integer, dz As Integer) = (+1, 0, 0) ' rechts
    Private ReadOnly OFF_2 As (dx As Integer, dy As Integer, dz As Integer) = (0, +1, 0) ' unten
    Private ReadOnly OFF_3 As (dx As Integer, dy As Integer, dz As Integer) = (+1, +1, 0) ' unten-rechts

    ' ──────────────────────────────────────────────────────────────────────────
    ' Serialisierbare Typen
    ' ──────────────────────────────────────────────────────────────────────────
    <XmlRoot("FBAnchors")>
    Public Class FBAnchors
        <XmlAttribute("X")> Public Property X As Integer
        <XmlAttribute("Y")> Public Property Y As Integer
        <XmlAttribute("Z")> Public Property Z As Integer

        ' Für Lesbarkeit/Debug: üblich = 1000
        <XmlAttribute("AnchorBase")> Public Property AnchorBase As Integer = THOUSAND

        <XmlArray("Stones"), XmlArrayItem("S")>
        Public Property Stones As List(Of FBAnchorItem)

        Public Sub New()
        End Sub

        Public Sub New(x As Integer, y As Integer, z As Integer, stones As List(Of FBAnchorItem))
            Me.X = x : Me.Y = y : Me.Z = z : Me.Stones = stones
        End Sub
    End Class

    Public Class FBAnchorItem
        <XmlAttribute("X")> Public Property X As Integer
        <XmlAttribute("Y")> Public Property Y As Integer
        <XmlAttribute("Z")> Public Property Z As Integer
        ' Achtung: Id = DEKODIERTER Index (ohne +1, ohne *1000)
        <XmlAttribute("Idx")> Public Property StoneId As Integer

        Public Sub New()
        End Sub

        Public Sub New(x As Integer, y As Integer, z As Integer, stoneId As Integer)
            Me.X = x : Me.Y = y : Me.Z = z : Me.StoneId = stoneId
        End Sub
    End Class

    ' ──────────────────────────────────────────────────────────────────────────
    ' 3D-Array -> Anchors
    ' - Liest NUR Ankerzellen (>= AnchorBase)
    ' - Dekodiert StoneId korrekt mit  (v \ 1000) - 1
    ' - Flags werden beim Export ignoriert; ToggleFlag wird damit implizit verworfen
    ' ──────────────────────────────────────────────────────────────────────────
    Public Function ToAnchors(arr As Integer(,,),
                              Optional anchorBase As Integer = THOUSAND) As FBAnchors
        If arr Is Nothing Then Return Nothing

        Dim maxX As Integer = arr.GetLength(0)
        Dim maxY As Integer = arr.GetLength(1)
        Dim maxZ As Integer = arr.GetLength(2)

        Dim list As New List(Of FBAnchorItem)(256)

        For x As Integer = 0 To maxX - 1
            For y As Integer = 0 To maxY - 1
                For z As Integer = 0 To maxZ - 1
                    Dim v As Integer = arr(x, y, z)
                    If v >= anchorBase Then
                        ' Anker: Index = (v \ 1000) - 1   (Flags sind bei Anker per Definition 0)
                        Dim stoneId As Integer = (v \ THOUSAND) - 1
                        list.Add(New FBAnchorItem(x, y, z, stoneId))
                    End If
                Next
            Next
        Next

        Return New FBAnchors(maxX, maxY, maxZ, list) With {.AnchorBase = anchorBase}
    End Function

    ' ──────────────────────────────────────────────────────────────────────────
    ' Anchors -> 3D-Array
    ' - Schreibt die Ankerzelle als (Id+1)*1000  (ohne Flags)
    ' - Setzt die drei Nachbarzellen mit FLAG_XOffset/FLAG_YOffset (ohne Toggle)
    ' - Optional strenge Bounds-/Konfliktprüfung
    ' ──────────────────────────────────────────────────────────────────────────
    Public Function FromAnchors(fb As FBAnchors,
                                Optional anchorBase As Integer = THOUSAND,
                                Optional validate As Boolean = True) As Integer(,,)
        If fb Is Nothing OrElse fb.X <= 0 OrElse fb.Y <= 0 OrElse fb.Z <= 0 Then Return Nothing

        Dim arr As Integer(,,) = New Integer(fb.X - 1, fb.Y - 1, fb.Z - 1) {}

        If fb.Stones Is Nothing OrElse fb.Stones.Count = 0 Then
            Return arr
        End If

        For Each s As FBAnchorItem In fb.Stones
            If validate Then
                ' Bounds Anker
                If s.X < 0 OrElse s.X >= fb.X OrElse
                   s.Y < 0 OrElse s.Y >= fb.Y OrElse
                   s.Z < 0 OrElse s.Z >= fb.Z Then
                    Throw New InvalidOperationException($"Anchor außerhalb: ({s.X},{s.Y},{s.Z}) bei Größe {fb.X}×{fb.Y}×{fb.Z}.")
                End If
                ' 2×2-Block passt?
                If s.X + 1 >= fb.X OrElse s.Y + 1 >= fb.Y Then
                    Throw New InvalidOperationException($"2×2-Block ragt raus: Anchor=({s.X},{s.Y},{s.Z}).")
                End If
                ' Keine Überschreibung
                CheckEmpty(arr, s.X, s.Y, s.Z)
                CheckEmpty(arr, s.X + OFF_1.dx, s.Y + OFF_1.dy, s.Z + OFF_1.dz)
                CheckEmpty(arr, s.X + OFF_2.dx, s.Y + OFF_2.dy, s.Z + OFF_2.dz)
                CheckEmpty(arr, s.X + OFF_3.dx, s.Y + OFF_3.dy, s.Z + OFF_3.dz)
            End If

            ' Ankerzelle schreiben: (Id+1)*1000 (Flags = 0)
            arr(s.X, s.Y, s.Z) = (s.StoneId + 1) * THOUSAND

            ' Nachbarn: nur X/Y-Flags persistieren (Toggle NIE speichern)
            arr(s.X + OFF_1.dx, s.Y + OFF_1.dy, s.Z + OFF_1.dz) = FLAG_XOffset
            arr(s.X + OFF_2.dx, s.Y + OFF_2.dy, s.Z + OFF_2.dz) = FLAG_YOffset
            arr(s.X + OFF_3.dx, s.Y + OFF_3.dy, s.Z + OFF_3.dz) = FLAG_XOffset Or FLAG_YOffset
        Next

        Return arr
    End Function

    ' ──────────────────────────────────────────────────────────────────────────
    ' Helpers
    ' ──────────────────────────────────────────────────────────────────────────
    Private Sub CheckEmpty(arr As Integer(,,), x As Integer, y As Integer, z As Integer)
        If arr(x, y, z) <> 0 Then
            Throw New InvalidOperationException($"Zellenkonflikt bei ({x},{y},{z}); vorhandener Wert={arr(x, y, z)}.")
        End If
    End Sub

End Module
