using DotBahn.Shared.Models;
using DotBahn.Shared.Transformer;
using DotBahn.Timetables.Models;

namespace DotBahn.Timetables.Internal.Transformers;

/// <summary>
/// Merges timetable model updates into the current timetable.
/// </summary>
internal sealed class TimetableMerger : IMerger<Timetable> {
    /// <inheritdoc />
    public Timetable Merge(Timetable current, in Timetable changes) {
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(changes);

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
    /// Merges two <see cref="TimetableStop"/> in a new instance.
    /// </summary>
    /// <param name="current">The current value.</param>
    /// <param name="change">The changed value.</param>
    /// <returns>Instance with combined values.</returns>
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
    /// <typeparam name="T">The type that can be changed.</typeparam>
    /// <returns>Instance with combined values.</returns>
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
    /// <typeparam name="T">The type that can be changed.</typeparam>
    /// <returns>Instance with combined values.</returns>
    private static ChangedRef<T> MergeRef<T>(ChangedRef<T> current, ChangedRef<T> change)
        where T : class {
        var hasRealUpdate = change.HasUpdate && !Equals(change.Updated, current.Original);
        return new ChangedRef<T> {
            Original = current.Original,
            Updated = hasRealUpdate ? change.Updated : current.Updated
        };
    }

    /// <summary>
    /// Merges two message sequences, appending new messages while avoiding duplicates by ID.
    /// </summary>
    /// <param name="current">The current messages.</param>
    /// <param name="change">The changed messages.</param>
    /// <returns>Combined list of messages.</returns>
    private static IEnumerable<TimetableMessage> MergeMessages(IEnumerable<TimetableMessage> current, IEnumerable<TimetableMessage> change) {
        var messages = current.ToList();
        var existing = messages.Select(m => m.Id).ToHashSet();
        return messages.Concat(change.Where(m => !existing.Contains(m.Id)));
    }
}
