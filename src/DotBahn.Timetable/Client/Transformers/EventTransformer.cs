using DotBahn.Parsing.Base;
using DotBahn.Timetable.Contracts;
using DotBahn.Timetable.Enumerations;
using DotBahn.Timetable.Models;
using DotBahn.Timetable.Models.Base;

namespace DotBahn.Timetable.Client.Transformers;

/// <summary>
/// Transformer for converting <see cref="EventContract"/> to <see cref="EventInfo"/>.
/// </summary>
public class EventTransformer : ITransformer<EventContract, EventInfo?>
{
    private static readonly Dictionary<string, EventStatus> StatusMap = new()
    {
        { "p", EventStatus.Planned },
        { "a", EventStatus.Added },
        { "c", EventStatus.Cancelled }
    };

    /// <inheritdoc />
    public EventInfo? Transform(EventContract raw)
    {
        if (string.IsNullOrEmpty(raw.PlannedTime))
            return null;

        var plannedTime = TransformerUtils.ParseApiTime(raw.PlannedTime);
        DateTime? changedTime = !string.IsNullOrEmpty(raw.ChangedTime) 
            ? TransformerUtils.ParseApiTime(raw.ChangedTime) 
            : null;

        var plannedPath = TransformerUtils.ParsePath(raw.PlannedPath);
        var changedPath = TransformerUtils.ParsePath(raw.ChangedPath);

        return new EventInfo
        {
            Time = new ChangedValue<DateTime>
            {
                Planned = plannedTime,
                Changed = (DateTime?)changedTime
            },
            Platform = new ChangedValue<string>
            {
                Planned = raw.PlannedPlatform ?? string.Empty,
                Changed = raw.ChangedPlatform
            },
            Path = new ChangedValue<List<string>>
            {
                Planned = plannedPath,
                Changed = changedPath.Count > 0 ? changedPath : null
            },
            Status = ParseEventStatus(raw.PlannedStatus ?? "p"),
            IsHidden = raw.IsHidden == "1",
            Line = raw.Line,
            DistantEndpoint = new ChangedValue<string>
            {
                Planned = raw.PlannedDistantEndpoint ?? string.Empty,
                Changed = raw.ChangedDistantEndpoint
            }
        };
    }

    private static EventStatus ParseEventStatus(string? status)
    {
        return status != null && StatusMap.TryGetValue(status, out var eventStatus) 
            ? eventStatus 
            : EventStatus.Planned;
    }
}
