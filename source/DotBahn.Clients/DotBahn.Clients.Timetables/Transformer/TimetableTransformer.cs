using System.Globalization;
using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Data.Timetables.Enumerations;
using DotBahn.Data.Timetables.Models;
using DotBahn.Data.Timetables.Models.Base;
using DotBahn.Modules.Shared.Enumerations;

namespace DotBahn.Clients.Timetables.Transformer;

/// <summary>
/// Transforms timetable contracts into domain models and handles merging of model updates.
/// </summary>
public class TimetableTransformer : ITransformer<Timetable, TimetableResponseContract> {
    private const string BahnTimeFormat = "yyMMddHHmm";

    #region Transforming 
    /// <inheritdoc />
    public Timetable Transform(in TimetableResponseContract contract) => new() {
            Station = contract.Station,
            Stops = contract.Stops.Select(TransformStop),
            Messages = []
        };

    /// <summary>
    /// Transforms the <see cref="StopDataContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model.</returns>
    private static TimetableStop TransformStop(StopDataContract contract) => new() {
            Id = contract.Id,
            Train = TransformTrainLabel(contract.TripInfo),
            Arrival = TransformEvent(contract.Arrival),
            Departure = TransformEvent(contract.Departure),
            Messages = contract.Messages?.Select(TransformMessage).ToList() ?? []
        };
    
    /// <summary>
    /// Transforms the <see cref="EventContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model.</returns>
    private static TrainEvent? TransformEvent(EventContract? contract) {
        if (contract == null) {
            return null;
        }
        
        var time = new ChangedValue<DateTime> {
            Original = ParseBahnTime(contract.PlannedTime) ?? default(DateTime),
            Updated = ParseBahnTime(contract.ChangedTime)
        };
        
        var platform = new ChangedRef<string> {
            Original = contract.PlannedPlatform ?? string.Empty,
            Updated = contract.ChangedPlatform
        };

        var changedStatus = EnumExtensions.FromAssociatedValue(contract.ChangedStatus, EventStatus.Unknown);
        var status = new ChangedValue<EventStatus> {
            Original = EnumExtensions.FromAssociatedValue(contract.PlannedStatus, EventStatus.Unknown),
            Updated = changedStatus != EventStatus.Unknown ? changedStatus : null
        };
        
        var path = new ChangedRef<IEnumerable<string>> {
            Original = ParsePipeSeparatedList(contract.PlannedPath) ?? [],
            Updated = ParsePipeSeparatedList(contract.ChangedPath)
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
            Wings = ParsePipeSeparatedList(contract.Wings) ?? [],
            Messages = []
        };
    }

    /// <summary>
    /// Transforms the <see cref="TripInfoContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model.</returns>
    private static TrainLabel TransformTrainLabel(TripInfoContract? contract) => new() {
            Category = contract?.Category ?? string.Empty,
            Number = contract?.Number ?? string.Empty,
            Owner = contract?.Owner ?? string.Empty,
            Type = string.IsNullOrEmpty(contract?.TripType) 
                ? null
                : EnumExtensions.FromAssociatedValue(contract.TripType, TripType.Passenger),
            FilterFlags = contract?.FilterFlags
        };

