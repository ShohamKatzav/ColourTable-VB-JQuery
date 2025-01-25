Imports Microsoft.AspNetCore.Mvc
Imports ColoursTable.Services
Imports ColoursTable.Models

<ApiController>
<Route("api/[controller]")>
Public Class HelloWorldController
    Inherits ControllerBase

    Private ReadOnly _helloWorldService As HelloWorldService

    ' injection of the service layer
    Public Sub New(helloWorldService As HelloWorldService)
        _helloWorldService = helloWorldService
    End Sub
    
    <HttpGet>
    Public Function GetMessage() As ActionResult(Of MessageModel)
        Dim message = _helloWorldService.GetMessage()
        Return Ok(message)
    End Function
End Class