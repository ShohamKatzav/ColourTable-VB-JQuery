Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Hosting
Imports System.Data

Module Program
    Sub Main(args As String())
        CreateHostBuilder(args).Build().Run()
    End Sub

    Function CreateHostBuilder(args As String()) As IHostBuilder
        Return Host.CreateDefaultBuilder(args).
            ConfigureAppConfiguration(Sub(context, config)
                                          config.AddJsonFile("appsettings.json", optional:=True, reloadOnChange:=True)
                                          config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional:=True)
                                          config.AddEnvironmentVariables()
                                      End Sub).
            ConfigureWebHostDefaults(Sub(webBuilder)
                                         webBuilder.UseStartup(Of Startup)() _
                                         .UseUrls("http://localhost:5000")
                                     End Sub)
    End Function
End Module