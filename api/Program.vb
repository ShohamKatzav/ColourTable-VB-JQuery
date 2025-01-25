Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Hosting
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
                                     End Sub)
    End Function
End Module