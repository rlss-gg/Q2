open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.DurableTask.Client

let (!) f = f |> ignore

HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(fun ctx services ->
        !services.AddHttpClient()
        !services.AddLogging()
        !services.AddDurableTaskClient(fun builder -> !builder.UseGrpc())
    )
    .Build()
    .Run()
