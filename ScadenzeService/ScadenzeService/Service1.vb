Imports System.ServiceProcess
Imports System.Timers
Imports System.IO
Imports System.Reflection
Imports System.Configuration

Public Class ServiceScadenzario
    Inherits ServiceBase

    Private _timer As Timer
    Private _scheduler As SchedulerService

    Protected Overrides Sub OnStart(args As String())

        Try

            ' Log di avvio servizio
            LogService.Write($"[START]")

            '----------------------------------------------------------
            ' configurazione Database
            '----------------------------------------------------------

            Dim repo = New ScadenzeRepository()

            '----------------------------------------------------------
            ' configurazione servizio email
            '----------------------------------------------------------
            Dim email = New EmailService(
                host:="smtp.gmail.com",
                port:=587,
                user:="giulio.maffeo@gmail.com",
                pass:="hunb zwdm qdge qhhh",
                fromAddress:="giulio.maffeo@gmail.com"
            )



            '----------------------------------------------------------
            ' Scheduler del servizio
            '----------------------------------------------------------
            _scheduler = New SchedulerService(repo, email)

            ' Imposta timer
            Dim minuti As Integer = 60 ' default

            Try
                Dim raw = ConfigurationManager.AppSettings("TimerIntervalMinutes")

                If Not String.IsNullOrEmpty(raw) Then
                    minuti = Convert.ToInt32(raw)
                End If
            Catch
                ' Se c'è un errore, resta il default
            End Try

            _timer = New Timer()
            _timer.Interval = minuti * 60 * 1000
            LogService.Write($"Timer impostato a {minuti} minuti{Environment.NewLine}")

            AddHandler _timer.Elapsed, AddressOf TimerTick
            _timer.Start()

            LogService.Write("[START] Servizio avviato correttamente" & Environment.NewLine)

        Catch ex As Exception
            Dim exePath As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            Dim logPath As String = Path.Combine(exePath, "ScadenzarioService.log")
            LogService.Write($"[ERRORE OnStart] " & ex.ToString() & Environment.NewLine)
        End Try

    End Sub

    Private Sub TimerTick(sender As Object, e As ElapsedEventArgs)

        Try
            'LogService.Write($"[Timer Tick] " & Environment.NewLine)

            _scheduler.Check()

        Catch ex As Exception
            Dim exePath As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            Dim logPath As String = Path.Combine(exePath, "ScadenzarioService.log")
            LogService.Write($"[ERRORE OnStart] " & ex.ToString() & Environment.NewLine)

        End Try
    End Sub

    Protected Overrides Sub OnStop()

        Try
            _timer.Stop()

            Dim exePath As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            Dim logPath As String = Path.Combine(exePath, "ScadenzarioService.log")
            LogService.Write($"[STOP] Servizio arrestato" & Environment.NewLine)

        Catch ex As Exception
            Dim exePath As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            Dim logPath As String = Path.Combine(exePath, "ScadenzarioService.log")
            LogService.Write($"[ERRORE OnStop] " & ex.ToString() & Environment.NewLine)
        End Try
    End Sub

End Class