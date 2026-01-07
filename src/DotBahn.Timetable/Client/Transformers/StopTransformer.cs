using DotBahn.Core.Contracts;
using DotBahn.Core.Models;
using DotBahn.Parsing.Base;
using DotBahn.Timetable.Contracts;
using DotBahn.Timetable.Enumerations;
using DotBahn.Timetable.Models;

namespace DotBahn.Timetable.Client.Transformers;

/// <summary>
/// Transformer for converting <see cref="StopDataContract"/> to <see cref="TrainStop"/>.
/// </summary>
public class StopTransformer : ITransformer<StopDataContract, TrainStop>
{
    private readonly ITransformer<EventContract, EventInfo?> _eventTransformer;
    private readonly ITransformer<RawMessage, Message> _messageTransformer;

    private static readonly Dictionary<string, TripType> TripTypeMap = new()
    {
        { "p", TripType.Passenger },
        { "e", TripType.Empty },
        { "z", TripType.Freight }
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="StopTransformer"/> class.
    /// </summary>
    /// <param name="eventTransformer">The event transformer.</param>
    /// <param name="messageTransformer">The message transformer.</param>
    public StopTransformer(
        ITransformer<EventContract, EventInfo?> eventTransformer,
        ITransformer<RawMessage, Message> messageTransformer)
    {
        _eventTransformer = eventTransformer;
        _messageTransformer = messageTransformer;
    }

    /// <inheritdoc />
    public TrainStop Transform(StopDataContract raw)
    {
        var arrival = raw.Arrival != null ? _eventTransformer.Transform(raw.Arrival) : null;
        var departure = raw.Departure != null ? _eventTransformer.Transform(raw.Departure) : null;
        var messages = raw.Messages?.Select(_messageTransformer.Transform).ToList() 
                       ?? new List<Message>();

        return new TrainStop
        {
            Id = raw.Id,
            Station = new StationInfo
            {
                Eva = raw.Eva ?? string.Empty,
                // Name will be set by the calling service or parent transformer if available
            },
            Train = raw.TripLabel != null ? new TrainInfo
            {
                Category = raw.TripLabel.Category ?? string.Empty,
                Number = raw.TripLabel.Number ?? string.Empty,
                Type = ParseTripType(raw.TripLabel.TripType ?? "p"),
                Owner = raw.TripLabel.Owner,
                FilterFlags = raw.TripLabel.FilterFlags
            } : new TrainInfo(),
            Arrival = arrival,
            Departure = departure,
            Messages = messages
        };
    }

    private static TripType ParseTripType(string? type)
    {
        return type != null && TripTypeMap.TryGetValue(type, out var tripType) 
            ? tripType 
            : TripType.Passenger;
    }
}
