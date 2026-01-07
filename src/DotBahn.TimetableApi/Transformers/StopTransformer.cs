using DotBahn.Core.Contracts;
using DotBahn.Core.Models;
using DotBahn.Parsing.Base;
using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Enumerations;
using DotBahn.TimetableApi.Models;

namespace DotBahn.TimetableApi.Transformers;

/// <summary>
/// Transformer for converting <see cref="StopDataContract"/> to <see cref="TrainStop"/>.
/// </summary>
public class StopTransformer(ITransformer<EventContract, EventInfo?> eventTransformer, ITransformer<RawMessage, Message> messageTransformer) : ITransformer<StopDataContract, TrainStop> {
    private static readonly Dictionary<string, TripType> TripTypeMap = new() {
        { "p", TripType.Passenger },
        { "e", TripType.Empty },
        { "z", TripType.Freight }
    };

    /// <inheritdoc />
    public TrainStop Transform(StopDataContract contract) {
        var arrival = contract.Arrival != null ? eventTransformer.Transform(contract.Arrival) : null;
        var departure = contract.Departure != null ? eventTransformer.Transform(contract.Departure) : null;
        var messages = contract.Messages?.Select(messageTransformer.Transform).ToList() ?? [];

        return new TrainStop {
            Id = contract.Id,
            Station = new StationInfo {
                Eva = contract.Eva,
                // Name will be set by the calling service or parent transformer if available
            },
            Train = contract.TripLabel != null ? new TrainInfo {
                Category = contract.TripLabel.Category ?? string.Empty,
                Number = contract.TripLabel.Number ?? string.Empty,
                Type = ParseTripType(contract.TripLabel.TripType ?? "p"),
                Owner = contract.TripLabel.Owner,
                FilterFlags = contract.TripLabel.FilterFlags
            } : new TrainInfo(),
            Arrival = arrival,
            Departure = departure,
            Messages = messages
        };
    }

    private static TripType ParseTripType(string? type) => 
        type != null && TripTypeMap.TryGetValue(type, out var tripType) ? tripType : TripType.Passenger;
}
