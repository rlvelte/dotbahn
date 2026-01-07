using DotBahn.Modules.Shared;
using DotBahn.Modules.Shared.Enumerations;
using DotBahn.Modules.Shared.Transformer;
using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Enumerations;
using DotBahn.TimetableApi.Models;
using DotBahn.TimetableApi.Utilities;

namespace DotBahn.TimetableApi.Transformers;

/// <summary>
/// Transformer for converting <see cref="EventContract"/> to <see cref="EventInfo"/>.
/// </summary>
public class EventTransformer : ITransformer<EventContract, EventInfo?> {
    /// <inheritdoc />
    public EventInfo? Transform(EventContract contract) {
        if (string.IsNullOrEmpty(contract.PlannedTime)) {
            return null;
        }

        var plannedTime = TransformerUtils.ParseApiTime(contract.PlannedTime);
        var changedTime = !string.IsNullOrEmpty(contract.ChangedTime) ? TransformerUtils.ParseApiTime(contract.ChangedTime) : (DateTime?)null;

        var plannedPath = TransformerUtils.ParsePath(contract.PlannedPath);
        var changedPath = TransformerUtils.ParsePath(contract.ChangedPath);

        return new EventInfo {
            Status = EnumExtensions.FromAssociatedValue(contract.PlannedStatus, EventStatus.Planned),
            IsHidden = contract.IsHidden == "1",
            Line = contract.Line,
            Time = new ChangedValue<DateTime?> {
                Planned = plannedTime,
                Changed = changedTime
            },
            Platform = new ChangedValue<string?> {
                Planned = contract.PlannedPlatform ?? string.Empty,
                Changed = contract.ChangedPlatform
            },
            Path = new ChangedValue<List<string>?> {
                Planned = plannedPath,
                Changed = changedPath.Count > 0 ? changedPath : null
            },
            DistantEndpoint = new ChangedValue<string?> {
                Planned = contract.PlannedDistantEndpoint ?? string.Empty,
                Changed = contract.ChangedDistantEndpoint
            }
        };
    }
}
