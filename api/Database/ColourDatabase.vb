Imports System
Imports System.Configuration
Imports Microsoft.Data.SqlClient
Imports ColoursTable.Models
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

        Public Function GetColours() As List(Of Colour)
            ' Provide the query string with a parameter placeholder.
            Dim queryString As String = _
                "SELECT * FROM dbo.Colours " & _
                "ORDER BY ViewOrder"
            Dim colors As New List(Of Colour)()

            Using connection As New SqlConnection(connectionString)

                Dim command As New SqlCommand(queryString, connection)

                Try
                    connection.Open()
                    Dim dataReader As SqlDataReader = _
                    command.ExecuteReader()
                    While dataReader.Read()
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

        Public Function GetColourCount(ColourName As String) As Integer


            Dim count As Integer = 0   
            Dim queryString As String = _
                "SELECT COUNT(ColourName) FROM dbo.Colours " & _
                "WHERE ColourName = @ColourName"

            Using connection As New SqlConnection(connectionString)

                Dim command As New SqlCommand(queryString, connection)
                command.Parameters.AddWithValue("@ColourName", ColourName)

                Try
                    connection.Open()
                    count = command.ExecuteScalar()
                    
                    Return count

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            End Using
        End Function

        Public Function AddColour(ColourName As String, Price As Integer,
                                    ViewOrder As Integer, Available As Boolean) As Colour

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
                    connection.Open()
                    command.ExecuteScalar()

                    Return New Colour() With {
                        .ColourName = ColourName,
                        .Price = Price,
                        .ViewOrder = ViewOrder,
                        .Available = Available
                    }

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            End Using
        End Function

        Public Function DeleteColour(ColourName As String) As Boolean

            Dim queryString As String = 
                "DELETE FROM dbo.Colours " &
                "WHERE ColourName = @ColourName"

            Using connection As New SqlConnection(connectionString)

                Dim command As New SqlCommand(queryString, connection)
                command.Parameters.AddWithValue("@ColourName", ColourName)

                Try
                    connection.Open()
                    Dim rowsAffected As Integer = command.ExecuteNonQuery()
                    Return rowsAffected > 0

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    Return False
                End Try
            End Using
        End Function

        Public Function UpdateColour(ColourName As String, Price As Integer,
                                    ViewOrder As Integer, Available As Boolean, OldColourName As String) As Colour

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
                    connection.Open()
                    command.ExecuteNonQuery()

                    Return New Colour() With {
                        .ColourName = ColourName,
                        .Price = Price,
                        .ViewOrder = ViewOrder,
                        .Available = Available
                    }

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            End Using
        End Function

        Public Function UpdateColourPosition(ColourName As String, ViewOrder As Integer) As Boolean

            Dim queryString As String =
            "UPDATE dbo.Colours " &
            "SET ViewOrder = @ViewOrder " &
            "WHERE ColourName = @ColourName"

            Using connection As New SqlConnection(connectionString)

                Dim command As New SqlCommand(queryString, connection)
                command.Parameters.AddWithValue("@ColourName", ColourName)
                command.Parameters.AddWithValue("@ViewOrder", ViewOrder)

                Try
                    connection.Open()
                    Dim rowsAffected As Integer = command.ExecuteNonQuery()
                    Return rowsAffected > 0

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            End Using
        End Function

    End Class
End Namespace