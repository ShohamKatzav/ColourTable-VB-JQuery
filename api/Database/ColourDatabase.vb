Imports System
Imports System.Configuration
Imports Microsoft.Data.SqlClient
Imports ColoursTable.Models
Imports ColoursTable.DTO
Imports Microsoft.Extensions.Configuration

Namespace DataAccess
    Public Class ColourDatabase

        Private ReadOnly _configuration As IConfiguration
        Private connectionString As String
        Private providerName As String

        Public Sub New(configuration As IConfiguration)

            _configuration = configuration

            connectionString = _configuration.GetConnectionString("DbConnection")

            If String.IsNullOrWhiteSpace(connectionString) Then
                Throw New InvalidOperationException("Connection string 'DbConnection' not configured.")
            End If

        End Sub

        Public Async Function GetColoursAsync() As Task(Of List(Of Colour))
            ' Provide the query string with a parameter placeholder.
            Dim queryString As String = _
                "SELECT * FROM dbo.Colours " & _
                "ORDER BY ViewOrder"
            Dim colors As New List(Of Colour)()

            Using connection As New SqlConnection(connectionString)

                Dim command As New SqlCommand(queryString, connection)

                Try
                    Await connection.OpenAsync()
                    Dim dataReader As SqlDataReader = _
                    Await command.ExecuteReaderAsync()
                    While Await dataReader.ReadAsync()
                        Dim color As New Colour() With {
                            .ColourName = dataReader("ColourName").ToString(),
                            .Price = Convert.ToInt32(dataReader("Price")),
                            .ViewOrder = Convert.ToInt32(dataReader("ViewOrder")),
                            .Available = Convert.ToBoolean(dataReader("Available"))
                        }
                        colors.add(color)
                    End While
                    dataReader.Close()

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
                Return colors
            End Using
        End Function

        Public Async Function GetColourCountAsync(ColourName As String, ViewOrder As Integer, Optional OldColourName As String = "") _
         As Task(Of DuplicateColourCheckResult)

            Dim nameCount As Integer = 0
            Dim viewOrderCount As Integer = 0   
            Dim nameQueryString As String = _
                "SELECT COUNT(ColourName) FROM dbo.Colours " & _
                "WHERE ColourName = @ColourName"
            Dim viewOrderQueryString As String = ""
            If Not(String.IsNullOrWhiteSpace(OldColourName)) Then
                viewOrderQueryString = _
                "SELECT COUNT(ColourName) FROM dbo.Colours " & _
                "WHERE ColourName <> @OldColourName AND " & _
                "ViewOrder = @ViewOrder"
            Else
                viewOrderQueryString = _
                "SELECT COUNT(ColourName) FROM dbo.Colours " & _
                "WHERE ViewOrder = @ViewOrder"
            End If

            Using connection As New SqlConnection(connectionString)

                Dim nameCommand As New SqlCommand(nameQueryString, connection)
                Dim viewOrderCommand As New SqlCommand(viewOrderQueryString, connection)
                nameCommand.Parameters.AddWithValue("@ColourName", ColourName)
                viewOrderCommand.Parameters.AddWithValue("@OldColourName", OldColourName)
                viewOrderCommand.Parameters.AddWithValue("@ViewOrder", ViewOrder)

                Try
                    Await connection.OpenAsync()
                    nameCount = Await nameCommand.ExecuteScalarAsync()
                    viewOrderCount = Await viewOrderCommand.ExecuteScalarAsync()
                    
                    Return New DuplicateColourCheckResult() With {
                        .DuplicateName = nameCount,
                        .DuplicateViewOrder = viewOrderCount
                    }

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    Return Nothing
                End Try
            End Using
        End Function

        Public Async Function AddColourAsync(ColourName As String, Price As Integer,
                                    ViewOrder As Integer, Available As Boolean) As Task(Of Colour)

            Dim queryString As String = 
                "INSERT INTO dbo.Colours (ColourName, Price, ViewOrder, Available) " &
                "VALUES (@ColourName, @Price, @ViewOrder,@Available)"
            

            Using connection As New SqlConnection(connectionString)

                Dim command As New SqlCommand(queryString, connection)
                command.Parameters.AddWithValue("@ColourName", ColourName)
                command.Parameters.AddWithValue("@Price", Price)
                command.Parameters.AddWithValue("@ViewOrder", ViewOrder)
                command.Parameters.AddWithValue("@Available", Available)

                Try
                    Await connection.OpenAsync()
                    Await command.ExecuteScalarAsync()

                    Return New Colour() With {
                        .ColourName = ColourName,
                        .Price = Price,
                        .ViewOrder = ViewOrder,
                        .Available = Available
                    }

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    Return Nothing
                End Try
            End Using
        End Function

        Public Async Function DeleteColourAsync(ColourName As String) As Task(Of Boolean)

            Dim queryString As String = 
                "DELETE FROM dbo.Colours " &
                "WHERE ColourName = @ColourName"

            Using connection As New SqlConnection(connectionString)

                Dim command As New SqlCommand(queryString, connection)
                command.Parameters.AddWithValue("@ColourName", ColourName)

                Try
                    Await connection.OpenAsync()
                    Dim rowsAffected As Integer = Await command.ExecuteNonQueryAsync()
                    Return rowsAffected > 0

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    Return False
                End Try
            End Using
        End Function

        Public Async Function UpdateColourAsync(ColourName As String, Price As Integer,
                                    ViewOrder As Integer, Available As Boolean, OldColourName As String) As Task(Of Colour)

            Dim queryString As String =
            "UPDATE dbo.Colours " &
            "SET ColourName = @ColourName, " &
            "Price = @Price, " &
            "ViewOrder = @ViewOrder, " &
            "Available = @Available " &
            "WHERE ColourName = @OldColourName"

            Using connection As New SqlConnection(connectionString)

                Dim command As New SqlCommand(queryString, connection)
                command.Parameters.AddWithValue("@ColourName", ColourName)
                command.Parameters.AddWithValue("@Price", Price)
                command.Parameters.AddWithValue("@ViewOrder", ViewOrder)
                command.Parameters.AddWithValue("@Available", Available)
                command.Parameters.AddWithValue("@OldColourName", OldColourName)

                Try
                    Await connection.OpenAsync()
                    Await command.ExecuteNonQueryAsync()

                    Return New Colour() With {
                        .ColourName = ColourName,
                        .Price = Price,
                        .ViewOrder = ViewOrder,
                        .Available = Available
                    }

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    Return Nothing
                End Try
            End Using
        End Function

        Public Async Function UpdateColourPositionAsync(ColourName As String, ViewOrder As Integer) As Task(Of Colour)

            Dim queryString As String =
                "UPDATE dbo.Colours SET ViewOrder = @ViewOrder WHERE ColourName = @ColourName"

            Using connection As New SqlConnection(connectionString)
                Dim command As New SqlCommand(queryString, connection)
                command.Parameters.AddWithValue("@ColourName", ColourName)
                command.Parameters.AddWithValue("@ViewOrder", ViewOrder)

                Try
                    Await connection.OpenAsync()
                        Dim rowsAffected As Integer = Await command.ExecuteNonQueryAsync()
                        Return New Colour() With {
                            .ColourName = ColourName,
                            .ViewOrder = ViewOrder
                        }

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    Return Nothing
                End Try
            End Using
        End Function
    End Class
End Namespace