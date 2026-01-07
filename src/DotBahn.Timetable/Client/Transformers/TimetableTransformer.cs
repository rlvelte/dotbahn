using DotBahn.Parsing.Base;
using DotBahn.Timetable.Contracts;
using DotBahn.Timetable.Models;

namespace DotBahn.Timetable.Client.Transformers;

/// <summary>
/// Transformer for converting <see cref="TimetableContract"/> to <see cref="Models.Timetable"/>.
/// </summary>
public class TimetableTransformer : ITransformer<TimetableContract, Models.Timetable>
{
    private readonly StopTransformer _stopTransformer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimetableTransformer"/> class.
    /// </summary>
    /// <param name="stopTransformer">The stop transformer.</param>
    public TimetableTransformer(StopTransformer stopTransformer)
    {
        _stopTransformer = stopTransformer;
    }

    /// <inheritdoc />
    public Models.Timetable Transform(TimetableContract raw)
    {
        var stops = raw.Stops
            .Select(stop => {
                var transformedStop = _stopTransformer.Transform(stop);
                return transformedStop with {
                    Station = transformedStop.Station with { Name = raw.Station }
                };
            })
            .ToList();

        return new Models.Timetable
        {
            Station = raw.Station,
            Stops = stops,
            LastUpdated = DateTime.Now
        };
    }
}
