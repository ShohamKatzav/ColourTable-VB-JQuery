
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports ColoursTable.Services
Imports ColoursTable.DataAccess
Imports System.Configuration
Imports Microsoft.Extensions.Configuration

Public Class Startup
    Private ReadOnly _configuration As IConfiguration

    Public Sub New(configuration As IConfiguration)
        _configuration = configuration
    End Sub

    Public Sub ConfigureServices(services As IServiceCollection)
        Dim frontendUri = _configuration("FrontendUri")
        Console.WriteLine(frontendUri)

        If String.IsNullOrWhiteSpace(frontendUri) Then
            Throw New InvalidOperationException("FrontendUri is not configured.")
        End If

        services.AddScoped(Of ColourDatabase)(Function(provider) 
                                              Return New ColourDatabase(_configuration)
                                          End Function)
        services.AddScoped(Of ColourService)()
        services.AddScoped(Of HelloWorldService)()

        services.AddCors(Sub(options)
            options.AddPolicy("AllowSpecificOrigin",
                Sub(builder)
                    builder.WithOrigins(frontendUri) _
                           .AllowAnyHeader() _
                           .AllowAnyMethod() _
                           .AllowCredentials()
                End Sub)
        End Sub)

        services.AddControllers()
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