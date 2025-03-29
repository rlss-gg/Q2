open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Q2.AppHost
open Q2.Application

let (!) f = f |> ignore

HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(fun ctx services ->
        !services.AddHttpClient()
        !services.AddLogging()
        !services.AddSingleton<IPersistence, Persistence>()
    )
    .Build()
    .Run()
