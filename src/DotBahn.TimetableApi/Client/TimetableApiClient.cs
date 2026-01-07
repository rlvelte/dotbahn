using DotBahn.Core.Client;
using DotBahn.Core.Token.Base;
using DotBahn.Parsing;
using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Transformers;

namespace DotBahn.TimetableApi.Client;

/// <summary>
/// API client for fetching timetable information.
/// </summary>
public class TimetableApiClient(BaseDotBahnConfiguration configuration, ITokenService tokenService, HttpClient httpClient) 
    : BaseDotBahnClient(configuration, tokenService, httpClient) {
    
    private readonly XmlParser<TimetableContract> _parser = new();
    private readonly TimetableTransformer _transformer = new(
        new StopTransformer(new EventTransformer(), new MessageTransformer())
    );

    /// <summary>
    /// Gets full changes for a specific station.
    /// </summary>
    /// <param name="eva">The EVA station number.</param>
    /// <returns>A <see cref="Models.Timetable"/> with current information.</returns>
    public async Task<Models.Timetable> GetFullChangesAsync(string eva) {
        var rawXml = await GetRawDataAsync($"/fchg/{eva}");
        var contract = _parser.Parse(rawXml);
        return _transformer.Transform(contract);
    }
}
