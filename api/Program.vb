Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Configuration
Imports System.Data

Module Program
    Sub Main(args As String())
        CreateHostBuilder(args).Build().Run()
    End Sub

    Function CreateHostBuilder(args As String()) As IHostBuilder
        Return Host.CreateDefaultBuilder(args).
            ConfigureWebHostDefaults(Sub(webBuilder)
                                         webBuilder.UseStartup(Of Startup)() _
                                         .UseUrls("http://localhost:5000")
                                     End Sub).
            ConfigureAppConfiguration(Function(hostContext, config)
                                          ' Add appsettings.json and environment-specific JSON files
                                          config.AddJsonFile("appsettings.json", optional:=True, reloadOnChange:=True)
                                          config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional:=True)

                                          ' Add Azure environment variables
                                          config.AddEnvironmentVariables()
                                          Return config.Build()
                                      End Function)
    End Function
End Module