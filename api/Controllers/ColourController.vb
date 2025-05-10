Imports Microsoft.AspNetCore.Mvc
Imports ColoursTable.Services
Imports ColoursTable.Models
Imports ColoursTable.DTO

<ApiController>
<Route("api/[controller]")>
Public Class ColourController
    Inherits ControllerBase

    Private ReadOnly _colourService As ColourService

    ' injection of the service layer
    Public Sub New(colourService As ColourService)
        _colourService = colourService
    End Sub
    
    <HttpGet>
    Public Async Function GetColours() As Task(Of ActionResult(Of List(Of Colour)))
        Try
            Dim colours = Await _colourService.GetColours()
            If colours Is Nothing OrElse colours.Count = 0 Then
                Return NotFound("No colours found")
            End If
            Return Ok(colours)
        Catch ex As Exception
        ' If something failed (cold start, DB down), return 503
            Return StatusCode(503, "Service unavailable or still warming up")
    End Try
    End Function

    <HttpPost>
    Public Async Function AddColour(<FromBody> colour As Colour) As Task(Of ActionResult(Of Colour))
        If colour Is Nothing Then
            Return BadRequest("Invalid colour data")
        End If
        Dim newColour = Await _colourService.AddColour(colour.ColourName, colour.Price, colour.ViewOrder, colour.Available)
        If newColour.Success Then
            Return Ok(newColour.Colour)
        Else
            Return BadRequest(newColour.Message)
        End If
    End Function

    <HttpDelete>
    Public Async Function DeleteColour(<FromBody> colourName As String) As Task(Of ActionResult)
        If colourName Is Nothing OrElse String.IsNullOrEmpty(colourName) Then
            Return BadRequest("Invalid colour name")
        End If
        Dim result As Boolean = Await _colourService.DeleteColour(ColourName)
        If result Then
            Return Ok($"Colour '{ColourName}' deleted successfully.")
        Else
            Return NotFound($"Colour '{ColourName}' not found.")
        End If
    End Function

    <HttpPut>
    Public Async Function UpdateColour(<FromBody> colour As ColourUpdateDTO) As Task(Of ActionResult)
        If colour Is Nothing OrElse String.IsNullOrEmpty(colour.ColourName) OrElse String.IsNullOrEmpty(colour.OldColourName) Then
            Return BadRequest("Invalid colour data")
        End If
        Dim result = Await _colourService.UpdateColour(colour.ColourName, colour.Price, colour.ViewOrder, colour.Available, colour.OldColourName)
        If result.Success Then
            Return Ok(result.Colour)
        Else
            Return BadRequest(result.Message)
        End If
    End Function

    <HttpPut("position")>
    Public Async Function UpdateColourPosition(<FromBody> colour As ColourPositionUpdateDTO) As Task(Of IActionResult)
        If colour Is Nothing Then
            Return BadRequest("Invalid colour data")
        End If
        Dim result = Await _colourService.UpdateColourPosition(colour.ColourName, colour.ViewOrder)
        If result.Success Then
            Return Ok(result.Colour)
        Else
            Return NotFound($"Colour '{colour.ColourName}' not found.")
        End If
    End Function
End Class