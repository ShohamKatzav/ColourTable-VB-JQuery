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
    Public  Function GetColours() As ActionResult(Of List(Of Colour))
        Dim colours = _colourService.GetColours()
        Return Ok(colours)
    End Function

    <HttpPost>
    Public Function AddColour(<FromBody> colour As Colour) As ActionResult(Of Colour)
        If colour Is Nothing Then
            Return BadRequest("Invalid colour data")
        End If
        Dim newColour = _colourService.AddColour(colour.ColourName, colour.Price, colour.ViewOrder, colour.Available)
        If newColour.Success Then
            Return Ok(newColour.Colour)
        Else
            Return BadRequest(newColour.Message)
        End If
    End Function

    <HttpDelete>
    Public Function DeleteColour(<FromBody> colourName As String) As IActionResult
        If colourName Is Nothing OrElse String.IsNullOrEmpty(colourName) Then
            Return BadRequest("Invalid colour name")
        End If
        Dim result As Boolean = _colourService.DeleteColour(ColourName)
        If result Then
            Return Ok($"Colour '{ColourName}' deleted successfully.")
        Else
            Return NotFound($"Colour '{ColourName}' not found.")
        End If
    End Function

    <HttpPut>
    Public Function UpdateColour(<FromBody> colour As ColourUpdateDTO) As ActionResult(Of Colour)
        If colour Is Nothing Then
            Return BadRequest("Invalid colour data")
        End If
        Dim result = _colourService.UpdateColour(colour.ColourName, colour.Price, colour.ViewOrder, colour.Available, colour.OldColourName)
        If result.Success Then
            Return Ok(result.Colour)
        Else
            Return BadRequest(result.Message)
        End If
    End Function

    <HttpPut("position")>
    Public Function UpdateColourPosition(<FromBody> colour As ColourPositionUpdateDTO) As IActionResult
        If colour Is Nothing Then
            Return BadRequest("Invalid colour data")
        End If
        Dim result As Boolean = _colourService.UpdateColourPosition(colour.ColourName, colour.ViewOrder)
        If result Then
            Return Ok($"Colour '{colour.ColourName}' position changed successfully.")
        Else
            Return NotFound($"Colour '{colour.ColourName}' not found.")
        End If
    End Function
End Class