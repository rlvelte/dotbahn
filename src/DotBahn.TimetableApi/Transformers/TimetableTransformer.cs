using DotBahn.Parsing.Base;
using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Models;

namespace DotBahn.TimetableApi.Transformers;

/// <summary>
/// Transformer for converting <see cref="TimetableResponseContract"/> to <see cref="Models.Timetable"/>.
/// </summary>
public class TimetableTransformer(StopTransformer stopTransformer) : ITransformer<TimetableResponseContract, Timetable> {
    /// <inheritdoc />
    public Timetable Transform(TimetableResponseContract contract) {
        var stops = contract.Stops.Select(stop => {
                var transformedStop = stopTransformer.Transform(stop);
                return transformedStop with {
                    Station = transformedStop.Station with { Name = contract.Station }
                };
            }).ToList();

        return new Timetable {
            Station = contract.Station,
            Stops = stops,
            LastUpdated = DateTime.Now
        };
    }
}
