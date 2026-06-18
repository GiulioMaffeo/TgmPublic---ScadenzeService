Imports System.Globalization

Module DateHelper

    Private ReadOnly _formatoDb As String = "yyyy-MM-dd HH:mm:ss"
    Private ReadOnly _cultura As CultureInfo = CultureInfo.InvariantCulture

    Public Function ParseDataDb(dataString As String) As DateTime?

        If String.IsNullOrWhiteSpace(dataString) Then
            Return Nothing
        End If

        Dim risultato As DateTime
        If DateTime.TryParseExact(dataString,
                                  _formatoDb,
                                  _cultura,
                                  DateTimeStyles.None,
                                  risultato) Then
            Return risultato
        End If

        Throw New Exception("Formato data non riconosciuto: " & dataString)

    End Function

    ''' <summary>
    ''' Converte un DateTime nel formato standardizzato per il DB.
    ''' </summary>
    Public Function ToDbString(data As DateTime) As String
        Return data.ToString(_formatoDb, _cultura)
    End Function

End Module
