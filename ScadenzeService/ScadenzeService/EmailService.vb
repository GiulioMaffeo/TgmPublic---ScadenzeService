Imports System.Net
Imports System.Net.Mail

Public Class EmailService
    Private ReadOnly _smtp As SmtpClient
    Private ReadOnly _from As String

    Public Sub New(host As String, port As Integer, user As String, pass As String, fromAddress As String)
        _from = fromAddress
        _smtp = New SmtpClient(host, port) With {
            .Credentials = New NetworkCredential(user, pass),
            .EnableSsl = True
        }
    End Sub

    Public Sub InviaPromemoria(s As Scadenza)

        Dim mail As New MailMessage()
        mail.From = New MailAddress(_from)
        mail.To.Add(s.EmailDestinatario)
        mail.Subject = "Promemoria scadenza: " & s.Titolo
        mail.Body = $"Ciao, ti ricordo che il  {s.DataScadenza} scade: {s.Titolo}{vbCrLf}{s.Descrizione}"

        _smtp.Send(mail)
        LogService.Write($"[Notifica inviata] {mail.To} {mail.Subject}" & Environment.NewLine)

    End Sub
End Class
