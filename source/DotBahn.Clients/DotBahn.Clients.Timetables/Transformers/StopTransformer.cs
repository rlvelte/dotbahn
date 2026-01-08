using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Enumerations;
using DotBahn.Clients.Timetables.Models;
using DotBahn.Modules.Shared.Enumerations;
using DotBahn.Modules.Shared.Transformer;

namespace DotBahn.Clients.Timetables.Transformers;

/// <summary>
/// Transformer for converting <see cref="StopDataContract"/> to <see cref="TrainStop"/>.
/// </summary>
public class StopTransformer(ITransformer<EventContract, EventInfo?> eventTransformer, ITransformer<MessageContract, Message> messageTransformer) : ITransformer<StopDataContract, TrainStop> {
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
                Type = EnumExtensions.FromAssociatedValue(contract.TripLabel.TripType, TripType.Passenger),
                Owner = contract.TripLabel.Owner,
                FilterFlags = contract.TripLabel.FilterFlags
            } : new TrainInfo(),
            Arrival = arrival,
            Departure = departure,
            Messages = messages
        };
    }
}
