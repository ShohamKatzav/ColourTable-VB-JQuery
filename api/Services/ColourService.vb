Imports ColoursTable.DataAccess
Imports ColoursTable.Models
Imports ColoursTable.DTO

Namespace Services
    Public Class ColourService
        Private _colourDatabase As ColourDatabase

        Public Sub New(colourDatabase As ColourDatabase)
            _colourDatabase = colourDatabase
        End Sub

        Public Function GetColours() As List(Of Colour)
            Try
                Dim colours = _colourDatabase.GetColours()
                Return colours
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Throw
            End Try
        End Function

        Public Function AddColour(ColourName As String, Price As Integer,
                                    ViewOrder As Integer, Available As Boolean) As OperationResult
            Try
                Dim Exist As DuplicateColourCheckResult = _colourDatabase.GetColourCount(ColourName, ViewOrder)
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
                Dim colour = _colourDatabase.AddColour(ColourName, Price, ViewOrder, Available)
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

        Public Function DeleteColour(ColourName As String) As Boolean
            Try
                Return _colourDatabase.DeleteColour(ColourName)
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return False
                Throw
            End Try
        End Function

        Public Function UpdateColour(ColourName As String, Price As Integer,
                                    ViewOrder As Integer, Available As Boolean, OldColourName As String) As OperationResult
            Try
                Dim Exist As DuplicateColourCheckResult = _colourDatabase.GetColourCount(OldColourName, ViewOrder)
                Dim Duplicate As DuplicateColourCheckResult = _colourDatabase.GetColourCount(ColourName, ViewOrder, OldColourName)
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
                Dim updatedColour = _colourDatabase.UpdateColour(ColourName, Price,
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

        Public Function UpdateColourPosition(ColourName As String, ViewOrder As Integer) As OperationResult
            Try
                Dim updatedColour = _colourDatabase.UpdateColourPosition(ColourName, ViewOrder)
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