    /// <summary>
    /// Transforms the <see cref="MessageContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model.</returns>
    private static TimetableMessage TransformMessage(MessageContract contract) => new() {
            Id = contract.Id ?? string.Empty,
            Type = EnumExtensions.FromAssociatedValue(contract.Type, MessageType.Him),
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
    #endregion
    
    #region Merging
    /// <inheritdoc />
    public Timetable Merge(Timetable current, in Timetable changes) {
        var stops = current.Stops.ToDictionary(s => s.Id);
        foreach (var change in changes.Stops) {
            if (stops.TryGetValue(change.Id, out var existing)) {
                stops[change.Id] = MergeStop(existing, change);
            } else {
                stops[change.Id] = change;
            }
        }

        return new Timetable {
            Station = current.Station,
            Stops = stops.Values,
            Messages = MergeMessages(current.Messages, changes.Messages)
        };
    }

    /// <summary>
    /// Merges two <see cref="TrainEvent"/> in a new instance.
    /// </summary>
    /// <param name="current">The current value.</param>
    /// <param name="change">The changed value.</param>
    //// <returns>Instance with combined values.</returns>
    private static TimetableStop MergeStop(TimetableStop current, TimetableStop change) => new() {
        Id = current.Id,
        Train = current.Train,
        Arrival = MergeEvent(current.Arrival, change.Arrival),
        Departure = MergeEvent(current.Departure, change.Departure),
        Messages = MergeMessages(current.Messages, change.Messages)
    };

    /// <summary>
    /// Merges two <see cref="TrainEvent"/> in a new instance.
    /// </summary>
    /// <param name="current">The current value.</param>
    /// <param name="change">The changed value.</param>
    /// <returns>Instance with combined values.</returns>
    private static TrainEvent? MergeEvent(TrainEvent? current, TrainEvent? change) {
        if (current == null || change == null) {
            return change;
        }

        return new TrainEvent {
            Time = MergeValue(current.Time, change.Time),
            Platform = MergeRef(current.Platform, change.Platform),
            Status = MergeValue(current.Status, change.Status),
            DistantEndpoint = MergeRef(current.DistantEndpoint, change.DistantEndpoint),
            Path = MergeRef(current.Path, change.Path),
            Wings = change.Wings.Any() ? change.Wings : current.Wings,
            Messages = MergeMessages(current.Messages, change.Messages)
        };
    }

    /// <summary>
    /// Merges two <see cref="ChangedValue{T}"/> in a new instance.
    /// </summary>
    /// <param name="current">The current value.</param>
    /// <param name="change">The changed value.</param>
    /// <typeparam name="T">The type which can be changed.</typeparam>
    //// <returns>Instance with combined values.</returns>
    private static ChangedValue<T> MergeValue<T>(ChangedValue<T> current, ChangedValue<T> change)
        where T : struct {
        var hasRealUpdate = change.HasUpdate && !EqualityComparer<T>.Default.Equals(change.Updated!.Value, current.Original);
        return new ChangedValue<T> {
            Original = current.Original,
            Updated = hasRealUpdate ? change.Updated : current.Updated
        };
    }

    /// <summary>
    /// Merges two <see cref="ChangedRef{T}"/> in a new instance.
    /// </summary>
    /// <param name="current">The current value.</param>
    /// <param name="change">The changed value.</param>
    /// <typeparam name="T">The type which can be changed.</typeparam>
    //// <returns>Instance with combined values.</returns>
    private static ChangedRef<T> MergeRef<T>(ChangedRef<T> current, ChangedRef<T> change)
        where T : class {
        var hasRealUpdate = change.HasUpdate && !Equals(change.Updated, current.Original);
        return new ChangedRef<T> {
            Original = current.Original,
            Updated = hasRealUpdate ? change.Updated : current.Updated
        };
    }

    /// <summary>
    /// Merges two <see cref="TimetableMessage"/> in a combined list.
    /// </summary>
    /// <param name="current">The current value.</param>
    /// <param name="change">The changed value.</param>
    //// <returns>Instance with combined values.</returns>
    private static IEnumerable<TimetableMessage> MergeMessages(IEnumerable<TimetableMessage> current, IEnumerable<TimetableMessage> change) {
        var messages = current.ToList();
        
        var existing = messages.Select(m => m.Id).ToHashSet();
        return messages.Concat(change.Where(m => !existing.Contains(m.Id)));
    }
    #endregion

    /// <summary>
    /// Parses the Bahn time to a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="time">A bahn formatted time.</param>
    /// <returns>A parsed time.</returns>
    private static DateTime? ParseBahnTime(string? time) {
        if (string.IsNullOrEmpty(time)) {
            return null;
        }

        return DateTime.TryParseExact(time, BahnTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
            ? result
            : null;
    }

    /// <summary>
    /// Parses a pipe separated list to a <see cref="IReadOnlyList{T}"/>
    /// </summary>
    /// <param name="list">The list to seperate.</param>
    /// <returns>A paresd list.</returns>
    private static IReadOnlyList<string>? ParsePipeSeparatedList(string? list) => 
        string.IsNullOrEmpty(list) ? null : list.Split('|', StringSplitOptions.RemoveEmptyEntries);
}
