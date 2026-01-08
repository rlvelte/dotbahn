using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Models;
using DotBahn.Modules.Shared.Transformer;

namespace DotBahn.Clients.Timetables.Transformers;

/// <summary>
/// Transformer for converting <see cref="TimetableResponseContract"/> to <see cref="Models.Timetable"/>.
/// </summary>
/// <param name="stopTransformer">The transformer used to convert individual stops.</param>
public class TimetableTransformer(ITransformer<StopDataContract, TrainStop> stopTransformer) : ITransformer<TimetableResponseContract, Timetable> {
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
