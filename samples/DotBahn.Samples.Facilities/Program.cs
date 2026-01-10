using DotBahn.Clients.Facilities;
using DotBahn.Clients.Facilities.Client;
using DotBahn.Clients.Facilities.Enumerations;
using DotBahn.Clients.Facilities.Models;
using DotBahn.Modules.Authorization;
using DotBahn.Modules.Cache;
using Microsoft.Extensions.DependencyInjection;

if (args.Length < 2) {
    Console.WriteLine("Usage: DotBahn.Sample.Facilities <ClientId> <ClientSecret>");
    return;
}

var clientId = args[0];
var clientSecret = args[1];

var services = new ServiceCollection();
services.AddLogging();

// Add Authorization
services.AddAuthorizationProvider((_, opt) => {
    opt.ClientId = clientId;
    opt.ClientSecret = clientSecret;
});

// Add Cache
services.AddCacheProvider((_, opt) => {
    opt.DefaultExpiration = TimeSpan.FromSeconds(30); 
});

// Add Facilities Client
services.AddDotBahnFacilities((_, opt) => {
    opt.BaseEndpoint = new Uri("https://apis.deutschebahn.com/db-api-marketplace/apis/fasta/v2/");
});

var serviceProvider = services.BuildServiceProvider();


// Use the API
var client = serviceProvider.GetRequiredService<FacilitiesClient>();

var facilities = await client.GetFacilitiesAsync(new FacilitiesQuery().WithType(FacilityType.Elevator));
Console.WriteLine($"Found {facilities.Count} facilities.");
        
foreach (var f in facilities.Take(10)) {
    Console.WriteLine($"- {f.Description}");
}
