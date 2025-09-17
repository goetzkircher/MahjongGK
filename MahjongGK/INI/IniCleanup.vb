Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Reflection
Imports System.Text.RegularExpressions

' ──────────────────────────────────────────────────────────────────────────────
'  Entfernt verwaiste INI-Keys (inkl. direkt davorstehenden Kommentarblock)
'  basierend auf den aktuell gültigen Properties (Regel: "Sektion_Key").
'  Indexierte Properties (Enum) werden zu Key-Kombinationen expandiert.
'  Enum-Properties bekommen zusätzlich Key+"_Enum" als gültigen Key.
' ──────────────────────────────────────────────────────────────────────────────
Public Module IniCleanup

    ''' <summary>
    ''' Löscht verwaiste INI-Einträge in allen bekannten INIs (AllIniManagersIniLines),
    ''' inklusive des lückenlos direkt darüber stehenden Kommentarblocks ('; ...').
    ''' Rückgabewert: Anzahl gelöschter Key-Zeilen (Kommentare nicht mitgezählt).
    ''' </summary>
    Public Function RemoveObsoleteIniKeys(Optional alsoSave As Boolean = True) As Integer

        If AllIniManagersIniLines Is Nothing OrElse AllIniManagersIniLines.Length = 0 Then
            Return 0
        End If

        ' 1) Gültige Keys aus Reflection gemäß deiner bestehenden Regeln aufbauen
        Dim valid As HashSet(Of (Section As String, Key As String)) = BuildValidSectionKeySet()


        ' 2) Pro Datei säubern
        Dim removedTotal As Integer = 0
        Dim sectionRx As New Regex("^\s*\[(.+?)\]\s*$")
        Dim keyRx As New Regex("^\s*([^=\s]+)\s*=")

        For i As Integer = 0 To AllIniManagersIniLines.Length - 1
            Dim src As List(Of String) = AllIniManagersIniLines(i)
            If src Is Nothing OrElse src.Count = 0 Then
                Continue For
            End If

            Dim cleaned As New List(Of String)(src.Count)
            Dim currentSection As String = String.Empty
            Dim removedHere As Integer = 0

            Dim idx As Integer = 0
            While idx < src.Count
                Dim line As String = src(idx)

                ' Abschnitt?
                Dim mSec As Match = sectionRx.Match(line)
                If mSec.Success Then
                    currentSection = mSec.Groups(1).Value.Trim()
                    cleaned.Add(line)
                    idx += 1
                    Continue While
                End If

                ' Key?
                Dim mKey As Match = keyRx.Match(line)
                If Not mKey.Success Then
                    cleaned.Add(line)
                    idx += 1
                    Continue While
                End If

                Dim key As String = mKey.Groups(1).Value.Trim()

                If valid.Contains((currentSection, key)) Then
                    cleaned.Add(line)
                Else
                    ' Kommentarblock direkt über der Key-Zeile mit entfernen (nur ;*-Zeilen)
                    Dim j As Integer = cleaned.Count - 1
                    While j >= 0
                        Dim prev As String = cleaned(j)
                        If IsCommentLine(prev) Then
                            cleaned.RemoveAt(j)
                            j -= 1
                            Continue While
                        End If
                        Exit While
                    End While
                    ' (Key-Zeile selbst wird NICHT hinzugefügt → entfernt)
                    removedHere += 1
                End If

                idx += 1
            End While

            ' Ergebnis zurückschreiben
            src.Clear()
            src.AddRange(cleaned)
            removedTotal += removedHere

            ' optional speichern
            If alsoSave AndAlso i <= AllIniManagers.Count - 1 Then
                Try
                    AllIniManagers(i).Save(alwaysSave:=True)
                Catch
                    ' bewusst still: Säuberung ist im Speicher erfolgt; Save folgt ggf. später erneut
                End Try
            End If
        Next

        Return removedTotal
    End Function

    ' ──────────────────────────────────────────────────────────────────────────
    ' Gültige Keys ermitteln (gemäß deinen vorhandenen Reflection-Helfern)
    ' ──────────────────────────────────────────────────────────────────────────

    Private Function BuildValidSectionKeySet() As HashSet(Of (Section As String, Key As String))
        Dim setCmp As IEqualityComparer(Of (String, String)) = New SecKeyComparer()
        Dim hs As New HashSet(Of (Section As String, Key As String))(setCmp)

        ' 1) Einfache INI-Properties (Sektion_Key)
        For Each p As PropertyInfo In GetIniPropertiesWithUnderscore()
            Dim section As String = String.Empty, keyBase As String = String.Empty
            SplitSectionAndKeyFromPropertyName(p.Name, section, keyBase)

            ' Basis-Key
            hs.Add((section, keyBase))

            ' Enum-Properties: zusätzlich *_Enum zulassen
            Dim t As Type = p.PropertyType
            Dim isNullable As Boolean = t.IsGenericType AndAlso t.GetGenericTypeDefinition() Is GetType(Nullable(Of ))
            Dim nn As Type = If(isNullable, Nullable.GetUnderlyingType(t), t)
            If nn IsNot Nothing AndAlso nn.IsEnum Then
                hs.Add((section, keyBase & "_Enum"))
            End If
        Next

        ' 2) Indexierte INI-Properties mit Enum-Index(en)
        For Each p As PropertyInfo In GetIniIndexedPropertiesWithEnumIndex()
            Dim section As String = String.Empty, keyBase As String = String.Empty
            SplitSectionAndKeyFromPropertyName(p.Name, section, keyBase)

            Dim idxTypes As Type() = p.GetIndexParameters().Select(Function(pi) pi.ParameterType).ToArray()
            For Each args As Object() In EnumerateIndexArgTuples(idxTypes)
                ' Schlüsselbildung: KeyBase _ <EnumName> [_ <EnumName2> ...]
                Dim suffix As String = String.Join("_", args.Select(Function(a) a.ToString()))
                hs.Add((section, $"{keyBase}_{suffix}"))
            Next
        Next

        Return hs
    End Function

    ''' <summary>
    ''' Zerlegt "Sektion_Key..." am ERSTEN Unterstrich:
    ''' "Rendering_EmptyMessageFont" → Sektion="Rendering", Key="EmptyMessageFont".
    ''' </summary>
    Private Sub SplitSectionAndKeyFromPropertyName(propName As String, ByRef section As String, ByRef key As String)
        Dim pos As Integer = propName.IndexOf("_"c)
        If pos <= 0 OrElse pos >= propName.Length - 1 Then
            section = String.Empty
            key = propName
            Return
        End If
        section = propName.Substring(0, pos)
        key = propName.Substring(pos + 1)
    End Sub

    ' ──────────────────────────────────────────────────────────────────────────
    ' Utilities
    ' ──────────────────────────────────────────────────────────────────────────

    Private NotInheritable Class SecKeyComparer
        Implements IEqualityComparer(Of (Section As String, Key As String))

        Public Overloads Function Equals(x As (Section As String, Key As String), y As (Section As String, Key As String)) As Boolean _
            Implements IEqualityComparer(Of (Section As String, Key As String)).Equals
            Return x.Section.Equals(y.Section, StringComparison.OrdinalIgnoreCase) AndAlso
                   y.Key.Equals(x.Key, StringComparison.OrdinalIgnoreCase)
        End Function

        Public Overloads Function GetHashCode(obj As (Section As String, Key As String)) As Integer _
            Implements IEqualityComparer(Of (Section As String, Key As String)).GetHashCode
            Return (obj.Section.ToUpperInvariant() & "§" & obj.Key.ToUpperInvariant()).GetHashCode()
        End Function
    End Class

    Private Function IsCommentLine(line As String) As Boolean
        If String.IsNullOrWhiteSpace(line) Then Return False
        Dim t As String = line.TrimStart()
        Return t.StartsWith(";", StringComparison.Ordinal)
    End Function

End Module

