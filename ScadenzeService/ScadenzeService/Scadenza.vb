Public Class Scadenza

    Public Property Id As Integer
    Public Property Titolo As String
    Public Property Descrizione As String
    Public Property DataScadenza As DateTime?          ' Data della scadenza 
    Public Property DataUltimaNotifica As DateTime?     ' Data dell'ultima notifica inviata
    Public Property NotificheInviare As Integer         ' max notifiche
    Public Property NotificheInviate As Integer         ' quante già inviate
    Public Property Stato As String                     ' Attiva / Notificata / etc.
    Public Property EmailDestinatario As String         ' Email a cui inviare il promemoria

    Public Property GiorniAnticipo As Integer    ' Giorni di anticipo per l'invio del promemoria

End Class


