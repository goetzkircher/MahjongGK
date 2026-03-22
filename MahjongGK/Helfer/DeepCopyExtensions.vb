Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization

Namespace Helfer

    ' --- Attribute, um einzelne Members zu steuern ---
    <AttributeUsage(AttributeTargets.Field Or AttributeTargets.Property)>
    Public NotInheritable Class DeepCopyIgnoreAttribute
        Inherits Attribute
    End Class

    <AttributeUsage(AttributeTargets.Field Or AttributeTargets.Property)>
    Public NotInheritable Class DeepCopyShallowAttribute
        Inherits Attribute
    End Class

    ' --- Referenz-Gleichheit für Zyklus-Map ---
    Friend NotInheritable Class ReferenceEqualityComparer
        Implements IEqualityComparer(Of Object)

        Public Shared ReadOnly Instance As New ReferenceEqualityComparer()

        Public Overloads Function Equals(x As Object, y As Object) As Boolean _
          Implements IEqualityComparer(Of Object).Equals
            Return Object.ReferenceEquals(x, y)
        End Function

        Public Overloads Function GetHashCode(obj As Object) As Integer _
          Implements IEqualityComparer(Of Object).GetHashCode
            Return RuntimeHelpers.GetHashCode(obj)
        End Function
    End Class
    '
    ''' <summary>
    ''' Stellt Erweiterungsmethoden zur Verfügung, um beliebige Objekte tief zu kopieren.
    ''' Unterstützt Werttypen, Strings, Enums, Arrays (einschließlich mehrdimensionaler Arrays),
    ''' generische Collections wie List(Of T) und Dictionary(Of K,V) sowie eigene Klassen.
    ''' </summary>
    ''' <remarks>
    ''' Die Implementierung arbeitet reflexionsbasiert:
    ''' <list type="bullet">
    '''   <item><description>Wenn eine Klasse selbst eine DeepCopy()-Methode bereitstellt, wird diese bevorzugt.</description></item>
    '''   <item><description>ICloneable wird unterstützt (z. B. bei System.Drawing.Font).</description></item>
    '''   <item><description>Für Arrays und Collections werden neue Instanzen angelegt und deren Inhalte rekursiv kopiert.</description></item>
    '''   <item><description>Zyklische Referenzen werden erkannt und korrekt aufgelöst.</description></item>
    '''   <item><description>Über Attribute &lt;DeepCopyIgnore&gt; und &lt;DeepCopyShallow&gt; kann das Verhalten pro Member gesteuert werden.</description></item>
    ''' </list>
    ''' Ziel ist Robustheit und Einfachheit beim Umschalten zwischen Bearbeitungs- und Laufzeitkontexten,
    ''' nicht maximale Geschwindigkeit. Für Performance-kritische Klassen können eigene DeepCopy-Methoden
    ''' implementiert werden.
    ''' </remarks>
    ''' <example>
    ''' Beispiel zur Verwendung:
    ''' <code language="vbnet">
    ''' Dim f As New Spielfeld With {
    '''     .SteinInfos = New List(Of SteinInfo) From {
    '''         New SteinInfo With {.Id = 1, .Name = "A"},
    '''         New SteinInfo With {.Id = 2, .Name = "B"}
    '''     },
    '''     .arrFB = New Integer(1, 1, 1) {}
    ''' }
    ''' f.arrFB(1, 1, 1) = 42
    ''' 
    ''' ' Tiefe Kopie erstellen
    ''' Dim copy As Spielfeld = f.DeepCopy()
    ''' 
    ''' ' Änderungen in der Kopie haben keine Wirkung auf das Original
    ''' copy.SteinInfos(0).Name = "X"
    ''' copy.arrFB(1, 1, 1) = 99
    ''' 
    ''' Debug.Assert(f.SteinInfos(0).Name = "A")
    ''' Debug.Assert(f.arrFB(1, 1, 1) = 42)
    ''' </code>
    ''' </example>

    Public Module DeepCopyExtensions

        ' ----- Öffentliche API -----
        <Extension>
        Public Function DeepCopy(Of T)(ByVal source As T) As T
            Dim visited As IDictionary(Of Object, Object) =
              New Dictionary(Of Object, Object)(ReferenceEqualityComparer.Instance)
            Dim result As Object = DeepCopyInternal(TryCast(DirectCast(source, Object), Object), visited)
            Return DirectCast(result, T)
        End Function

        ' ----- Kern-Logik -----
        Private Function DeepCopyInternal(obj As Object,
                                          visited As IDictionary(Of Object, Object)) As Object
            If obj Is Nothing Then Return Nothing

            Dim t As Type = obj.GetType()

            ' 1) Triviale/immutable Typen direkt zurückgeben
            If IsPrimitiveLike(t) Then Return obj

            ' 2) Schon gesehen? => Zyklus abbrechen
            Dim existing As Object = Nothing
            If visited.TryGetValue(obj, existing) Then
                Return existing
            End If

            ' 3) Falls eigenes DeepCopy() vorhanden -> bevorzugen
            Dim mDC As MethodInfo = t.GetMethod("DeepCopy", BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic, Nothing, Type.EmptyTypes, Nothing)
            If mDC IsNot Nothing AndAlso mDC.ReturnType Is t Then
                Dim resOwn As Object = mDC.Invoke(obj, Nothing)
                visited(obj) = resOwn
                Return resOwn
            End If

            ' 4) Falls ICloneable -> Clone()
            Dim clonable As ICloneable = TryCast(obj, ICloneable)
            If clonable IsNot Nothing Then
                Dim cloned As Object = clonable.Clone()
                visited(obj) = cloned
                Return cloned
            End If

            ' 5) Arrays separat behandeln
            If t.IsArray Then
                Dim arr As Array = DirectCast(obj, Array)
                Dim elemType As Type = t.GetElementType()
                Dim rank As Integer = arr.Rank

                ' Jagged vs. Multi-Dim wird über Array-API gleichermaßen unterstützt
                Dim lengths(rank - 1) As Integer
                Dim lowers(rank - 1) As Integer
                For i As Integer = 0 To rank - 1
                    lengths(i) = arr.GetLength(i)
                    lowers(i) = arr.GetLowerBound(i)
                Next

                Dim copy As Array = Array.CreateInstance(elemType, lengths, lowers)
                visited(obj) = copy

                If IsPrimitiveLike(elemType) Then
                    ' Für reine Wert-Elemente kann man blockweise kopieren (schnell)
                    Array.Copy(arr, copy, arr.Length)
                Else
                    ' Elementweise deepcopieren
                    Dim indices(rank - 1) As Integer
                    CopyArrayRecursively(arr, copy, 0, indices, visited)
                End If

                Return copy
            End If

            ' 6) Collections: List(Of T)
            If IsGenericList(t) Then
                Dim copy As IList = DirectCast(Activator.CreateInstance(t), IList)
                visited(obj) = copy
                Dim src As IList = DirectCast(obj, IList)
                Dim elemType As Type = t.GetGenericArguments()(0)

                For Each item As Object In src
                    Dim newItem As Object =
                      If(IsPrimitiveLike(elemType), item, DeepCopyInternal(item, visited))
                    copy.Add(newItem)
                Next
                Return copy
            End If

            ' 7) Collections: Dictionary(Of K, V)
            If IsGenericDictionary(t) Then
                Dim copy As IDictionary = DirectCast(Activator.CreateInstance(t), IDictionary)
                visited(obj) = copy
                Dim src As IDictionary = DirectCast(obj, IDictionary)

                Dim args As Type() = t.GetGenericArguments()
                Dim kType As Type = args(0)
                Dim vType As Type = args(1)

                For Each de As DictionaryEntry In src
                    Dim newKey As Object =
                      If(IsPrimitiveLike(kType), de.Key, DeepCopyInternal(de.Key, visited))
                    Dim newVal As Object =
                      If(IsPrimitiveLike(vType), de.Value, DeepCopyInternal(de.Value, visited))
                    copy.Add(newKey, newVal)
                Next
                Return copy
            End If

            ' 8) Beliebige Klassen/Structs: über Felder/Properties
            Dim created As Object = CreateUninitializedOrDefault(t)
            visited(obj) = created

            ' 8a) Felder kopieren
            Dim fields As FieldInfo() =
              t.GetFields(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
            For Each f As FieldInfo In fields
                ' readonly Felder (InitOnly) überspringen, um Exceptions zu vermeiden
                If f.IsInitOnly Then Continue For

                ' Attribute beachten
                If HasAttribute(Of DeepCopyIgnoreAttribute)(f) Then Continue For

                Dim srcVal As Object = f.GetValue(obj)
                Dim tgtVal As Object

                If HasAttribute(Of DeepCopyShallowAttribute)(f) Then
                    tgtVal = srcVal
                Else
                    Dim ft As Type = f.FieldType
                    tgtVal = If(IsPrimitiveLike(ft), srcVal, DeepCopyInternal(srcVal, visited))
                End If

                f.SetValue(created, tgtVal)
            Next

            ' 8b) Setzbare Properties (ohne Indexer) kopieren – optional/nützlich
            Dim props As PropertyInfo() =
              t.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
            For Each p As PropertyInfo In props
                If Not p.CanRead OrElse Not p.CanWrite Then Continue For
                If p.GetIndexParameters().Length <> 0 Then Continue For
                If HasAttribute(Of DeepCopyIgnoreAttribute)(p) Then Continue For

                Try
                    Dim srcVal As Object = p.GetValue(obj, Nothing)
                    Dim tgtVal As Object

                    If HasAttribute(Of DeepCopyShallowAttribute)(p) Then
                        tgtVal = srcVal
                    Else
                        Dim pt As Type = p.PropertyType
                        tgtVal = If(IsPrimitiveLike(pt), srcVal, DeepCopyInternal(srcVal, visited))
                    End If

                    p.SetValue(created, tgtVal, Nothing)
                Catch
                    ' Manche Properties haben interne Guards oder werfen – dann einfach auslassen
                End Try
            Next

            Return created
        End Function

        ' ----- Helpers -----

        Private Function IsPrimitiveLike(t As Type) As Boolean
            If t.IsPrimitive OrElse t.IsEnum Then Return True

            ' Häufige "Wert-ähnliche" Typen (immutable oder Strukturen)
            If t Is GetType(String) Then Return True
            If t Is GetType(Decimal) Then Return True
            If t Is GetType(DateTime) OrElse t Is GetType(DateTimeOffset) Then Return True
            If t Is GetType(TimeSpan) Then Return True
            If t Is GetType(Guid) Then Return True

            ' Deine typischen Structs (ValueTypes): Color, Point, Size, Rectangle etc. sind ValueTypes
            If t.IsValueType Then Return True ' deckt Color, Point, Size, Rectangle, ... ab

            Return False
        End Function

        Private Function IsGenericList(t As Type) As Boolean
            Return t.IsGenericType AndAlso GetType(IList).IsAssignableFrom(t)
        End Function

        Private Function IsGenericDictionary(t As Type) As Boolean
            Return t.IsGenericType AndAlso GetType(IDictionary).IsAssignableFrom(t)
        End Function

        Private Sub CopyArrayRecursively(src As Array,
                                         dst As Array,
                                         dimension As Integer,
                                         indices As Integer(),
                                         visited As IDictionary(Of Object, Object))
            Dim lower As Integer = src.GetLowerBound(dimension)
            Dim upper As Integer = src.GetUpperBound(dimension)

            For i As Integer = lower To upper
                indices(dimension) = i
                If dimension < src.Rank - 1 Then
                    CopyArrayRecursively(src, dst, dimension + 1, indices, visited)
                Else
                    Dim val As Object = src.GetValue(indices)
                    Dim elemType As Type = src.GetType().GetElementType()
                    Dim newVal As Object =
                      If(IsPrimitiveLike(elemType), val, DeepCopyInternal(val, visited))
                    dst.SetValue(newVal, indices)
                End If
            Next
        End Sub

        Private Function CreateUninitializedOrDefault(t As Type) As Object
            ' Versuche zuerst einen parameterlosen Ctor (häufig unproblematisch)
            Try
                Dim ci As ConstructorInfo = t.GetConstructor(Type.EmptyTypes)
                If ci IsNot Nothing Then
                    Return Activator.CreateInstance(t)
                End If
            Catch
                ' weiter unten fallback
            End Try

            ' Fallback: uninitialisiert (ohne Ctor) – vorsichtig, aber robust zum Füllen via Felder
            Try
                Return FormatterServices.GetUninitializedObject(t)
            Catch
                ' Letzter Fallback: Nothing (sollte kaum passieren)
                Return Nothing
            End Try
        End Function

        Private Function HasAttribute(Of TAttr As Attribute)(fi As FieldInfo) As Boolean
            Return fi.GetCustomAttributes(GetType(TAttr), inherit:=True).Any()
        End Function

        Private Function HasAttribute(Of TAttr As Attribute)(pi As PropertyInfo) As Boolean
            Return pi.GetCustomAttributes(GetType(TAttr), inherit:=True).Any()
        End Function

    End Module

End Namespace

