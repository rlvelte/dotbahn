using System.Globalization;
using DotBahn.Common.Models;
using DotBahn.Common.Transformer;
using DotBahn.Common.Utilities;
using DotBahn.Timetables.Internal.Contracts;
using DotBahn.Timetables.Models;
using DotBahn.Timetables.Models.Enumerations;
using TimetableJsonContext = DotBahn.Timetables.Internal.Json.TimetableJsonContext;

namespace DotBahn.Timetables.Internal.Transformers;

/// <summary>
/// Transforms timetable contracts into domain models
/// </summary>
internal sealed class TimetableTransformer : ITransformer<Timetable, TimetableResponseContract> {
    private const string BahnTimeFormat = "yyMMddHHmm";

    /// <inheritdoc />
    public Timetable Transform(TimetableResponseContract contracts) {
        ArgumentNullException.ThrowIfNull(contracts);
        return new Timetable {
            Station = contracts.Station,
            Stops = contracts.Stops.Select(TransformStop),
            Messages = []
        };
    }

    /// <summary>
    /// Transforms the <see cref="StopDataContract"/> into its domain model
    /// </summary>
    /// <param name="contract">The contract to transform</param>
    /// <returns>The transformed model</returns>
    private static TimetableStop TransformStop(StopDataContract contract) => new() {
        Id = contract.Id,
        Train = TransformTrainLabel(contract.TripInfo),
        Arrival = TransformEvent(contract.Arrival),
        Departure = TransformEvent(contract.Departure),
        Messages = contract.Messages?.Select(TransformMessage).ToList() ?? []
    };

    /// <summary>
    /// Transforms the <see cref="EventContract"/> into its domain model
    /// </summary>
    /// <param name="contract">The contract to transform</param>
    /// <returns>The transformed model</returns>
    private static TrainEvent? TransformEvent(EventContract? contract) {
        if (contract == null) {
            return null;
        }

        var time = new ChangedValue<DateTime> {
            Original = ParseBahnTime(contract.PlannedTime) ?? default,
            Updated = ParseBahnTime(contract.ChangedTime)
        };

        var platform = new ChangedRef<string> {
            Original = contract.PlannedPlatform ?? string.Empty,
            Updated = contract.ChangedPlatform
        };

        var changedStatus = EnumUtil.Parse(contract.ChangedStatus, EventStatus.Unknown, TimetableJsonContext.Default.EventStatus);
        var status = new ChangedValue<EventStatus> {
            Original = EnumUtil.Parse(contract.PlannedStatus, EventStatus.Unknown, TimetableJsonContext.Default.EventStatus),
            Updated = changedStatus != EventStatus.Unknown ? changedStatus : null
        };

        var path = new ChangedRef<IEnumerable<string>> {
            Original = ParsePipeSeparatedList(contract.PlannedPath),
            Updated = contract.ChangedPath is not null ? ParsePipeSeparatedList(contract.ChangedPath) : null
        };

        var distantEndpoint = new ChangedRef<string> {
            Original = contract.PlannedDistantEndpoint ?? string.Empty,
            Updated = contract.ChangedDistantEndpoint
        };

        return new TrainEvent {
            Time = time,
            Platform = platform,
            Status = status,
            DistantEndpoint = distantEndpoint,
            Path = path,
            Wings = ParsePipeSeparatedList(contract.Wings),
            Messages = []
        };
    }

    /// <summary>
    /// Transforms the <see cref="TripInfoContract"/> into its domain model
    /// </summary>
    /// <param name="contract">The contract to transform</param>
    /// <returns>The transformed model</returns>
    private static TrainLabel TransformTrainLabel(TripInfoContract? contract) => new() {
        Category = contract?.Category ?? string.Empty,
        Number = contract?.Number ?? string.Empty,
        Owner = contract?.Owner ?? string.Empty,
        Type = string.IsNullOrEmpty(contract?.TripType) ? null : EnumUtil.Parse(contract.TripType, TripType.Passenger, TimetableJsonContext.Default.TripType),
        FilterFlags = contract?.FilterFlags
    };

    /// <summary>
    /// Transforms the <see cref="MessageContract"/> into its domain model
    /// </summary>
    /// <param name="contract">The contract to transform</param>
    /// <returns>The transformed model</returns>
    private static TimetableMessage TransformMessage(MessageContract contract) => new() {
        Id = contract.Id ?? string.Empty,
        Type = EnumUtil.Parse(contract.Type, MessageType.Him, TimetableJsonContext.Default.MessageType),
        Timestamp = ParseBahnTime(contract.Timestamp) ?? new DateTime(),
        Code = int.TryParse(contract.Code, out var code) ? code : null,
        Category = contract.Category,
        ExternalCategory = contract.ExternalCategory,
        Priority = int.TryParse(contract.Priority, out var priority) ? (MessagePriority)priority : null,
        Owner = contract.Owner,
        ValidFrom = ParseBahnTime(contract.ValidFrom),
        ValidTo = ParseBahnTime(contract.ValidTo),
        InternalText = contract.IsInternal == "1" ? contract.Text : null,
        ExternalText = contract.IsInternal != "1" ? contract.Text : null,
        ExternalLink = null,
        IsDeleted = contract.IsDeleted == "1",
        AffectedTrips = []
    };

    /// <summary>
    /// Parses the Bahn time to a <see cref="DateTime"/>
    /// </summary>
    /// <param name="time">A bahn formatted time</param>
    /// <returns>A parsed time</returns>
    private static DateTime? ParseBahnTime(string? time) {
        if (string.IsNullOrEmpty(time)) {
            return null;
        }

        return DateTime.TryParseExact(time, BahnTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : null;
    }

    /// <summary>
    /// Parses a pipe-separated list to an array
    /// </summary>
    /// <param name="list">The list to separate</param>
    /// <returns>A parsed list</returns>
    private static string[] ParsePipeSeparatedList(string? list) => (string.IsNullOrEmpty(list) ? null : list.Split('|', StringSplitOptions.RemoveEmptyEntries)) ?? [];
}
