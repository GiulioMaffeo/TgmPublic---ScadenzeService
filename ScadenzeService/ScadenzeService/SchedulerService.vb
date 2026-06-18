Imports System.Configuration

Public Class SchedulerService

    Private ReadOnly _repo As ScadenzeRepository
    Private ReadOnly _email As EmailService

    Public Sub New(repo As ScadenzeRepository, email As EmailService)
        _repo = repo
        _email = email
    End Sub

    Public Sub Check()

        Dim ggScadenza As Integer = ConfigurationManager.AppSettings("ggScadenza")
        Dim scadenze = _repo.GetScadenzeAttive()

        For Each s In scadenze

            ggScadenza = s.GiorniAnticipo

            ' Se ha già raggiunto il limite, la saltiamo
            If s.NotificheInviate >= s.NotificheInviare Then
                Continue For
            End If

            '-------------------------------------------------------------------------------------
            ' Se la data di oggi è > della data di scadenza -giorni di anticipo notifica,
            ' ma minore della data di scadenza, allora invia notifica
            ' e la dataUltimaNotifica è minore di oggi (per evitare notifiche multiple nella stessa giornata)
            '-------------------------------------------------------------------------------------
            If DateTime.Now.Date >= s.DataScadenza.Value.Date.AddDays(-ggScadenza) AndAlso
               DateTime.Now.Date < s.DataScadenza.Value.Date AndAlso
               (s.DataUltimaNotifica Is Nothing OrElse DateTime.Now.Date > s.DataUltimaNotifica) Then

                LogService.Write($"Invio Notifica={s}{Environment.NewLine}")

                ' Invia email
                _email.InviaPromemoria(s)

                ' Aggiorna contatore
                s.NotificheInviate += 1
                s.DataUltimaNotifica = DateTime.Now.ToString("yyyy-MM-dd")


                ' Se ha raggiunto il limite, blocchiamo ulteriori notifiche
                If s.NotificheInviate >= s.NotificheInviare Then
                    s.Stato = "Notificata"
                End If
                ' Salva su DB
                _repo.UpdateScadenza(s)

            End If

        Next

    End Sub
End Class