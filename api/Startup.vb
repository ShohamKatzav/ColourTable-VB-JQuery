Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports ColoursTable.Services
Imports ColoursTable.DataAccess
Imports System.Configuration

Public Class Startup

    Private FrontendUri As String

    Public Sub ConfigureServices(services As IServiceCollection)

        Dim frontendUri As String = ConfigurationManager.AppSettings("FrontendUri")
        If String.IsNullOrEmpty(frontendUri) Then
            Throw New Exception("FrontendUri is not configured in Web.config")
        End If

        services.AddScoped(Of ColourDatabase)()
        services.AddScoped(Of ColourService)()

        services.AddControllers()
        services.AddCors(Sub(options)
            options.AddPolicy("AllowSpecificOrigin",
                Sub(builder)
                    builder.WithOrigins(frontendUri) _
                            .AllowAnyHeader() _
                            .AllowAnyMethod() _
                            .AllowCredentials()
                End Sub)
            End Sub)
    End Sub

    Public Sub Configure(app As IApplicationBuilder, env As IWebHostEnvironment)
        If env.IsDevelopment() Then
            app.UseDeveloperExceptionPage()
        End If

        app.UseRouting()

        app.UseCors("AllowSpecificOrigin")

        app.UseHttpsRedirection()
        app.UseAuthorization()

        app.UseEndpoints(Sub(endpoints)
                             endpoints.MapControllers()
                         End Sub)
    End Sub
End Class