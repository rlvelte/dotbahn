using DotBahn.Modules.Shared.Transformer;
using DotBahn.Timetables.Contracts;
using DotBahn.Timetables.Models;

namespace DotBahn.Timetables.Transformers;

/// <summary>
/// Transformer for converting <see cref="StationContract"/> to <see cref="StationInfo"/>.
/// </summary>
public class StationTransformer : ITransformer<StationContract, StationInfo> {
    /// <inheritdoc />
    public StationInfo Transform(StationContract contract) {
        return new StationInfo {
            Eva = contract.Eva,
            Name = contract.Name
        };
    }
}
