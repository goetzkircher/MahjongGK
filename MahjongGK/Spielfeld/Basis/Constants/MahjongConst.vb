'
'##############################################################################
'Gespeichert als ...\SFKlassen\SFConstants
'##############################################################################
'
''' <summary>
''' Pfad: MahjongGK/Spielfeld/Basis
''' </summary>
Public Module MahjongConst

    Public Const MJ_STEININDEX_MAX As Integer = 42

    'Die minimal mögliche Große von frmMain wird hierau bezogen.
    Public Const MJ_SPIELFELD_MIN_WIDTH As Integer = 600
    Public Const MJ_SPIELFELD_MIN_HEIGHT As Integer = 400

    'Das ist die Mindestfeldgröße, die für die Funktionen
    'Vorlagen_Pyramide und Vorlagen_Spielfeldaufteilung benötigt werden. 
    Public Const MJ_STEINE_MINX As Integer = 1
    Public Const MJ_STEINE_MINY As Integer = 1
    Public Const MJ_STEINE_MINZ As Integer = 1

    'Nach meinen Recherchen nach haben Mahjong-Layouts haben typischerweise
    'max. 15 bis 20 Steine nebeneinander und 8–12 Steine übereinander.
    '
    'Die hier angegebenen Grenzen stellen sicher, daß das Feld noch angezeigt
    'werden kann. Ob das sinnvoll ist, ist eine andere Frage. 
    'Und ob der Rechner ein derart großes und vollgepflastertes Feld noch
    'verdauen kann, ist wieder eine andere Frage.

    'Steine nebeneinander
    Public Const MJ_STEINE_MAXX As Integer = 60
    Public Const MJ_STEINE_MAXY As Integer = 20
    Public Const MJ_STEINE_MAXZ As Integer = 10

    ''' <summary>
    ''' Theoretischer Wert zur Begrenzung der maximal erzeugten Steine.
    ''' </summary>
    Public Const MJ_STEINE_MAXCOUNT As Integer = MJ_STEINE_MAXX * MJ_STEINE_MAXY * MJ_STEINE_MAXZ
    Public Const MJ_STEINE_VORRATMAXDEFAULT As Integer = 500
    Public Const MJ_STEINE_VORRATNACHSCHUBSCHWELLEDEFAULT As Integer = 250

    ''ergibt bei 30 Steinen nebeneinander 15 Steine übereinander
    'Public Const MJ_STEINE_OVERANOTHER_MAX As Integer =
    '            (MJ_STEINE_SIDEBYSIDE_MAX * MJ_GRAFIK_ORG_WIDTH * MJ_SPIELFELD_MIN_HEIGHT) \
    '            (MJ_GRAFIK_ORG_HEIGHT * MJ_SPIELFELD_MIN_WIDTH)
    'Hinweis zu obiger Formel: Es wird hier mit reinen Integer-Operationen gearbeitet.
    'Hier kann es ganz schnell passieren, daß ein Zwischenergebnis klein wird und das
    'Endergebnis durch die Rundung des Zwischenergenisses zerschossen, oder erschossen (=0)
    'wird. Daher zuerst alles multiplizieren und dann erst dividieren. 

    Public Const MJ_STEINE_SUMMESTEINE042_ALLE As Integer = 42
    Public Const MJ_STEINE_SUMMESTEINE042_NORMAL As Integer = 34
    Public Const MJ_STEINE_SUMMESTEINE042_SONDER As Integer = 8

    Public Const MJ_STEINE_SUMMESTEINE144_ALLE As Integer = 4 * MJ_STEINE_SUMMESTEINE042_NORMAL + MJ_STEINE_SUMMESTEINE042_SONDER
    Public Const MJ_STEINE_SUMMESTEINE144_NORMAL As Integer = 4 * MJ_STEINE_SUMMESTEINE042_NORMAL
    Public Const MJ_STEINE_SUMMESTEINE144_SONDER As Integer = MJ_STEINE_SUMMESTEINE042_SONDER

    Public Const MJ_STEINE_SUMMESTEINE152_ALLE As Integer = 4 * MJ_STEINE_SUMMESTEINE042_NORMAL + 2 * MJ_STEINE_SUMMESTEINE042_SONDER
    Public Const MJ_STEINE_SUMMESTEINE152_NORMAL As Integer = 4 * MJ_STEINE_SUMMESTEINE042_NORMAL
    Public Const MJ_STEINE_SUMMESTEINE152_SONDER As Integer = 2 * MJ_STEINE_SUMMESTEINE042_SONDER

    Public Const MJ_STEIN_VERTEILUNG_FAKTOR_144_NORMAL As Single = MJ_STEINE_SUMMESTEINE144_NORMAL / MJ_STEINE_SUMMESTEINE144_ALLE * 1 / MJ_STEINE_SUMMESTEINE042_NORMAL
    Public Const MJ_STEIN_VERTEILUNG_FAKTOR_144_SONDER As Single = MJ_STEINE_SUMMESTEINE144_SONDER / MJ_STEINE_SUMMESTEINE144_ALLE * 1 / MJ_STEINE_SUMMESTEINE042_SONDER
    Public Const MJ_STEIN_VERTEILUNG_FAKTOR_152_NORMAL As Single = MJ_STEINE_SUMMESTEINE152_NORMAL / MJ_STEINE_SUMMESTEINE152_ALLE * 1 / MJ_STEINE_SUMMESTEINE042_NORMAL
    Public Const MJ_STEIN_VERTEILUNG_FAKTOR_152_SONDER As Single = MJ_STEINE_SUMMESTEINE152_SONDER / MJ_STEINE_SUMMESTEINE152_ALLE * 1 / MJ_STEINE_SUMMESTEINE042_SONDER

    '
    '
    '
    Public Const MJ_GRAFIK_SRC_MAX_WIDTH_OR_HEIGHT As Integer = 600

    'Das sind "Notbremswerte", für den Spielbetrieb unbrauchbar klein.
    Public Const MJ_GRAFIK_SRC_MIN_WIDTH_OR_HEIGHT As Integer = 6
    Public Const MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT As Integer = 6

    'Public Const MJ_GRAFIK_FAKTOR_H_TO_W As Double = MJ_GRAFIK_ORG_WIDTH / MJ_GRAFIK_ORG_HEIGHT
    'Public Const MJ_GRAFIK_FAKTOR_W_TO_H As Double = MJ_GRAFIK_ORG_HEIGHT / MJ_GRAFIK_ORG_WIDTH

    Public Const MJ_MARGIN_ABSOLUT_LEFT As Integer = 10 'geradzahlige Werte nehmen
    Public Const MJ_MARGIN_ABSOLUT_TOP As Integer = 10  'für alle 4
    Public Const MJ_MARGIN_ABSOLUT_RIGHT As Integer = 10
    Public Const MJ_MARGIN_ABSOLUT_BOTTOM As Integer = 10

    'Die Hälfte. Wird benötigt um den Rahmen (falls er gezeichnet wird)
    'um das Spielfeld mittig auf den Rand zu zeichnen
    Public Const MJ_MARGIN_ABSOLUT_LEFT_HALF As Single = MJ_MARGIN_ABSOLUT_LEFT / 2
    Public Const MJ_MARGIN_ABSOLUT_TOP_HALF As Single = MJ_MARGIN_ABSOLUT_TOP / 2
    Public Const MJ_MARGIN_ABSOLUT_RIGHT_HALF As Single = MJ_MARGIN_ABSOLUT_RIGHT / 2
    Public Const MJ_MARGIN_ABSOLUT_BOTTOM_HALF As Single = MJ_MARGIN_ABSOLUT_BOTTOM / 2

    Public Const MJ_COLOR_BG_DEFAULT As Integer = &HFFD0D0D0 'helleres Grau

    ''Das ist die Verschiebung je Ebene der Steine bei maximaler Steingröße
    'Public Const MJ_OFFSET3D_MAX_LEFT As Double = 3
    'Public Const MJ_OFFSET3D_MAX_TOP As Double = 3
    'Public Const MJ_OFFSET3D_MIN_LEFT As Double = 1
    'Public Const MJ_OFFSET3D_MIN_TOP As Double = 1

    'Public Const MJ_OFFSET3D_PADDING_LEFTRIGHT As Integer = 10
    'Public Const MJ_OFFSET3D_PADDING_TOPBOTTOM As Integer = 10

    'Public Const MJ_OFFSET3DFAKTOR_MAX_LEFT As Double = MJ_OFFSET3D_MAX_LEFT / MJ_GRAFIK_ORG_WIDTH
    'Public Const MJ_OFFSET3DFAKTOR_MAX_TOP As Double = MJ_OFFSET3D_MAX_TOP / MJ_GRAFIK_ORG_HEIGHT

End Module
