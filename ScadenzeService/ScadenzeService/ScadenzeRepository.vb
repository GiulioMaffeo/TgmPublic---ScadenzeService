Imports System.Configuration
Imports System.Data.SQLite
Imports System.IO
Imports System.Reflection

Public Class ScadenzeRepository

    Private ReadOnly _connectionString As String

    Public Sub New()

        Dim exePath As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

        '----------------------------------------------------------
        ' configurazione Database
        '----------------------------------------------------------
        'Percorso DB
        Dim dbPathConf = ConfigurationManager.AppSettings("dbPath")
        Dim dbPath As String = Path.Combine(exePath, dbPathConf)

        If Not File.Exists(dbPath) Then
            LogService.Write($"ERRORE: DB NON TROVATO!{Environment.NewLine}")
            Throw New FileNotFoundException("Database non trovato", dbPath)
        End If

        _connectionString = $"Data Source={dbPath};Version=3;"

    End Sub

    Public Sub UpdateScadenza(s As Scadenza)

        Using conn As New SQLiteConnection(_connectionString)
            conn.Open()

            Dim cmd As New SQLiteCommand("
            UPDATE Scadenze
            SET NotificheInviate = @n,
                Stato = @stato,
                DataUltimaNotifica = @dataUltima
            WHERE Id = @id
            ", conn)

            cmd.Parameters.AddWithValue("@n", s.NotificheInviate)
            cmd.Parameters.AddWithValue("@stato", s.Stato)
            cmd.Parameters.AddWithValue("@id", s.Id)
            cmd.Parameters.AddWithValue("@dataUltima", ToDbString(s.DataUltimaNotifica))

            cmd.ExecuteNonQuery()
        End Using

    End Sub

    Public Function GetScadenzeAttive() As List(Of Scadenza)

        Dim result As New List(Of Scadenza)

        Using conn As New SQLiteConnection(_connectionString)
            conn.Open()

            Dim cmd As New SQLiteCommand("SELECT * FROM Scadenze WHERE Stato = 'Attiva'", conn)

            Using r = cmd.ExecuteReader()
                While r.Read()
                    result.Add(MapScadenza(r))
                End While
            End Using
        End Using

        Return result

    End Function

    Private Function MapScadenza(r As SQLiteDataReader) As Scadenza

        Dim s As New Scadenza()

        s.Id = Convert.ToInt32(r("Id"))
        s.Titolo = Convert.ToString(r("Titolo"))
        s.Descrizione = Convert.ToString(r("Descrizione"))
        s.NotificheInviare = If(IsDBNull(r("NotificheInviare")), 1, Convert.ToInt32(r("NotificheInviare")))
        s.NotificheInviate = If(IsDBNull(r("NotificheInviate")), 0, Convert.ToInt32(r("NotificheInviate")))
        s.Stato = If(IsDBNull(r("Stato")), "Attiva", Convert.ToString(r("Stato")))
        s.EmailDestinatario = Convert.ToString(r("EmailDestinatario"))
        s.GiorniAnticipo = If(IsDBNull(r("GiorniAnticipo")), 3, CInt(r("GiorniAnticipo")))

        Try
            s.DataScadenza = ParseDataDb(r("DataScadenza"))
            s.DataUltimaNotifica = If(IsDBNull(r("DataUltimaNotifica")), Nothing, ParseDataDb(r("DataUltimaNotifica")))
        Catch ex As Exception
            LogService.Write(ex.Source)
        End Try

        Return s

    End Function

End Class
