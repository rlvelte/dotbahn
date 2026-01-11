using DotBahn.Clients.Facilities;
using DotBahn.Clients.Facilities.Client;
using DotBahn.Clients.Facilities.Enumerations;
using DotBahn.Clients.Facilities.Models;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

if (args.Length < 2) {
    Console.WriteLine("Usage: DotBahn.Sample.Facilities <ClientId> <ClientSecret>");
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

// Add Cache
services.AddDotBahnCache(opt => {
    opt.DefaultExpiration = TimeSpan.FromSeconds(30); 
});

// Add Facilities Client
services.AddDotBahnFacilities(opt => {
    opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/");
});

// Usage
var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<FacilitiesClient>();

var response = await client.GetFacilitiesAsync(new FacilitiesQuery().WithType(FacilityType.Elevator).AtStation(1));
foreach (var f in response) {
    Console.WriteLine($"""
                       {f.Type} (ID: {f.StationNumber}/{f.EquipmentNumber})
                       ==========================================================
                       GPS: {f.Latitude}/{f.Longitude}
                       State: {f.State} ({f.StateExplanation ?? "No explanation"})
                       Description: {f.Description}
                                
                       """);
}