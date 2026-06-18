Imports System.IO
Imports System.Reflection

Public Module LogService

    '    Private ReadOnly logFile As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "service.log")

    Dim exePath As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
    Dim logPath As String = Path.Combine(exePath, "ScadenzarioService.log")


    Public Sub Write(message As String)
        Try
            Dim line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}"
            File.AppendAllText(logPath, line & Environment.NewLine)
        Catch
            ' Se il log fallisce, non blocchiamo l'app
        End Try
    End Sub

End Module
