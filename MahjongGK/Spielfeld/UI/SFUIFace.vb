
Namespace Spielfeld

    '''<summary>
    '''Pfad: MahjongGK/Spielfeld/UI
    '''
    ''' Hält globale UI-, Bedien- und Sichtbarkeitszustände des Spielfeldsystems. 
    ''' 
    ''' SFUI bündelt alle nicht fachlichen Zustände, die zur Benutzeroberfläche 
    ''' und zur Interaktion mit dem Spielfeldbereich gehören. Dazu zählen insbesondere: 
    ''' 
    ''' - welches UserControl aktuell sichtbar oder aktiv ist, 
    ''' - Sichtbarkeitszustände von Toolbox oder Zusatzbereichen, 
    ''' - UI-bezogene Statuswechsel, 
    ''' - Formular- und Control-Referenzen, 
    ''' - Scrollbar-Renderer oder vergleichbare UI-Hilfsobjekte, 
    ''' - weitere globale Zustände der Bedienoberfläche. 
    ''' 
    ''' SFUI enthält keine Spielfeldlogik und keine eigentlichen Spielfelddaten. 
    ''' Zweck der Klasse ist, die Benutzeroberfläche klar vom Fachmodell,
    ''' vom Layout und vom Renderkern zu trennen. 
    ''' 
    ''' Alles, was der Benutzeroberfläche dient, aber nicht das Spielfeld selbst beschreibt, 
    ''' ist typischerweise ein Kandidat für SFUI. 
    ''' </summary>
    Public Class SFUIFace

        Public Sub New()

        End Sub

        Public Sub New(owner As SFDaten)
            _sfd = owner
        End Sub

        Private ReadOnly _sfd As SFDaten

    End Class
End Namespace
