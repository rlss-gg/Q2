﻿open Microsoft.Azure.Cosmos
open Microsoft.Extensions.Configuration
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
        !services.AddSingleton<CosmosClient>(fun _ -> new CosmosClient(ctx.Configuration.GetValue<string>("CosmosDb")))
        !services.AddTransient<IPersistence, Persistence>()
    )
    .Build()
    .Run()
