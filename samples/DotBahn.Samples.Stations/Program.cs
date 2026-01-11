using DotBahn.Clients.Shared.Options;
using DotBahn.Clients.Stations;
using DotBahn.Clients.Stations.Client;
using DotBahn.Clients.Stations.Models;
using DotBahn.Modules.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

if (args.Length < 2) {
    Console.WriteLine("Usage: DotBahn.Sample.Stations <ClientId> <ClientSecret>");
    return;
}

var clientId = args[0];
var clientSecret = args[1];

var services = new ServiceCollection();

// Add Logging
services.AddLogging(builder => {
    builder.SetMinimumLevel(LogLevel.Debug);
});

// Add Authorization
services.AddDotBahnAuthorization(opt => {
    opt.ClientId = clientId;
    opt.ApiKey = clientSecret;
});

// Add Stations Client
services.AddDotBahnStations(opt => {
    opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/station-data/v2/");
});

// Usage
var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<StationsClient>();

var response = await client.GetStationsAsync(new StationsQuery {
    Categories = "1-2",
    Names = ["hamburg"]
});
foreach (var s in response.Stations) {
    Console.WriteLine($"""
                      {s.Name.ToUpper()} (ID: {s.Number} | EVA: {s.EvaNumbers.First().Number} | RIL100: {s.Ril100Identifiers.First().RilIdentifier} | CAT: {s.Category})
                      ==========================================================
                      Region: {s.RegionalArea.Name} (ID: {s.RegionalArea.Number})
                      Address: {s.MailingAddress.Street}
                               {s.MailingAddress.ZipCode} {s.MailingAddress.City}
                      
                      """);
}