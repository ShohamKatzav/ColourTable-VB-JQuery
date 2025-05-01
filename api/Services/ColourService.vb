Imports ColoursTable.DataAccess
Imports ColoursTable.Models
Imports ColoursTable.DTO

Namespace Services
    Public Class ColourService
        Private _colourDatabase As ColourDatabase

        Public Sub New(colourDatabase As ColourDatabase)
            _colourDatabase = colourDatabase
        End Sub

        Public Async Function GetColours() As Task(Of List(Of Colour))
            Try
                Dim colours = Await _colourDatabase.GetColoursAsync()
                Return colours
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Throw
            End Try
        End Function

        Public Async Function AddColour(ColourName As String, Price As Integer,
                                    ViewOrder As Integer, Available As Boolean) As Task(Of OperationResult)
            Try
                Dim Exist As DuplicateColourCheckResult = Await _colourDatabase.GetColourCountAsync(ColourName, ViewOrder)
                If Exist.DuplicateName <> 0 Then
                    Return New OperationResult With {
                        .Success = False,
                        .Message = "Colour name already added"
                    }
                End If
                If Exist.DuplicateViewOrder <> 0 Then
                    Return New OperationResult With {
                        .Success = False,
                        .Message = "Colour with same view order already exists"
                    }
                End If
                If Not (TypeOf ColourName Is String) OrElse ColourName Is Nothing OrElse String.IsNullOrEmpty(ColourName) OrElse _
                        Price.GetType() IsNot GetType(Integer) OrElse Price < 0 OrElse _
                        ViewOrder.GetType() IsNot GetType(Integer) OrElse ViewOrder < 0 OrElse _
                        Available.GetType() IsNot GetType(Boolean) Then
                        Return New OperationResult With {
                            .Success = False,
                            .Message = "Invalid Input"
                        }
                End If
                Dim colour = Await _colourDatabase.AddColourAsync(ColourName, Price, ViewOrder, Available)
                Return New OperationResult With {
                    .Success = True,
                    .Message = "Successfully added",
                    .Colour = colour
                }
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Throw
            End Try
        End Function

        Public Async Function DeleteColour(ColourName As String) As Task(Of Boolean)
            Try
                Return Await _colourDatabase.DeleteColourAsync(ColourName)
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return False
                Throw
            End Try
        End Function

        Public Async Function UpdateColour(ColourName As String, Price As Integer,
                                    ViewOrder As Integer, Available As Boolean, OldColourName As String) As Task(Of OperationResult)
            Try
                Dim Exist As DuplicateColourCheckResult = Await _colourDatabase.GetColourCountAsync(OldColourName, ViewOrder)
                Dim Duplicate As DuplicateColourCheckResult = Await _colourDatabase.GetColourCountAsync(ColourName, ViewOrder, OldColourName)
                If Duplicate.DuplicateName > 0 AndAlso OldColourName <> ColourName Then
                    Return New OperationResult With {
                        .Success = False,
                        .Message = "Could not update. Colour with this name is already exist"
                    }
                End If
                If Duplicate.DuplicateViewOrder > 0 Then
                    Return New OperationResult With {
                        .Success = False,
                        .Message = "Could not update. Colour with the same view order exist"
                    }
                End If
                If Not (TypeOf ColourName Is String) OrElse ColourName Is Nothing OrElse String.IsNullOrEmpty(ColourName) OrElse _
                        Price.GetType() IsNot GetType(Integer) OrElse Price < 0 OrElse _
                        ViewOrder.GetType() IsNot GetType(Integer) OrElse ViewOrder < 0 OrElse _
                        Available.GetType() IsNot GetType(Boolean) Then
                        Return New OperationResult With {
                            .Success = False,
                            .Message = "Invalid Input"
                        }
                End If
                If Exist.DuplicateName = 0 Then
                    Return New OperationResult With {
                        .Success = False,
                        .Message = "Colour Isn't Exist"
                    }
                End If
                Dim updatedColour = Await _colourDatabase.UpdateColourAsync(ColourName, Price,
                                                    ViewOrder, Available, OldColourName)
                Return New OperationResult With {
                    .Success = True,
                    .Message = "Successfully updated colour",
                    .Colour = updatedColour
                }
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Throw
            End Try
        End Function

        Public Async Function UpdateColourPosition(ColourName As String, ViewOrder As Integer) As  Task(Of OperationResult)
            Try
                Dim updatedColour = Await _colourDatabase.UpdateColourPositionAsync(ColourName, ViewOrder)
                Return New OperationResult With {
                    .Success = True,
                    .Message = "Successfully updated position colour",
                    .Colour = updatedColour
                }
            Catch ex As Exception
                Return New OperationResult With {
                        .Success = False,
                        .Message = "Failed to update position"
                }
                Throw
            End Try
        End Function

    End Class
End Namespace