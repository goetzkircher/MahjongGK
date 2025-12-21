
Namespace Spielfeld

    Public Module SpielfeldDatenModul

        'Einzige Aufgabe dieses Moduls ist es, eine einzige Instanz der SpielfeldDatenClass
        'zu halten, die von überall im Namespace Spielfeld aus erreichbar ist.
        'Von außerhalb des Namespace Spielfeld ist sie über Spielfeld.SFD erreichbar.
        'Die Kapselung der Daten - alle Daten sind nur über das Präfix SFD. erreichbar -
        'gibt die Sicherheit, versehentliche Seiteneffekte zu vermeiden, die schwer zu finden sind.

        Public SFD As New SpielfeldDatenClass()

    End Module
End Namespace