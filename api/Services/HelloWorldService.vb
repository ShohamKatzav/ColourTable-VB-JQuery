Imports ColoursTable.Models

Namespace Services
    Public Class HelloWorldService
        ' Business logic to get the message
        Public Function GetMessage() As MessageModel
            Dim response As New MessageModel With {
                .Message = "Hello from the service layer!"
            }
            Return response
        End Function
    End Class
End Namespace